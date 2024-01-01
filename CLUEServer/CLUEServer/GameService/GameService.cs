using GameService.Contracts;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.ModelConfiguration.Configuration;
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
        public string Left { get { return left; } set { left = value; } }
        public string Right { get { return right; } set { right = value; } }
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
                CreatCards(matchCode);
                SendFirstTurn(matchCode);
                
            }
        }

        private bool AreAllPlayersConnected(string matchCode)
        {
            int playersCount = 0;
            bool areAllPlayersConnected = false;
            List<string> gamersInLobby = GetGamersByMatch(matchCode);
            foreach (string gamer in gamersInLobby)
            {
                if (GamersInGameBoard.ContainsKey(gamer))
                {
                    playersCount++;
                }
            }

            if (playersCount == 3)
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
                lock (GamersInGameBoard)
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
                    pawn.Color = charactersPerGamer[gamertag].Color;
                    charactersPerGamer[gamertag].XPosition = column;
                    charactersPerGamer[gamertag].YPosition = row;
                }
                ShowMovePawn(pawn, GetMatchCode(gamertag));
                ChangeTurn(matchCode, gamertag);
            }
            else
            {
                ShowMoveIsInvalid(gamertag);
                GamersInGameBoardCallback[gamertag].ReceiveTurn(true);
            }
        }

        private string GetMatchCode(string gamertag)
        {
            string matchCode = string.Empty;
            foreach (var gamer in GamersInGameBoard)
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
                        if (gamer.Gamertag == gamertag)
                        {
                            GamersInGameBoardCallback[gamertag].ReceiveTurn(false);
                            GamersInGameBoardCallback[gamer.Left].ReceiveTurn(true);
                            break;
                        }
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
            ResetDice(matchCode);
        }

        private void ResetDice(string matchCode)
        {
            if (GameBoardDiceRoll.ContainsKey(matchCode))
            {
                GameBoardDiceRoll[matchCode] = 0;
            }
        }

        public void ShowMoveIsInvalid(string gamertag)
        {
            if (GamersInGameBoard.ContainsKey(gamertag))
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

        private int GetGameBoardRollDice(string matchCode)
        {
            int rollDice = 0;
            if (GameBoardDiceRoll.ContainsKey(matchCode))
            {
                rollDice = GameBoardDiceRoll[matchCode];
            }
            return rollDice;
        }

        private void SendAccusationOption(string gamertag, Door door)
        {
            if (GamersInGameBoardCallback.ContainsKey(gamertag))
            {
                GamersInGameBoardCallback[gamertag].ReceiveCommonAccusationOption(true, door);
            }
        }

        private Door GetDoor(int column, int row)
        {
            Door doorData = new Door();
            foreach (var door in Doors)
            {
                if (door.Xposition == column && door.Yposition == row)
                {
                    doorData.Xposition = door.Xposition;
                    doorData.Yposition = door.Yposition;
                    doorData.ZoneName = door.ZoneName;
                    break;
                }
            }
            return doorData;
        }

        public bool IsAValidMove(int column, int row, string gamertag, string matchCode)
        {
            bool isAValidMove = false;
            int rollDice = GetGameBoardRollDice(matchCode);
            Console.WriteLine("dados son: " + rollDice);
            if (IsADoor(column, row)) //Si es una puerta
            {
                Console.WriteLine("sí es una puerta");
                GridNode start = GetPawnPosition(gamertag);
                GridNode finish = new GridNode
                {
                    Xposition = column,
                    Yposition = row,
                };
                isAValidMove = AreTheStepsValid(start, finish, rollDice);

                if (isAValidMove)
                {
                    Door door = GetDoor(column, row);
                    SendAccusationOption(gamertag, door);
                } 
            }
            else if (IsAnInvalidZone(column, row)) //Sí es una zona prohibida
            {
                Console.WriteLine("sí es una zona inválida");
                if (IsAValidCorner(column, row))
                {
                    Console.WriteLine("pero es una corner valida");
                    GridNode start = GetPawnPosition(gamertag);
                    GridNode finish = new GridNode
                    {
                        Xposition = column,
                        Yposition = row,
                    };
                    isAValidMove = AreTheStepsValid(start, finish, rollDice);
                }
            }
            else if(IsTheCenter(column, row))
            {
                Console.WriteLine("Es el centro");
                GridNode start = GetPawnPosition(gamertag);
                GridNode finish = new GridNode
                {
                    Xposition = column,
                    Yposition = row,
                };
                isAValidMove = AreTheStepsValid(start, finish, rollDice);
                if(isAValidMove)
                {
                    GamersInGameBoardCallback[gamertag].ReceiveFinalAccusationOption(true);
                }
            } else
            {
                Console.WriteLine("zona válida");
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

        private bool IsTheCenter(int column, int row)
        {
            bool isTheCenter = false;
            if(column >= 8 && column <= 12 && row >= 7 && row <= 13)
            {
                isTheCenter = true;
            }
            return isTheCenter;
        }

        private bool AreTheStepsValid(GridNode start, GridNode finish, int rollDice)
        {
            bool isValidStep = false;
            Console.WriteLine("AreTheStepsValid: start: " + start.Xposition + "," + start.Yposition + " fin: " + finish.Xposition + "," + finish.Yposition + " dados: " + rollDice);
            if(rollDice != 0)
            {
                isValidStep = SearchMoves(start, start, finish, rollDice, new List<GridNode>(), new Queue<GridNode>());
            }
            return isValidStep;
        }

        private bool SearchMoves(GridNode start, GridNode current, GridNode end, int steps, List<GridNode> visitedNodes, Queue<GridNode> nextNodes)
        {
            Console.WriteLine("actual: " + current.Xposition + "," + current.Yposition);
            if (current.Xposition == end.Xposition && current.Yposition == end.Yposition && GetNumberOfSteps(start, current) >= 0)
            {
                Console.WriteLine("se encontró un camino");
                return true;
            }
            if (GetNumberOfSteps(start, current) > steps)
            {
                Console.WriteLine("numero de pasos invalido");
                return false;
            }      
            visitedNodes.Add(current);
            nextNodes = GetNeighbors(current, nextNodes, visitedNodes);
            GridNode nextNode = nextNodes.Dequeue();
            return SearchMoves(start, nextNode, end, steps, visitedNodes, nextNodes);
        }

        private int GetNumberOfSteps(GridNode start, GridNode end)
        {
            int differenceColumn = start.Xposition - end.Xposition;
            int differentRow = start.Yposition - end.Yposition;
            int total = Math.Abs(differenceColumn) + Math.Abs(differentRow);
            return total;
        }

        private Queue<GridNode> GetNeighbors(GridNode current, Queue<GridNode> nextNodes, List<GridNode> visitedNodes)
        {
            List<GridNode> neighbors = new List<GridNode>() {
                new GridNode() { Xposition = current.Xposition, Yposition = current.Yposition - 1 },
                new GridNode() { Xposition = current.Xposition, Yposition = current.Yposition + 1 },
                new GridNode() { Xposition = current.Xposition - 1, Yposition = current.Yposition },
                new GridNode() { Xposition = current.Xposition + 1, Yposition = current.Yposition },
            };

            foreach(GridNode node in neighbors) 
            {
                if (IsNeighborValid(node, visitedNodes))
                {
                    nextNodes.Enqueue(node);
                    Console.WriteLine("se agregó el siguiente nodo a vecinos: " + node.Xposition + "," + node.Yposition);
                }
            }
            return nextNodes;
        }

        private bool IsNeighborValid (GridNode neighbor, List<GridNode> visitedNodes)
        {
            bool isNeighborValid = false;
            if (!IsNeighborInVisitedNodes(neighbor, visitedNodes) && !IsNeighborInInvalidZone(neighbor))
            {
                isNeighborValid = true;
            }
            return isNeighborValid;
        }

        private bool IsNeighborInVisitedNodes(GridNode neighbor, List<GridNode> visitedNodes)
        {
            bool isNeighborVisited = false;
            foreach (GridNode node in visitedNodes) 
            {
                if(node.Xposition == neighbor.Xposition && node.Yposition == neighbor.Yposition)
                {
                    isNeighborVisited = true;
                }
            }
            return isNeighborVisited;
        }

        private bool IsNeighborInInvalidZone(GridNode neighbor)
        {
            bool isNeighborInvalid = false;
            foreach (GridNode node in InvalidZones)
            {
                if (node.Xposition == neighbor.Xposition && node.Yposition == neighbor.Yposition)
                {
                    isNeighborInvalid = true;
                }
            }

            return isNeighborInvalid;
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

        public void MakeFinalAccusation(List<string> cards, string matchCode, string gamertag)
        {
            int cardsCount = 0;
            if (clueDeckByMatch.ContainsKey(matchCode))
            {
                List<Card> clueDeck = clueDeckByMatch[matchCode];
                foreach (var card in clueDeck)
                {
                    foreach (var gamerCard in cards)
                    {
                        if (card.ID == gamerCard)
                        {
                            cardsCount++;
                        }
                    }
                }
            }

            if(cardsCount == 3)
            {
                string icon = GetIcon(gamertag);
                foreach (var gamer in GamersInGameBoard.ToList())
                {
                    if (gamer.Value.Equals(matchCode))
                    {
                        string gamerFound = gamer.Key;

                        if (GamersInGameBoardCallback.ContainsKey(gamerFound))
                        {
                            GamersInGameBoardCallback[gamerFound].ReceiveWinner(gamertag, icon);
                        }
                    }
                }
            }
            else
            {
                RemoveFromTurns(gamertag, matchCode);
            }
        }

        private void RemoveFromTurns(string gamertag, string matchCode)
        {
            string leftGamer = string.Empty;
            string rightGamer = string.Empty;
            int index = 0;
            if (TurnsGameBoard.ContainsKey(matchCode))
            {
                foreach (var gamer in TurnsGameBoard[matchCode])
                {
                    if (gamer.Gamertag == gamertag)
                    {
                        leftGamer = gamer.Left;
                        rightGamer = gamer.Right;
                        break;
                    }
                }

                foreach(var gamer in TurnsGameBoard[matchCode])
                {
                    if(gamer.Gamertag == rightGamer)
                    {
                        break;
                    }
                    index++;
                }
                TurnsGameBoard[matchCode][index].Left = leftGamer;
            }
        }



        public void ShowCard(Card card, string matchCode, string accuser)
        {
            GamersInGameBoardCallback[accuser].ReceiveCardAccused(card);
        }

        public void ShowCommonAccusation(string[] accusation, string matchCode, string accuser)
        {
            foreach (var gamer in GamersInGameBoard.ToList())
            {
                if (gamer.Value.Equals(matchCode))
                {
                    string gamertag = gamer.Key;

                    if (GamersInGameBoardCallback.ContainsKey(gamertag))
                    {
                        GamersInGameBoardCallback[gamertag].ReceiveCommonAccusationByOtherGamer(accusation);
                    }
                }
            }
            string leftGamer = GetLeftGamer(matchCode, accuser);
            IsLeftOwnerOfCards(accusation, accuser, leftGamer, matchCode);
        }

        private void IsLeftOwnerOfCards(string[] accusation, string accuser, string leftGamer,string matchCode)
        {
            if(accuser != leftGamer)
            {
                List<Card> leftGamerDeck = GetDeck(leftGamer);
                List<Card> cardsInCommon = new List<Card>();

                if (leftGamerDeck.Any())
                {
                    foreach (var card in leftGamerDeck)
                    {
                        if (accusation.Contains(card.ID))
                        {
                            cardsInCommon.Add(card);
                        }
                    }
                }
                if (cardsInCommon.Any())
                {
                    GamersInGameBoardCallback[leftGamer].RequestShowCard(cardsInCommon, accuser);

                } else
                {
                    string leftOfLeftGamer = GetLeftGamer(matchCode, leftGamer);
                    IsLeftOwnerOfCards(accusation, accuser, leftOfLeftGamer, matchCode);
                }
            }
            else
            {
                foreach (var gamer in GamersInGameBoard.ToList())
                {
                    if (gamer.Value.Equals(matchCode))
                    {
                        string gamertag = gamer.Key;

                        if (GamersInGameBoardCallback.ContainsKey(gamertag))
                        {
                            GamersInGameBoardCallback[gamertag].ShowNobodyAnswers();
                        }
                    }
                }
            }
        }

        private string GetLeftGamer(string matchCode, string gamertag)
        {
            string leftGamer = string.Empty;
            if(TurnsGameBoard.ContainsKey(matchCode))
            {
                foreach (var gamer in TurnsGameBoard[matchCode])
                {
                    if(gamer.Right == gamertag)
                    {
                        leftGamer = gamer.Gamertag;
                    }
                }
            }
            return leftGamer;
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

        public List<Card> GetDeck(string gamertag)
        {
            List<Card> gamerDeck = new List<Card>();
            if (decks.ContainsKey(gamertag))
            {
                Console.WriteLine("Sí está el jugador en el diccionario");
                gamerDeck = decks[gamertag];
            }
            else
            {
                Console.WriteLine("No se encontró al jugador: " + gamertag);
                Console.WriteLine("Los mazos guardados son los siguientes");
                Show();
            }

            Console.WriteLine("Sí pasé por el getDeck");
            return gamerDeck;
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
            new Door { Xposition = 5, Yposition = 2, ZoneName = "place6.png" },
            new Door { Xposition = 5, Yposition = 7, ZoneName = "place1.png" },
            new Door { Xposition = 2, Yposition = 9, ZoneName = "place1.png" },
            new Door { Xposition = 4, Yposition = 14, ZoneName = "place8.png" },
            new Door { Xposition = 3, Yposition = 18, ZoneName = "place7.png" },
            new Door { Xposition = 8, Yposition = 3, ZoneName = "place9.png" },
            new Door { Xposition = 10, Yposition = 5, ZoneName = "place9.png" },
            new Door { Xposition = 11, Yposition = 5, ZoneName = "place9.png" },
            new Door { Xposition = 8, Yposition = 16, ZoneName = "place2.png" },
            new Door { Xposition = 13, Yposition = 16, ZoneName = "place2.png" },
            new Door { Xposition = 7, Yposition = 18, ZoneName = "place2.png" },
            new Door { Xposition = 14, Yposition = 18, ZoneName = "place2.png" },
            new Door { Xposition = 16, Yposition = 4, ZoneName = "place4.png" },
            new Door { Xposition = 16, Yposition = 8, ZoneName = "place5.png" },
            new Door { Xposition = 15, Yposition = 11, ZoneName = "place5.png" },
            new Door { Xposition = 18, Yposition = 17, ZoneName = "place3.png" }
        };

    }
}