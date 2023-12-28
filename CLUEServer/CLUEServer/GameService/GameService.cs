using GameService.Contracts;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace GameService.Services
{
    public partial class GameService : IGameManager
    {
        public static int DiceRoll;
        Dictionary<string, Pawn> PawnsGamer = new Dictionary<string, Pawn>();

        public List<GridNode> AllowedCorners = new List<GridNode>();
        public List<GridNode> InvalidZones = new List<GridNode>();
        public List<Door> Doors = new List<Door>();

        private static readonly Dictionary<string, IGameManagerCallback> GamersInGameBoardCallback = new Dictionary<string, IGameManagerCallback>();

        public void AddToDoorsList(Door door)
        {
            Doors.Add(door);
        }

        public void AddToAllowedCorners(GridNode node)
        {
            AllowedCorners.Add(node);
        }

        public void AddToInvalidZones(GridNode node)
        {
            InvalidZones.Add(node);
        }

        public void MovePawn(int column, int row, string gamertag)
        {
            if (IsAValidMove(column, row, gamertag))
            {
                //Crear un pawn con la columan, fila y color 
                //Mandar el pawn
                //Cambiar la posición actual del pawn
            }
            else
            {
                //Envíar el pawn nulo
            }
        }

        public void ConnectGamerToGameBoard(string gamertag, string matchCode)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IGameManagerCallback>();

            if (!GamersInGameBoardCallback.ContainsKey(gamertag))
            {
                GamersInGameBoardCallback.Add(gamertag, callback);
                BroadcastNumberOfPlayersInGame(matchCode);
            }
        }

        private void BroadcastNumberOfPlayersInGame(string matchCode)
        {
            lock (GamersInGameBoardCallback)
            {
                foreach (var gamer in gamersInMatch)
                {
                    if (gamer.Value.Equals(matchCode) && GamersInGameBoardCallback.ContainsKey(gamer.Key))
                    {
                        GamersInGameBoardCallback[gamer.Key].UpdateNumberOfPlayersInGameboard(GetNumberOfGamers(matchCode));
                    }
                }
            }
        }

        private int GetNumberOfGamers(string matchCode)
        {
            int count = 0;

            foreach (var gamer in GetCharactersInMatch(matchCode))
            {
                if (GamersInGameBoardCallback.ContainsKey(gamer.Key))
                {
                    string gamertag = gamer.Key;
                    count++;
                }
            }

            return count;
        }

        public void GetGamersInGameboard(string gamertag, string code)
        {
            OperationContext.Current.GetCallbackChannel<IGameManagerCallback>().UpdateNumberOfPlayersInGameboard(GetNumberOfGamers(code));
        }

        public void DisconnectFromBoard(string gamertag, string matchCode)
        {
            lock (GamersInGameBoardCallback)
            {
                GamersInGameBoardCallback.Remove(gamertag);
            }

            RemoveFromMatch(gamertag);
            BroadcastNumberOfPlayersInGame(matchCode);
        }

        public GridNode GetPawnPosition(string gamertag) 
        {
            GridNode node = new GridNode();
            if (PawnsGamer.ContainsKey(gamertag))
            {
                Pawn pawn = PawnsGamer[gamertag];
                node.Xposition = pawn.XPosition;
                node.Yposition = pawn.YPosition;
            }
            return node;
        }

        public Boolean IsAValidMove(int column, int row, string gamertag)
        {
            Boolean isAValidMove    ;
            Door door = IsADoor(column, row);

            if (door != null) //Sí es una puerta
            {
                GridNode start = GetPawnPosition(gamertag);
                GridNode finish = new GridNode
                {
                    Xposition = column,
                    Yposition = row,
                };
                isAValidMove = AreTheStepsValid(start, finish, DiceRoll);
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
                    isAValidMove = AreTheStepsValid(start, finish, DiceRoll);
                }
                else
                {
                    isAValidMove = false;
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
                isAValidMove = AreTheStepsValid(start, finish, DiceRoll);
            }

            return isAValidMove;
        }

        public Boolean AreTheStepsValid(GridNode start, GridNode finish, int steps)
        {
            return DFSAlgoritm(start, finish, steps, new HashSet<GridNode>());
        }

        public Boolean DFSAlgoritm(GridNode current, GridNode finish, int steps, HashSet<GridNode> visited)
        {
            if (current.Xposition == finish.Xposition && current.Yposition == finish.Yposition && steps >= 0)
            {
                return true;
            }
            if (steps <= 0 || visited.Contains(current) || InvalidZones.Contains(current)) 
            {
                return false;
            }
            
            visited.Add(current);

            var neighbors = GetNeighbors(current, visited);

            foreach(var neighbor in neighbors)
            {
                if(DFSAlgoritm(neighbor, finish, steps-1, visited)){
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<GridNode> GetNeighbors(GridNode node, HashSet<GridNode> visited)
        {
            AddNeighbors(node.Xposition, node.Yposition - 1, visited);
            AddNeighbors(node.Xposition, node.Yposition + 1, visited);
            AddNeighbors(node.Xposition - 1, node.Yposition, visited);
            AddNeighbors(node.Xposition + 1, node.Yposition, visited);

            return visited;
        }
        
        public void AddNeighbors(int colum, int row, HashSet<GridNode> visited)
        {
            var neighbor = new GridNode
            {
                Xposition = colum,
                Yposition = row,
            };
            if (!visited.Contains(neighbor) && !InvalidZones.Contains(neighbor))
            {
                visited.Add(neighbor);
            }
        }

        public int RollDice()
        {
            Random random = new Random();
            int rollDice = random.Next(2, 13);
            DiceRoll = rollDice;
            return rollDice;
        }

        public Boolean IsAnInvalidZone (int column, int row)
        {
            Boolean isAnInvalidZone = false;
            if(column < 6)
            {
                if (row < 3) //Salón F103
                {
                    isAnInvalidZone = true;
                } else if(row > 4 && row < 10) //Salón cristal
                {
                    isAnInvalidZone = true;
                } else if (row > 10 && row < 16) //Laboratorio
                {
                    isAnInvalidZone = true;
                } else if (column < 5 && row > 17) //Cubículo
                {
                    isAnInvalidZone = true;
                }
            } else if(column >= 7 && column <= 14 ) 
            {
                if( row < 6 && column > 7 && column < 14)//Anfiteatro
                {
                    isAnInvalidZone = true;
                } else if (row > 15) //Centro de cómputo
                {
                    isAnInvalidZone = true;
                }
            }else
            {
                if(column > 15 && row < 5) //Cancha
                {
                    isAnInvalidZone = true;
                } else if(row > 7 && row < 15) //Estacionamiento
                {
                    isAnInvalidZone = true;
                }else if(column > 16 && row > 16) //Salón de profesores
                {
                    isAnInvalidZone = true;
                }
            }
            return isAnInvalidZone;
        }

        public Door IsADoor(int column, int row)
        {
            Door door = null;
            foreach(var grid in Doors)
            {
                if(grid.Xposition.Equals(column) && grid.Yposition.Equals(row))
                {
                    door.Xposition = grid.Xposition;
                    door.Yposition = grid.Yposition;
                    door.ZoneName = grid.ZoneName;
                    break;
                }
            }
            return door;
        }

        public Boolean IsAValidCorner(int column, int row)
        {
            Boolean isAValidCorner = false;
            foreach(var grid in AllowedCorners)
            {
                if(grid.Xposition.Equals(column) && grid.Yposition.Equals(row))
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

        public void CreatDoors()
        {
            Door roomf103 = new Door { Xposition = 5, Yposition = 2, ZoneName = "place6" };
            Door glassRoom = new Door { Xposition = 5, Yposition = 7, ZoneName = "place1" };
            Door glassRoom2 = new Door { Xposition = 2, Yposition = 9, ZoneName = "place1" };
            Door laboratory = new Door { Xposition = 0, Yposition = 11, ZoneName = "place8" };
            Door laboratory2 = new Door { Xposition = 4, Yposition = 14, ZoneName = "place8" };
            Door cubicle = new Door { Xposition = 3, Yposition = 18, ZoneName = "place7" };
            Door amphitheater = new Door { Xposition = 8, Yposition = 3, ZoneName = "place9" };
            Door amphitheater2 = new Door { Xposition = 10, Yposition = 5, ZoneName = "place9" };
            Door amphitheater3= new Door { Xposition = 11, Yposition = 5, ZoneName = "place9" };
            Door computingCenter = new Door { Xposition = 8, Yposition = 16, ZoneName = "place2" };
            Door computingCenter2 = new Door { Xposition = 13, Yposition = 16, ZoneName = "place2" };
            Door computingCenter3 = new Door { Xposition = 7, Yposition = 18, ZoneName = "place2" };
            Door computingCenter4 = new Door { Xposition = 14, Yposition = 18, ZoneName = "place2" };
            Door field = new Door { Xposition = 16, Yposition = 4, ZoneName = "place4" };
            Door parkingLot = new Door { Xposition = 16, Yposition = 8, ZoneName = "place5" };
            Door parkingLot2 = new Door { Xposition = 15, Yposition = 11, ZoneName = "place5" };
            Door professorsRoom = new Door { Xposition = 18, Yposition = 17, ZoneName = "place3" };
            this.AddToDoorsList(roomf103);
            this.AddToDoorsList(glassRoom);
            this.AddToDoorsList(glassRoom2);
            this.AddToDoorsList(laboratory);
            this.AddToDoorsList(laboratory2);
            this.AddToDoorsList(cubicle);
            this.AddToDoorsList(amphitheater);
            this.AddToDoorsList(amphitheater2);
            this.AddToDoorsList(amphitheater3);
            this.AddToDoorsList(computingCenter);
            this.AddToDoorsList(computingCenter2);
            this.AddToDoorsList(computingCenter3);
            this.AddToDoorsList(computingCenter4);
            this.AddToDoorsList(field);
            this.AddToDoorsList(parkingLot);
            this.AddToDoorsList(parkingLot2);
            this.AddToDoorsList(professorsRoom);
        }

        public void createValidCorners()
        {
            GridNode corner1 = new GridNode { Xposition = 5, Yposition = 5, };
            GridNode corner2 = new GridNode { Xposition = 5, Yposition = 9, };
            GridNode corner3 = new GridNode { Xposition = 4, Yposition = 18, };
            GridNode corner4 = new GridNode { Xposition = 8, Yposition = 22, };
            GridNode corner5 = new GridNode { Xposition = 7, Yposition = 22, };
            GridNode corner6 = new GridNode { Xposition = 13, Yposition = 22, };
            GridNode corner7 = new GridNode { Xposition = 14, Yposition = 22, };
            GridNode corner8 = new GridNode { Xposition = 17, Yposition = 14, };
            GridNode corner9 = new GridNode { Xposition = 16, Yposition = 14, };
            GridNode corner10 = new GridNode { Xposition = 15, Yposition = 14, };
            this.AddToAllowedCorners(corner1);
            this.AddToAllowedCorners(corner2);
            this.AddToAllowedCorners(corner3);  
            this.AddToAllowedCorners(corner4);
            this.AddToAllowedCorners(corner5);
            this.AddToAllowedCorners(corner6);
            this.AddToAllowedCorners(corner7);
            this.AddToAllowedCorners(corner8);
            this.AddToAllowedCorners(corner9);
            this.AddToAllowedCorners(corner10);
        }

        public void createInvaZones()
        {
            GridNode corner1 = new GridNode { Xposition = 0, Yposition = 2, };
            GridNode corner2 = new GridNode { Xposition = 1, Yposition = 2, };
            GridNode corner3 = new GridNode { Xposition = 2, Yposition = 2, };
            GridNode corner4 = new GridNode { Xposition = 3, Yposition = 2, };
            GridNode corner5 = new GridNode { Xposition = 4, Yposition = 2, };
            GridNode corner6 = new GridNode { Xposition = 5, Yposition = 1, };
            GridNode corner7 = new GridNode { Xposition = 5, Yposition = 0, };
            GridNode corner8 = new GridNode { Xposition = 4, Yposition = 1, };

            GridNode corner9 = new GridNode { Xposition = 0, Yposition = 5, };
            GridNode corner10 = new GridNode { Xposition = 1, Yposition = 5, };
            GridNode corner11 = new GridNode { Xposition = 2, Yposition = 5, };
            GridNode corner12 = new GridNode { Xposition = 3, Yposition = 5, };
            GridNode corner13 = new GridNode { Xposition = 4, Yposition = 5, };
            GridNode corner14 = new GridNode { Xposition = 5, Yposition = 6, };
            GridNode corner15 = new GridNode { Xposition = 5, Yposition = 8, };
            GridNode corner16 = new GridNode { Xposition = 4, Yposition = 9, };
            GridNode corner17 = new GridNode { Xposition = 3, Yposition = 9, };
            GridNode corner18 = new GridNode { Xposition = 1, Yposition = 9, };
            GridNode corner19 = new GridNode { Xposition = 0, Yposition = 9, };
            GridNode corner20 = new GridNode { Xposition = 4, Yposition = 7, };
            GridNode corner21 = new GridNode { Xposition = 2, Yposition = 8, };

            GridNode corner22 = new GridNode { Xposition = 0, Yposition = 12, };
            GridNode corner23 = new GridNode { Xposition = 1, Yposition = 11, };
            GridNode corner24 = new GridNode { Xposition = 2, Yposition = 11, };
            GridNode corner25 = new GridNode { Xposition = 3, Yposition = 11, };
            GridNode corner26 = new GridNode { Xposition = 4, Yposition = 11, };
            GridNode corner27 = new GridNode { Xposition = 4, Yposition = 12, };
            GridNode corner28 = new GridNode { Xposition = 4, Yposition = 13, };
            GridNode corner29 = new GridNode { Xposition = 3, Yposition = 14, };
            GridNode corner30 = new GridNode { Xposition = 0, Yposition = 15, };
            GridNode corner31 = new GridNode { Xposition = 1, Yposition = 15, };
            GridNode corner32 = new GridNode { Xposition = 2, Yposition = 15, };
            GridNode corner33 = new GridNode { Xposition = 3, Yposition = 15, };
            GridNode corner34 = new GridNode { Xposition = 4, Yposition = 15, };

            GridNode corner35 = new GridNode { Xposition = 0, Yposition = 18, };
            GridNode corner36 = new GridNode { Xposition = 1, Yposition = 18, };
            GridNode corner37 = new GridNode { Xposition = 2, Yposition = 18, };
            GridNode corner38 = new GridNode { Xposition = 4, Yposition = 19, };
            GridNode corner39 = new GridNode { Xposition = 4, Yposition = 20, };
            GridNode corner40 = new GridNode { Xposition = 4, Yposition = 21, };
            GridNode corner41 = new GridNode { Xposition = 3, Yposition = 19, };

            GridNode corner42 = new GridNode { Xposition = 8, Yposition = 0, };
            GridNode corner43 = new GridNode { Xposition = 8, Yposition = 1, };
            GridNode corner44 = new GridNode { Xposition = 8, Yposition = 2, };
            GridNode corner45 = new GridNode { Xposition = 9, Yposition = 3, };
            GridNode corner46 = new GridNode { Xposition = 8, Yposition = 4, };
            GridNode corner47 = new GridNode { Xposition = 8, Yposition = 5, };
            GridNode corner48 = new GridNode { Xposition = 9, Yposition = 5, };
            GridNode corner49 = new GridNode { Xposition = 10, Yposition = 4, };
            GridNode corner50 = new GridNode { Xposition = 11, Yposition = 4, };
            GridNode corner51 = new GridNode { Xposition = 12, Yposition = 5, };
            GridNode corner52 = new GridNode { Xposition = 13, Yposition = 5, };
            GridNode corner53 = new GridNode { Xposition = 13, Yposition = 4, };
            GridNode corner54 = new GridNode { Xposition = 13, Yposition = 3, };
            GridNode corner55 = new GridNode { Xposition = 13, Yposition = 2, };
            GridNode corner56 = new GridNode { Xposition = 13, Yposition = 1, };
            GridNode corner57 = new GridNode { Xposition = 13, Yposition = 0, };

            GridNode corner58 = new GridNode { Xposition = 7, Yposition = 16, };
            GridNode corner59 = new GridNode { Xposition = 7, Yposition = 17, };
            GridNode corner60 = new GridNode { Xposition = 8, Yposition = 18, };
            GridNode corner61 = new GridNode { Xposition = 7, Yposition = 19, };
            GridNode corner62 = new GridNode { Xposition = 7, Yposition = 20, };
            GridNode corner63 = new GridNode { Xposition = 7, Yposition = 21, };
            GridNode corner64 = new GridNode { Xposition = 8, Yposition = 21, };
            GridNode corner65 = new GridNode { Xposition = 9, Yposition = 22, };
            GridNode corner66 = new GridNode { Xposition = 9, Yposition = 16, };
            GridNode corner67 = new GridNode { Xposition = 10, Yposition = 16, };
            GridNode corner68 = new GridNode { Xposition = 11, Yposition = 16, };
            GridNode corner69 = new GridNode { Xposition = 12, Yposition = 16, };
            GridNode corner70 = new GridNode { Xposition = 13, Yposition = 17, };
            GridNode corner71 = new GridNode { Xposition = 14, Yposition = 16, };
            GridNode corner72 = new GridNode { Xposition = 14, Yposition = 17, };
            GridNode corner73 = new GridNode { Xposition = 13, Yposition = 18, };
            GridNode corner74 = new GridNode { Xposition = 14, Yposition = 19, };
            GridNode corner75 = new GridNode { Xposition = 14, Yposition = 20, };
            GridNode corner76 = new GridNode { Xposition = 14, Yposition = 21, };
            GridNode corner77 = new GridNode { Xposition = 13, Yposition = 21, };
            GridNode corner78 = new GridNode { Xposition = 12, Yposition = 22, };

            GridNode corner79 = new GridNode { Xposition = 16, Yposition = 0, };
            GridNode corner80 = new GridNode { Xposition = 16, Yposition = 1, };
            GridNode corner81 = new GridNode { Xposition = 16, Yposition = 2, };
            GridNode corner82 = new GridNode { Xposition = 16, Yposition = 3, };
            GridNode corner83 = new GridNode { Xposition = 17, Yposition = 4, };
            GridNode corner84 = new GridNode { Xposition = 18, Yposition = 4, };
            GridNode corner85 = new GridNode { Xposition = 19, Yposition = 4, };
            GridNode corner86 = new GridNode { Xposition = 20, Yposition = 4, };
            GridNode corner87 = new GridNode { Xposition = 21, Yposition = 4, };


            GridNode corner88 = new GridNode { Xposition = 15, Yposition = 8, };
            GridNode corner89 = new GridNode { Xposition = 15, Yposition = 9, };
            GridNode corner90 = new GridNode { Xposition = 15, Yposition = 10, }; 
            GridNode corner91 = new GridNode { Xposition = 16, Yposition = 11, }; 
            GridNode corner92 = new GridNode { Xposition = 15, Yposition = 12, }; 
            GridNode corner93 = new GridNode { Xposition = 15, Yposition = 13, }; 
            GridNode corner94 = new GridNode { Xposition = 16, Yposition = 13, }; 
            GridNode corner95 = new GridNode { Xposition = 17, Yposition = 13, }; 
            GridNode corner96 = new GridNode { Xposition = 18, Yposition = 14, }; 
            GridNode corner97 = new GridNode { Xposition = 19, Yposition = 14, }; 
            GridNode corner98 = new GridNode { Xposition = 20, Yposition = 14, }; 
            GridNode corner99 = new GridNode { Xposition = 21, Yposition = 14, }; 
            GridNode corner100 = new GridNode { Xposition = 16, Yposition = 9, }; 
            GridNode corner101 = new GridNode { Xposition = 17, Yposition = 8, }; 
            GridNode corner102 = new GridNode { Xposition = 18, Yposition = 8, }; 
            GridNode corner103 = new GridNode { Xposition = 19, Yposition = 8, }; 
            GridNode corner104 = new GridNode { Xposition = 20, Yposition = 8, }; 
            GridNode corner105 = new GridNode { Xposition = 21, Yposition = 8, };
            
            GridNode corner106 = new GridNode { Xposition = 21, Yposition = 17, }; 
            GridNode corner107 = new GridNode { Xposition = 20, Yposition = 17, }; 
            GridNode corner108 = new GridNode { Xposition = 19, Yposition = 17, };  
            GridNode corner109 = new GridNode { Xposition = 17, Yposition = 18, }; 
            GridNode corner110 = new GridNode { Xposition = 17, Yposition = 19, }; 
            GridNode corner111 = new GridNode { Xposition = 17, Yposition = 20, }; 
            GridNode corner112 = new GridNode { Xposition = 17, Yposition = 21, };
            GridNode corner113 = new GridNode { Xposition = 17, Yposition = 17, };
            GridNode corner114 = new GridNode { Xposition = 18, Yposition = 18, };
            
            GridNode corner115 = new GridNode { Xposition = -1, Yposition = 17, }; 
            GridNode corner116 = new GridNode { Xposition = -1, Yposition = 16, }; 
            GridNode corner117 = new GridNode { Xposition = -1  , Yposition = 10,}; 
            GridNode corner118 = new GridNode { Xposition = -1, Yposition = 4, }; 
            GridNode corner119 = new GridNode { Xposition = -1, Yposition = 3, }; 
            GridNode corner120 = new GridNode { Xposition = 6, Yposition = -1, }; 
            GridNode corner121 = new GridNode { Xposition = 7, Yposition = -1, }; 
            GridNode corner122 = new GridNode { Xposition = 14, Yposition = -1, }; 
            GridNode corner123 = new GridNode { Xposition = 15, Yposition = -1, }; 
            GridNode corner124 = new GridNode { Xposition = 22, Yposition = 5, }; 
            GridNode corner125 = new GridNode { Xposition = 22, Yposition = 6, }; 
            GridNode corner126 = new GridNode { Xposition = 22, Yposition = 7, }; 
            GridNode corner127 = new GridNode { Xposition = 22, Yposition = 15, }; 
            GridNode corner128 = new GridNode { Xposition = 22, Yposition = 16, }; 
            GridNode corner129 = new GridNode { Xposition = 16, Yposition = 22, }; 
            GridNode corner130 = new GridNode { Xposition = 15, Yposition = 23, }; 
            GridNode corner131 = new GridNode { Xposition = 14, Yposition = 23, }; 
            GridNode corner132 = new GridNode { Xposition = 13, Yposition = 23, }; 
            GridNode corner133 = new GridNode { Xposition = 8, Yposition = 23, }; 
            GridNode corner134 = new GridNode { Xposition = 7, Yposition = 23, }; 
            GridNode corner135 = new GridNode { Xposition = 6, Yposition = 23, }; 
            GridNode corner136 = new GridNode { Xposition = 5, Yposition = 21, };

            this.AddToInvalidZones(corner1);
            this.AddToInvalidZones(corner2);
            this.AddToInvalidZones(corner3);
            this.AddToInvalidZones(corner4);
            this.AddToInvalidZones(corner5);
            this.AddToInvalidZones(corner6);
            this.AddToInvalidZones(corner7);
            this.AddToInvalidZones(corner8);
            this.AddToInvalidZones(corner9);
            this.AddToInvalidZones(corner10);

            this.AddToInvalidZones(corner11);
            this.AddToInvalidZones(corner12);
            this.AddToInvalidZones(corner13);
            this.AddToInvalidZones(corner14);
            this.AddToInvalidZones(corner15);
            this.AddToInvalidZones(corner16);
            this.AddToInvalidZones(corner17);
            this.AddToInvalidZones(corner18);
            this.AddToInvalidZones(corner19);
            this.AddToInvalidZones(corner20);

            this.AddToInvalidZones(corner21);
            this.AddToInvalidZones(corner22);
            this.AddToInvalidZones(corner23);
            this.AddToInvalidZones(corner24);
            this.AddToInvalidZones(corner25);
            this.AddToInvalidZones(corner26);
            this.AddToInvalidZones(corner27);
            this.AddToInvalidZones(corner28);
            this.AddToInvalidZones(corner29);

            this.AddToInvalidZones(corner30);
            this.AddToInvalidZones(corner31);
            this.AddToInvalidZones(corner32);
            this.AddToInvalidZones(corner33);
            this.AddToInvalidZones(corner34);
            this.AddToInvalidZones(corner35);
            this.AddToInvalidZones(corner36);
            this.AddToInvalidZones(corner37);
            this.AddToInvalidZones(corner38);
            this.AddToInvalidZones(corner39);

            this.AddToInvalidZones(corner40);
            this.AddToInvalidZones(corner41);
            this.AddToInvalidZones(corner42);
            this.AddToInvalidZones(corner43);
            this.AddToInvalidZones(corner44);
            this.AddToInvalidZones(corner45);
            this.AddToInvalidZones(corner46);
            this.AddToInvalidZones(corner47);
            this.AddToInvalidZones(corner48);
            this.AddToInvalidZones(corner49);
            this.AddToInvalidZones(corner50);

            this.AddToInvalidZones(corner51);
            this.AddToInvalidZones(corner52);
            this.AddToInvalidZones(corner53);
            this.AddToInvalidZones(corner54);
            this.AddToInvalidZones(corner55);
            this.AddToInvalidZones(corner56);
            this.AddToInvalidZones(corner57);
            this.AddToInvalidZones(corner58);
            this.AddToInvalidZones(corner59);
            this.AddToInvalidZones(corner60);

            this.AddToInvalidZones(corner61);
            this.AddToInvalidZones(corner62);
            this.AddToInvalidZones(corner63);
            this.AddToInvalidZones(corner64);
            this.AddToInvalidZones(corner65);
            this.AddToInvalidZones(corner66);
            this.AddToInvalidZones(corner67);
            this.AddToInvalidZones(corner68);
            this.AddToInvalidZones(corner69);
            this.AddToInvalidZones(corner70);

            this.AddToInvalidZones(corner71);
            this.AddToInvalidZones(corner72);
            this.AddToInvalidZones(corner73);
            this.AddToInvalidZones(corner74);
            this.AddToInvalidZones(corner75);
            this.AddToInvalidZones(corner76);
            this.AddToInvalidZones(corner77);
            this.AddToInvalidZones(corner78);
            this.AddToInvalidZones(corner79);
            this.AddToInvalidZones(corner80);
            this.AddToInvalidZones(corner81);

            this.AddToInvalidZones(corner82);
            this.AddToInvalidZones(corner83);
            this.AddToInvalidZones(corner84);
            this.AddToInvalidZones(corner85);
            this.AddToInvalidZones(corner86);
            this.AddToInvalidZones(corner87);
            this.AddToInvalidZones(corner88);
            this.AddToInvalidZones(corner89);
            this.AddToInvalidZones(corner90);
            this.AddToInvalidZones(corner91);
            this.AddToInvalidZones(corner92);
            this.AddToInvalidZones(corner93);
            this.AddToInvalidZones(corner94);
            this.AddToInvalidZones(corner95);
            this.AddToInvalidZones(corner96);
            this.AddToInvalidZones(corner97);
            this.AddToInvalidZones(corner98);
            this.AddToInvalidZones(corner99);
            this.AddToInvalidZones(corner100);
            this.AddToInvalidZones(corner101);
                
            this.AddToInvalidZones(corner102);
            this.AddToInvalidZones(corner103);
            this.AddToInvalidZones(corner104);
            this.AddToInvalidZones(corner105);
            this.AddToInvalidZones(corner106);
            this.AddToInvalidZones(corner107);
            this.AddToInvalidZones(corner108);
            this.AddToInvalidZones(corner109);
            this.AddToInvalidZones(corner110);
            this.AddToInvalidZones(corner111);
            this.AddToInvalidZones(corner112);
            this.AddToInvalidZones(corner113);
                
            this.AddToInvalidZones(corner114);
            this.AddToInvalidZones(corner115);
            this.AddToInvalidZones(corner116);
            this.AddToInvalidZones(corner117);
            this.AddToInvalidZones(corner118);
            this.AddToInvalidZones(corner119);
            this.AddToInvalidZones(corner120);
            this.AddToInvalidZones(corner121);
            this.AddToInvalidZones(corner122);
            this.AddToInvalidZones(corner123);
            this.AddToInvalidZones(corner124);
            this.AddToInvalidZones(corner125);
            this.AddToInvalidZones(corner126);
            this.AddToInvalidZones(corner127);
            this.AddToInvalidZones(corner128);
            this.AddToInvalidZones(corner129);
            this.AddToInvalidZones(corner130);
            this.AddToInvalidZones(corner131);
            this.AddToInvalidZones(corner132);
            this.AddToInvalidZones(corner133);
            this.AddToInvalidZones(corner134);
            this.AddToInvalidZones(corner135);
            this.AddToInvalidZones(corner136);
        }
    }
}