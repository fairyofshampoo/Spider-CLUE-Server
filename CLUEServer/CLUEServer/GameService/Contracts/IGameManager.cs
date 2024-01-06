using Microsoft.SqlServer.Server;
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
        void MovePawn(int columns, int rows, string gamertag, string matchCode);

        [OperationContract]
        int RollDice(string matchCode);

        [OperationContract(IsOneWay = true)]
        void ConnectGamerToGameBoard(string gamertag, string matchCode);

        [OperationContract]
        int GetNumberOfGamers(string matchCode);

        [OperationContract(IsOneWay = true)]
        void MakeFinalAccusation(List<string> cards, string matchCode, string gamertag);

        [OperationContract(IsOneWay = true)]
        void ShowCommonAccusation(string[] accusation, string matchCode, string accuser);

        [OperationContract(IsOneWay = true)]
        void ShowCard(Card card, string matchCode, string accuser);

        [OperationContract]
        List<Card> GetDeck(string gamertag);

        [OperationContract]
        void EndGame(string matchCode);
    }

    [ServiceContract]
    public interface IGameManagerCallback
    {
        [OperationContract]
        void ReceiveFinalAccusationOption(bool isEnabled);

        [OperationContract]
        void ReceiveCommonAccusationOption(bool isEnabled, Door door);

        [OperationContract]
        void ReceiveCommonAccusationByOtherGamer(string[] accusation);

        [OperationContract]
        void RequestShowCard(List<Card> cards, string accuser);

        [OperationContract]
        void ReceivePawnsMove(Pawn pawn);

        [OperationContract]
        void ReceiveTurn(bool isYourTurn);

        [OperationContract]
        void ReceiveInvalidMove();

        [OperationContract]
        void LeaveGameBoard();

        [OperationContract]
        void ShowNobodyAnswers();

        [OperationContract]
        void ReceiveCardAccused(Card card);

        [OperationContract]
        void ReceiveWinner(string winnerGamertag, string gamerIcon);
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
        public int XPosition { get { return xPosition; } set { xPosition = value; } }

        [DataMember]
        public int YPosition { get { return yPosition; } set { yPosition = value; } }

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
    public class Door
    {
        private int xPosition;
        private int yPosition;
        private string zoneName;

        [DataMember]
        public string ZoneName { get { return zoneName; } set { zoneName = value; } }

        [DataMember]
        public int Xposition { get { return xPosition; } set { xPosition = value; } }

        [DataMember]
        public int Yposition { get { return yPosition; } set { yPosition = value; } }
    }

    [DataContract]
    public class Card
    {
        private string id;
        private string type;

        [DataMember]
        public string ID { get { return id; } set { id = value; } }

        [DataMember]
        public string Type { get { return type; } set { type = value; } }
    }


}
