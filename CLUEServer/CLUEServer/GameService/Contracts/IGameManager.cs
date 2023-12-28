using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Contracts
{
    [ServiceContract(CallbackContract = typeof(IGameManagerCallback))]
    public interface IGameManager
    {
        [OperationContract(IsOneWay = true)]
        void MovePawn(int columns, int rows, string gamertag);

        [OperationContract]
        int RollDice();

        [OperationContract(IsOneWay = true)]
        void ConnectGamerToGameBoard(string gamertag, string matchCode);

        [OperationContract]
        void DisconnectFromBoard(string gamertag, string matchCode);

        [OperationContract(IsOneWay = true)]
        void GetGamersInGameboard(string gamertag, string code);
    }

    [ServiceContract]
    public interface IGameManagerCallback
    {
        [OperationContract]
        void ReceivePawnsMove(Pawn pawn);

        [OperationContract]
        void ReceiveTurn(bool isYourTurn);

        [OperationContract]
        void UpdateNumberOfPlayersInGameboard(int numberOfPlayers);
    }

    [DataContract]
    public class Pawn
    {
        private string color;
        private int xPosition;
        private int yPosition;

        [DataMember]
        public string Color { get { return color; } set { color = value; } }

        [DataMember]
        public int XPosition { get {  return xPosition; } set {  xPosition = value; } }

        [DataMember]
        public int YPosition { get { return yPosition; } set {  yPosition = value; } }

    }

    [DataContract]
    public class GridNode
    {
        private int xPosition;
        private int yPosition;

        [DataMember]
        public int Xposition { get { return xPosition; } set { xPosition = value; } }

        [DataMember]
        public int Yposition { get { return yPosition; } set { yPosition = value; } }
    }

    [DataContract]
    public class Door {
        private int xPosition;
        private int yPosition;
        private string zoneName;

        [DataMember]
        public string ZoneName { get {  return zoneName; } set {  zoneName = value; } }

        [DataMember]
        public int Xposition { get { return xPosition; } set { xPosition = value; } }

        [DataMember]
        public int Yposition { get { return yPosition; } set { yPosition = value; } }
    }
}
