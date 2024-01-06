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
        void MovePawn(int column, int row, string gamertag, string matchCode);

        [OperationContract]
        int RollDice(string matchCode);

        [OperationContract(IsOneWay = true)]
        void ConnectGamerToGameBoard(string gamertag, string matchCode);

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
        [DataMember]
        public string Color { get; set; }

        [DataMember]
        public int XPosition { get; set; }

        [DataMember]
        public int YPosition { get; set; }

    }

    [DataContract]
    public class GridNode
    {
        [DataMember]
        public int Xposition { get; set; }

        [DataMember]
        public int Yposition { get; set; }
    }

    [DataContract]
    public class Door
    {
        [DataMember]
        public string ZoneName { get; set; }

        [DataMember]
        public int Xposition { get; set; }

        [DataMember]
        public int Yposition { get; set; }
    }

    [DataContract]
    public class Card
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Type { get; set; }
    }


}
