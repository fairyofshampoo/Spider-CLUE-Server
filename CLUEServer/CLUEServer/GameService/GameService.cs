using GameService.Contracts;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Services
{
    public partial class GameService : IGameManager
    {
        public static int DiceRoll;
        Dictionary<string, Pawn> PawnsGamer = new Dictionary<string, Pawn>();
        public List<RestrictedSquare> RoomF103 = new List<RestrictedSquare>();
        public List<RestrictedSquare> GlassRoom = new List<RestrictedSquare>();
        public List<RestrictedSquare> Laboratory = new List<RestrictedSquare>();
        public List<RestrictedSquare> Cubicle = new List<RestrictedSquare>();
        public List<RestrictedSquare> Amphitheater = new List<RestrictedSquare>();
        public List<RestrictedSquare> ComputerLab = new List<RestrictedSquare>();
        public List<RestrictedSquare> Field = new List<RestrictedSquare>();
        public List<RestrictedSquare> ParkingLot = new List<RestrictedSquare>();
        public List<RestrictedSquare> ProfessorsRoom = new List<RestrictedSquare>();

        public void MovePawn(int columns, int rows, string gamertag)
        {
            if(IsMoveValid(columns, rows))
            {

            }
        }

        public List<Pawn> CreatePawns()
        {
            Pawn bluePawn = new Pawn { Color = "", XPosition = 0, YPosition = 17 };
            Pawn purplePawn = new Pawn { Color = "", XPosition = 0, YPosition = 4 };
            Pawn whitePawn = new Pawn { Color = "", XPosition = 13, YPosition = 22 };
            Pawn redPawn = new Pawn { Color = "", XPosition = 15, YPosition = 0 };
            Pawn yellowPawn = new Pawn { Color = "", XPosition = 21, YPosition = 6 };
            Pawn greenPawn = new Pawn { Color = "", XPosition = 8, YPosition = 22 };

            List<Pawn> pawns = new List<Pawn>();
            pawns.Add(bluePawn);
            pawns.Add(purplePawn);
            pawns.Add(whitePawn);
            pawns.Add(redPawn);
            pawns.Add(yellowPawn);
            pawns.Add(greenPawn);
            return pawns;
        }

        public void FillListProhibitedAreas()
        {
            FillRommF103List();
            FillGlassRoomList();
            FillLaboratoryList();
            FillCubicleList();
            FillAmphitheaterList();
            FillComputerLabList();
            FillFieldList();
            FillParkingLotList();
            FillProfessorsRoom();
        }

        public void FillRommF103List()
        {
            RestrictedSquare zone1 = new RestrictedSquare {Xposition = 0, Yposition = 0};
            RestrictedSquare zone2 = new RestrictedSquare {Xposition = 1, Yposition = 0};
            RestrictedSquare zone3 = new RestrictedSquare {Xposition = 2, Yposition = 0};
            RestrictedSquare zone4 = new RestrictedSquare {Xposition = 3, Yposition = 0};
            RestrictedSquare zone5 = new RestrictedSquare {Xposition = 4, Yposition = 0};
            RestrictedSquare zone6 = new RestrictedSquare {Xposition = 5, Yposition = 0};
            RestrictedSquare zone7 = new RestrictedSquare {Xposition = 0, Yposition = 1};
            RestrictedSquare zone8 = new RestrictedSquare {Xposition = 1, Yposition = 1};
            RestrictedSquare zone9 = new RestrictedSquare {Xposition = 2, Yposition = 1};
            RestrictedSquare zone10 = new RestrictedSquare {Xposition = 3, Yposition = 1};
            RestrictedSquare zone11 = new RestrictedSquare {Xposition = 4, Yposition = 1};
            RestrictedSquare zone12 = new RestrictedSquare {Xposition = 5, Yposition = 1};
            RestrictedSquare zone13 = new RestrictedSquare {Xposition = 0, Yposition = 2};
            RestrictedSquare zone14 = new RestrictedSquare {Xposition = 1, Yposition = 2};
            RestrictedSquare zone15 = new RestrictedSquare {Xposition = 2, Yposition = 2};
            RestrictedSquare zone16 = new RestrictedSquare {Xposition = 3, Yposition = 2};
            RestrictedSquare zone17 = new RestrictedSquare {Xposition = 4, Yposition = 2};
            RoomF103.Add(zone1);
            RoomF103.Add(zone2); 
            RoomF103.Add(zone3);
            RoomF103.Add(zone4);
            RoomF103.Add(zone5);
            RoomF103.Add(zone6);
            RoomF103.Add(zone7);
            RoomF103.Add(zone8);
            RoomF103.Add(zone9);
            RoomF103.Add(zone10);
            RoomF103.Add(zone11);
            RoomF103.Add(zone12);
            RoomF103.Add(zone13);
            RoomF103.Add(zone14);
            RoomF103.Add(zone15);
            RoomF103.Add(zone16);
            RoomF103.Add(zone17);
        }

        public void FillGlassRoomList()
        {
            RestrictedSquare zone1 = new RestrictedSquare { Xposition = 0, Yposition = 5 };
            RestrictedSquare zone2 = new RestrictedSquare { Xposition = 1, Yposition = 5 };
            RestrictedSquare zone3 = new RestrictedSquare { Xposition = 2, Yposition = 5 };
            RestrictedSquare zone4 = new RestrictedSquare { Xposition = 3, Yposition = 5 };
            RestrictedSquare zone5 = new RestrictedSquare { Xposition = 4, Yposition = 5 };
            RestrictedSquare zone6 = new RestrictedSquare { Xposition = 0, Yposition = 6 };
            RestrictedSquare zone7 = new RestrictedSquare { Xposition = 1, Yposition = 6 };
            RestrictedSquare zone8 = new RestrictedSquare { Xposition = 1, Yposition = 6 };
            RestrictedSquare zone9 = new RestrictedSquare { Xposition = 3, Yposition = 6 };
            RestrictedSquare zone10 = new RestrictedSquare { Xposition = 4, Yposition = 6 };
            RestrictedSquare zone11 = new RestrictedSquare { Xposition = 5, Yposition = 6 };
            RestrictedSquare zone12 = new RestrictedSquare { Xposition = 0, Yposition = 7 };
            RestrictedSquare zone13 = new RestrictedSquare { Xposition = 1, Yposition = 7 };
            RestrictedSquare zone14 = new RestrictedSquare { Xposition = 2, Yposition = 7 };
            RestrictedSquare zone15 = new RestrictedSquare { Xposition = 3, Yposition = 7 };
            RestrictedSquare zone16 = new RestrictedSquare { Xposition = 4, Yposition = 7 };
            RestrictedSquare zone17 = new RestrictedSquare { Xposition = 0, Yposition = 8 };
            RestrictedSquare zone18 = new RestrictedSquare { Xposition = 1, Yposition = 8 };
            RestrictedSquare zone19 = new RestrictedSquare { Xposition = 2, Yposition = 8 };
            RestrictedSquare zone20 = new RestrictedSquare { Xposition = 3, Yposition = 8 };
            RestrictedSquare zone21 = new RestrictedSquare { Xposition = 4, Yposition = 8 };
            RestrictedSquare zone22 = new RestrictedSquare { Xposition = 5, Yposition = 8 };
            RestrictedSquare zone23 = new RestrictedSquare { Xposition = 0, Yposition = 9 };
            RestrictedSquare zone24 = new RestrictedSquare { Xposition = 1, Yposition = 9 };
         //   RestrictedSquare zone25 = new RestrictedSquare { Xposition = , Yposition =  };
         //   RestrictedSquare zone26 = new RestrictedSquare { Xposition = , Yposition =  };
         //   RestrictedSquare zone27 = new RestrictedSquare { Xposition = , Yposition =  };
         //   RestrictedSquare zone28 = new RestrictedSquare { Xposition = , Yposition =  };
        }

        public void FillLaboratoryList()
        {

        }

        public void FillCubicleList()
        {

        }

        public void FillAmphitheaterList()
        {

        }

        public void FillComputerLabList()
        {

        }

        public void FillFieldList()
        {

        }

        public void FillParkingLotList()
        {

        }

        public void FillProfessorsRoom()
        {

        }

        public Boolean IsMoveValid(int columns, int rows)
        {
            Boolean moveValid = false;
            int totalMoves = columns + rows;
            if(totalMoves <= DiceRoll)   
            {
                moveValid = true;
            }
            return moveValid;
        }

        public int RollDice()
        {
            Random random = new Random();
            int rollDice = random.Next(2, 13);
            DiceRoll = rollDice;
            return rollDice;
        }
    }
}