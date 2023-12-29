using GameService.Contracts;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace GameService.Services
{
    public class GamerLeftAndRight
    {
        private string gamertag;
        private string left;
        private string right;

        public string Gamertag { get { return gamertag; } set { gamertag = value; } }
        public string Left { get { return left; } set { left = value;} }
        public string Right { get { return right; } set { right = value;} }
    }

    public partial class GameService : IGameManager
    {
        private static readonly Dictionary<string, IGameManagerCallback> GamersInGameBoardCallback = new Dictionary<string, IGameManagerCallback>();
        private static readonly Dictionary<string, string> GamersInGameBoard = new Dictionary<string, string>();
        private static readonly Dictionary<string, List<GamerLeftAndRight>> TurnsGameBoard = new Dictionary<string, List<GamerLeftAndRight>>();
        private static readonly Dictionary<string, int> GameBoardDiceRoll = new Dictionary<string, int>();

        public void ConnectGamerToGameBoard(string gamertag, string matchCode)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IGameManagerCallback>();
            if (!GamersInGameBoardCallback.ContainsKey(gamertag))
            {
                GamersInGameBoardCallback.Add(gamertag, callback);
                GamersInGameBoard.Add(gamertag, matchCode);
                StartMatchInGameBoard(matchCode);
            }
            
        }

        private void StartMatchInGameBoard(string matchCode)
        {
            if (AreAllPlayersConnected(matchCode))
            {
                SetTurns(matchCode);
                SendFirstTurn(matchCode);
            }

        }

        private bool AreAllPlayersConnected(string matchCode)
        {
            int playersCount = 0;
            bool areAllPlayersConnected = false;

            List<string> gamersInLobby = GetGamersByMatch(matchCode);
            foreach(string gamer in gamersInLobby)
            {
                if (GamersInGameBoard.ContainsKey(gamer))
                {
                    playersCount++;
                }
            }

            if(playersCount == 3)
            {
                areAllPlayersConnected = true;
            }

            return areAllPlayersConnected;
        }
        private void SendFirstTurn(string matchCode)
        {
            List<GamerLeftAndRight> turnsList = TurnsGameBoard[matchCode];
            GamersInGameBoardCallback[turnsList[0].Gamertag].ReceiveTurn(true);
        }
        private void DisconnectAllPlayers(string matchCode)
        {
            lock (GamersInGameBoardCallback) 
            {
                foreach (var gamer in GamersInGameBoard)
                {
                    if (gamer.Value.Equals(matchCode) && GamersInGameBoardCallback.ContainsKey(gamer.Key))
                    {
                        GamersInGameBoardCallback[gamer.Key].LeaveGameBoard(); 
                    }
                }
            }
        }

        public int GetNumberOfGamers(string matchCode)  
        {
            return GetGamersByGameBoard(matchCode).Count();
        }

        private List<string> GetGamersByGameBoard(string matchCode)
        {
            return GamersInGameBoard.Where(gamer => gamer.Value == matchCode).Select(gamer => gamer.Key).ToList();
        }

        public void DisconnectFromBoard(string gamertag, string matchCode)
        {
            lock (GamersInGameBoardCallback)
            {
                lock(GamersInGameBoard)
                {
                    GamersInGameBoardCallback.Remove(gamertag);
                    GamersInGameBoard.Remove(gamertag);
                }
            }

            RemoveFromMatch(gamertag);
            //sacar a los de la partida
        }

        private void SetTurns(string matchCode)
        {
            List<string> gamerByBoard = GetGamersByGameBoard(matchCode);
            string gamer1 = gamerByBoard[0];
            string gamer2 = gamerByBoard[1];
            string gamer3 = gamerByBoard[2];

            List<GamerLeftAndRight> gamerLeftAndRights = new List<GamerLeftAndRight>()
            {
                new GamerLeftAndRight { Gamertag = gamer1, Left = gamer2, Right = gamer3},
                new GamerLeftAndRight { Gamertag = gamer2, Left = gamer3, Right = gamer1},
                new GamerLeftAndRight { Gamertag = gamer3, Left = gamer1, Right =  gamer2}
            };

            TurnsGameBoard.Add(matchCode, gamerLeftAndRights);
        }

        public void MovePawn(int column, int row, string gamertag, string matchCode)
        {
            Pawn pawn = new Pawn();
            if (IsAValidMove(column, row, gamertag, matchCode))
            {
                pawn.XPosition = column;
                pawn.YPosition = row;
                if (charactersPerGamer.ContainsKey(gamertag))
                {
                    charactersPerGamer[gamertag].XPosition = column;
                    charactersPerGamer[gamertag].YPosition = row;
                }
                ShowMovePawn(pawn, GetMatchCode(gamertag));
                ChangeTurn(matchCode, gamertag);
            } else
            {
                ShowMoveIsInvalid(gamertag);
                GamersInGameBoardCallback[gamertag].ReceiveTurn(true);
            }
        }

        private string GetMatchCode(string gamertag)
        {
            string matchCode = string.Empty;
            foreach(var gamer in GamersInGameBoard)
            {
                if (gamer.Key == gamertag)
                {
                    matchCode = gamer.Value;
                }
            }
            return matchCode;
        }

        public void ChangeTurn(string matchCode, string gamertag)
        {
            foreach (var turn in TurnsGameBoard)
            {
                if (turn.Key.Equals(matchCode))
                {
                    List<GamerLeftAndRight> GamersTurns = turn.Value;
                    foreach (var gamer in GamersTurns)
                    {
                        if(gamer.Gamertag == gamertag)
                        {
                            GamersInGameBoardCallback[gamertag].ReceiveTurn(false);
                            GamersInGameBoardCallback[gamer.Left].ReceiveTurn(true);
                        }
                        break;
                    }
                    break;
                }
            }
        }

        public void ShowMovePawn(Pawn pawn, string matchCode)
        {
                foreach (var gamer in GamersInGameBoard.ToList())
                {
                    if (gamer.Value.Equals(matchCode))
                    {
                        string gamertag = gamer.Key;

                        if (GamersInGameBoardCallback.ContainsKey(gamertag))
                        {
                            GamersInGameBoardCallback[gamertag].ReceivePawnsMove(pawn);
                        }
                    }
                }
        }

        public void ShowMoveIsInvalid(string gamertag)
        {
            if(GamersInGameBoard.ContainsKey(gamertag))
            {
                if (GamersInGameBoardCallback.ContainsKey(gamertag))
                {
                    GamersInGameBoardCallback[gamertag].ReceiveInvalidMove();
                }
            }
        } 

        public GridNode GetPawnPosition(string gamertag)
        {
            GridNode node = new GridNode();
            if (charactersPerGamer.ContainsKey(gamertag))
            {
                Pawn pawn = charactersPerGamer[gamertag];
                node.Xposition = pawn.XPosition;
                node.Yposition = pawn.YPosition;
            }
            return node;
        }

        private int GetGameBoardRillDice(string matchCode)
        {
            int rollDice = 0;
            foreach(var match in GameBoardDiceRoll)
            {
                if(match.Key == matchCode)
                {
                    rollDice = match.Value;
                }
            }
            return rollDice;
        }

        public bool IsAValidMove(int column, int row, string gamertag, string matchCode)
        {
            bool isAValidMove = false;
            int rollDice = GetGameBoardRillDice(matchCode);
            Console.WriteLine("dados son: " + rollDice);
            if (IsADoor(column, row)) //Si es una puerta
            {
                GridNode start = GetPawnPosition(gamertag);
                GridNode finish = new GridNode
                {
                    Xposition = column,
                    Yposition = row,
                };
                isAValidMove = AreTheStepsValid(start, finish, rollDice);
            }
            else if (IsAnInvalidZone(column, row)) //Sí es una zona prohibida
            {
                if (IsAValidCorner(column, row))
                {
                    GridNode start = GetPawnPosition(gamertag);
                    GridNode finish = new GridNode
                    {
                        Xposition = column,
                        Yposition = row,
                    };
                    isAValidMove = AreTheStepsValid(start, finish, rollDice);
                }
            }
            else
            {
                GridNode start = GetPawnPosition(gamertag);
                GridNode finish = new GridNode
                {
                    Xposition = column,
                    Yposition = row,
                };
                isAValidMove = AreTheStepsValid(start, finish, rollDice);
            }

            return isAValidMove;
        }

        public bool AreTheStepsValid(GridNode start, GridNode finish, int steps)
        {
            return DFSAlgoritm(start, finish, steps, new HashSet<GridNode>());
        }

        public bool DFSAlgoritm(GridNode current, GridNode finish, int steps, HashSet<GridNode> visited)
        {
            if (current.Xposition == finish.Xposition && current.Yposition == finish.Yposition && steps >= 0)
            {
                return true;
            }

            if (steps == 0 || visited.Contains(current) || InvalidZones.Contains(current))
            {
                return false;
            }

            visited.Add(current);

            var neighbors = GetNeighbors(current, visited);

            foreach (var neighbor in neighbors)
            {
                if (DFSAlgoritm(neighbor, finish, steps - 1, visited))
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<GridNode> GetNeighbors(GridNode node, HashSet<GridNode> visited)
        {
            return AddNeighbors(node.Xposition, node.Yposition - 1, visited)
            .Concat(AddNeighbors(node.Xposition, node.Yposition + 1, visited))
            .Concat(AddNeighbors(node.Xposition - 1, node.Yposition, visited))
            .Concat(AddNeighbors(node.Xposition + 1, node.Yposition, visited));
        }
            
        public IEnumerable<GridNode> AddNeighbors(int column, int row, HashSet<GridNode> visited)
        {
            var neighbor = new GridNode
            {
                Xposition = column,
                Yposition = row,
            };

            if (!visited.Contains(neighbor) && !InvalidZones.Contains(neighbor))
            {
                visited.Add(neighbor);
                yield return neighbor;
            }
        }

        public int RollDice(string matchCode)
        {
            Random random = new Random();
            int rollDice = random.Next(2, 13);
            if (GameBoardDiceRoll.ContainsKey(matchCode))
            {
                GameBoardDiceRoll[matchCode] = rollDice;
            }
            else
            {
                GameBoardDiceRoll.Add(matchCode, rollDice);
            }
            return rollDice;
        }

        public bool IsAnInvalidZone(int column, int row)
        {
            bool isAnInvalidZone = false;
            if (column < 6)
            {
                if (row < 3) //Salón F103
                {
                    isAnInvalidZone = true;
                }
                else if (row > 4 && row < 10) //Salón cristal
                {
                    isAnInvalidZone = true;
                }
                else if (row > 10 && row < 16 && column < 5) //Laboratorio
                {
                    isAnInvalidZone = true;
                }
                else if (column < 5 && row > 17) //Cubículo
                {
                    isAnInvalidZone = true;
                }
            }
            else if (column >= 7 && column <= 14)
            {
                if (row < 6 && column > 7 && column < 14)//Anfiteatro
                {
                    isAnInvalidZone = true;
                }
                else if (row > 15) //Centro de cómputo
                {
                    isAnInvalidZone = true;
                }
            }
            else
            {
                if (column > 15 && row < 5) //Cancha
                {
                    isAnInvalidZone = true;
                }
                else if (row > 7 && row < 15 && column >= 15) //Estacionamiento
                {
                    isAnInvalidZone = true;
                }
                else if (column > 16 && row > 16) //Salón de profesores
                {
                    isAnInvalidZone = true;
                }
            }
            Console.WriteLine("es zona inválida " + isAnInvalidZone);
            return isAnInvalidZone;
        }

        public bool IsADoor(int column, int row)
        {
            bool isADoor = false;
            foreach (var door in Doors)
            {
                if (door.Xposition == column && door.Yposition == row)
                {
                    isADoor = true;
                    break;
                }
            }
            return isADoor;
        }

        public bool IsAValidCorner(int column, int row)
        {
            bool isAValidCorner = false;
            foreach (var grid in AllowedCorners)
            {
                if (grid.Xposition.Equals(column) && grid.Yposition.Equals(row))
                {
                    isAValidCorner = true;
                }
            }
            return isAValidCorner;
        }

        public List<Pawn> CreatePawns()
        {
            Pawn bluePawn = new Pawn { Color = "BluePawn.png", XPosition = 0, YPosition = 17 };
            Pawn purplePawn = new Pawn { Color = "PurplePawn.png", XPosition = 0, YPosition = 4 };
            Pawn whitePawn = new Pawn { Color = "WhitePawn.png", XPosition = 13, YPosition = 22 };
            Pawn redPawn = new Pawn { Color = "RedPawn.png", XPosition = 15, YPosition = 0 };
            Pawn yellowPawn = new Pawn { Color = "YellowPawn.png", XPosition = 21, YPosition = 6 };
            Pawn greenPawn = new Pawn { Color = "GreenPawn.png", XPosition = 8, YPosition = 22 };

            List<Pawn> pawns = new List<Pawn>();
            pawns.Add(bluePawn);
            pawns.Add(purplePawn);
            pawns.Add(whitePawn);
            pawns.Add(redPawn);
            pawns.Add(yellowPawn);
            pawns.Add(greenPawn);
            return pawns;
        }
        public List<GridNode> AllowedCorners = new List<GridNode>()
        {
            new GridNode { Xposition = 5, Yposition = 5,},
            new GridNode { Xposition = 5, Yposition = 9,},
            new GridNode { Xposition = 4, Yposition = 18,},
            new GridNode { Xposition = 8, Yposition = 22,},
            new GridNode { Xposition = 7, Yposition = 22,},
            new GridNode { Xposition = 13, Yposition = 22,},
            new GridNode { Xposition = 14, Yposition = 22,},
            new GridNode { Xposition = 17, Yposition = 14,},
            new GridNode { Xposition = 16, Yposition = 14,},
            new GridNode { Xposition = 15, Yposition = 14,}
    };
        public List<GridNode> InvalidZones { get; set; } = new List<GridNode>()
        {
            new GridNode { Xposition = 0, Yposition = 2, },
            new GridNode { Xposition = 1, Yposition = 2, },
            new GridNode { Xposition = 2, Yposition = 2, },
            new GridNode { Xposition = 3, Yposition = 2, },
            new GridNode { Xposition = 4, Yposition = 2, },
            new GridNode { Xposition = 5, Yposition = 1, },
            new GridNode { Xposition = 5, Yposition = 0, },
            new GridNode { Xposition = 4, Yposition = 1, },

            new GridNode { Xposition = 0, Yposition = 5, },
            new GridNode { Xposition = 1, Yposition = 5, },
            new GridNode { Xposition = 2, Yposition = 5, },
            new GridNode { Xposition = 3, Yposition = 5, },
            new GridNode { Xposition = 4, Yposition = 5, },
            new GridNode { Xposition = 5, Yposition = 6, },
            new GridNode { Xposition = 5, Yposition = 8, },
            new GridNode { Xposition = 4, Yposition = 9, },
            new GridNode { Xposition = 3, Yposition = 9, },
            new GridNode { Xposition = 1, Yposition = 9, },
            new GridNode { Xposition = 0, Yposition = 9, },
            new GridNode { Xposition = 4, Yposition = 7, },
            new GridNode { Xposition = 2, Yposition = 8, },

            new GridNode { Xposition = 0, Yposition = 12, },
            new GridNode { Xposition = 1, Yposition = 11, },
            new GridNode { Xposition = 2, Yposition = 11, },
            new GridNode { Xposition = 3, Yposition = 11, },
            new GridNode { Xposition = 4, Yposition = 11, },
            new GridNode { Xposition = 4, Yposition = 12, },
            new GridNode { Xposition = 4, Yposition = 13, },
            new GridNode { Xposition = 3, Yposition = 14, },
            new GridNode { Xposition = 0, Yposition = 15, },
            new GridNode { Xposition = 1, Yposition = 15, },
            new GridNode { Xposition = 2, Yposition = 15, },
            new GridNode { Xposition = 3, Yposition = 15, },
            new GridNode { Xposition = 4, Yposition = 15, },

            new GridNode { Xposition = 0, Yposition = 18, },
            new GridNode { Xposition = 1, Yposition = 18, },
            new GridNode { Xposition = 2, Yposition = 18, },
            new GridNode { Xposition = 4, Yposition = 19, },
            new GridNode { Xposition = 4, Yposition = 20, },
            new GridNode { Xposition = 4, Yposition = 21, },
            new GridNode { Xposition = 3, Yposition = 19, },

            new GridNode { Xposition = 8, Yposition = 0, },
            new GridNode { Xposition = 8, Yposition = 1, },
            new GridNode { Xposition = 8, Yposition = 2, },
            new GridNode { Xposition = 9, Yposition = 3, },
            new GridNode { Xposition = 8, Yposition = 4, },
            new GridNode { Xposition = 8, Yposition = 5, },
            new GridNode { Xposition = 9, Yposition = 5, },
            new GridNode { Xposition = 10, Yposition = 4, },
            new GridNode { Xposition = 11, Yposition = 4, },
            new GridNode { Xposition = 12, Yposition = 5, },
            new GridNode { Xposition = 13, Yposition = 5, },
            new GridNode { Xposition = 13, Yposition = 4, },
            new GridNode { Xposition = 13, Yposition = 3, },
            new GridNode { Xposition = 13, Yposition = 2, },
            new GridNode { Xposition = 13, Yposition = 1, },
            new GridNode { Xposition = 13, Yposition = 0, },

            new GridNode { Xposition = 7, Yposition = 16, },
            new GridNode { Xposition = 7, Yposition = 17, },
            new GridNode { Xposition = 8, Yposition = 18, },
            new GridNode { Xposition = 7, Yposition = 19, },
            new GridNode { Xposition = 7, Yposition = 20, },
            new GridNode { Xposition = 7, Yposition = 21, },
            new GridNode { Xposition = 8, Yposition = 21, },
            new GridNode { Xposition = 9, Yposition = 22, },
            new GridNode { Xposition = 9, Yposition = 16, },
            new GridNode { Xposition = 10, Yposition = 16, },
            new GridNode { Xposition = 11, Yposition = 16, },
            new GridNode { Xposition = 12, Yposition = 16, },
            new GridNode { Xposition = 13, Yposition = 17, },
            new GridNode { Xposition = 14, Yposition = 16, },
            new GridNode { Xposition = 14, Yposition = 17, },
            new GridNode { Xposition = 13, Yposition = 18, },
            new GridNode { Xposition = 14, Yposition = 19, },
            new GridNode { Xposition = 14, Yposition = 20, },
            new GridNode { Xposition = 14, Yposition = 21, },
            new GridNode { Xposition = 13, Yposition = 21, },
            new GridNode { Xposition = 12, Yposition = 22, },

            new GridNode { Xposition = 16, Yposition = 0, },
            new GridNode { Xposition = 16, Yposition = 1, },
            new GridNode { Xposition = 16, Yposition = 2, },
            new GridNode { Xposition = 16, Yposition = 3, },
            new GridNode { Xposition = 17, Yposition = 4, },
            new GridNode { Xposition = 18, Yposition = 4, },
            new GridNode { Xposition = 19, Yposition = 4, },
            new GridNode { Xposition = 20, Yposition = 4, },
            new GridNode { Xposition = 21, Yposition = 4, },


            new GridNode { Xposition = 15, Yposition = 8, },
            new GridNode { Xposition = 15, Yposition = 9, },
            new GridNode { Xposition = 15, Yposition = 10, },
            new GridNode { Xposition = 16, Yposition = 11, },
            new GridNode { Xposition = 15, Yposition = 12, },
            new GridNode { Xposition = 15, Yposition = 13, },
            new GridNode { Xposition = 16, Yposition = 13, },
            new GridNode { Xposition = 17, Yposition = 13, },
            new GridNode { Xposition = 18, Yposition = 14, },
            new GridNode { Xposition = 19, Yposition = 14, },
            new GridNode { Xposition = 20, Yposition = 14, },
            new GridNode { Xposition = 21, Yposition = 14, },
            new GridNode { Xposition = 16, Yposition = 9, },
            new GridNode { Xposition = 17, Yposition = 8, },
            new GridNode { Xposition = 18, Yposition = 8, },
            new GridNode { Xposition = 19, Yposition = 8, },
            new GridNode { Xposition = 20, Yposition = 8, },
            new GridNode { Xposition = 21, Yposition = 8, },

            new GridNode { Xposition = 21, Yposition = 17, },
            new GridNode { Xposition = 20, Yposition = 17, },
            new GridNode { Xposition = 19, Yposition = 17, },
            new GridNode { Xposition = 17, Yposition = 18, },
            new GridNode { Xposition = 17, Yposition = 19, },
            new GridNode { Xposition = 17, Yposition = 20, },
            new GridNode { Xposition = 17, Yposition = 21, },
            new GridNode { Xposition = 17, Yposition = 17, },
            new GridNode { Xposition = 18, Yposition = 18, },

            new GridNode { Xposition = -1, Yposition = 17, },
            new GridNode { Xposition = -1, Yposition = 16, },
            new GridNode { Xposition = -1, Yposition = 10, },
            new GridNode { Xposition = -1, Yposition = 4, },
            new GridNode { Xposition = -1, Yposition = 3, },
            new GridNode { Xposition = 6, Yposition = -1, },
            new GridNode { Xposition = 7, Yposition = -1, },
            new GridNode { Xposition = 14, Yposition = -1, },
            new GridNode { Xposition = 15, Yposition = -1, },
            new GridNode { Xposition = 22, Yposition = 5, },
            new GridNode { Xposition = 22, Yposition = 6, },
            new GridNode { Xposition = 22, Yposition = 7, },
            new GridNode { Xposition = 22, Yposition = 15, },
            new GridNode { Xposition = 22, Yposition = 16, },
            new GridNode { Xposition = 16, Yposition = 22, },
            new GridNode { Xposition = 15, Yposition = 23, },
            new GridNode { Xposition = 14, Yposition = 23, },
            new GridNode { Xposition = 13, Yposition = 23, },
            new GridNode { Xposition = 8, Yposition = 23, },
            new GridNode { Xposition = 7, Yposition = 23, },
            new GridNode { Xposition = 6, Yposition = 23, },
            new GridNode { Xposition = 5, Yposition = 21, },
        };
        public List<Door> Doors { get; set; } = new List<Door>
        {
            new Door { Xposition = 5, Yposition = 2, ZoneName = "place6" },
            new Door { Xposition = 5, Yposition = 7, ZoneName = "place1" },
            new Door { Xposition = 2, Yposition = 9, ZoneName = "place1" },
            new Door { Xposition = 4, Yposition = 14, ZoneName = "place8" },
            new Door { Xposition = 3, Yposition = 18, ZoneName = "place7" },
            new Door { Xposition = 8, Yposition = 3, ZoneName = "place9" },
            new Door { Xposition = 10, Yposition = 5, ZoneName = "place9" },
            new Door { Xposition = 11, Yposition = 5, ZoneName = "place9" },
            new Door { Xposition = 8, Yposition = 16, ZoneName = "place2" },
            new Door { Xposition = 13, Yposition = 16, ZoneName = "place2" },
            new Door { Xposition = 7, Yposition = 18, ZoneName = "place2" },
            new Door { Xposition = 14, Yposition = 18, ZoneName = "place2" },
            new Door { Xposition = 16, Yposition = 4, ZoneName = "place4" },
            new Door { Xposition = 16, Yposition = 8, ZoneName = "place5" },
            new Door { Xposition = 15, Yposition = 11, ZoneName = "place5" },
            new Door { Xposition = 18, Yposition = 17, ZoneName = "place3" }
        };

    }
}