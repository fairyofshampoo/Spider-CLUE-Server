using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace GameService.Contracts
{
    /// <summary>
    /// Service contract for managing game-related operations.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IGameManagerCallback))]
    public interface IGameManager
    {
        /// <summary>
        /// Notifies the server of a pawn movement on the game board.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void MovePawn(int column, int row, string gamertag, string matchCode);

        /// <summary>
        /// Rolls the dice for the specified match.
        /// </summary>
        [OperationContract]
        int RollDice(string matchCode);

        /// <summary>
        /// Connects a gamer to the game board for the specified match.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void ConnectGamerToGameBoard(string gamertag, string matchCode);

        /// <summary>
        /// Initiates the final accusation process in the game.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void MakeFinalAccusation(List<string> cards, string matchCode, string gamertag);

        /// <summary>
        /// Displays a common accusation to all gamers in the game.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void ShowCommonAccusation(string[] accusation, string matchCode, string accuser);

        /// <summary>
        /// Displays a card to a specific gamer in the game.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void ShowCard(Card card, string matchCode, string accuser);

        /// <summary>
        /// Retrieves the deck of cards for the specified gamer.
        /// </summary>
        [OperationContract]
        List<Card> GetDeck(string gamertag);

        /// <summary>
        /// Ends the game for the specified match.
        /// </summary>
        [OperationContract]
        void EndGame(string matchCode);
    }

    /// <summary>
    /// Callback contract for receiving notifications from the game manager.
    /// </summary>
    [ServiceContract]
    public interface IGameManagerCallback
    {
        /// <summary>
        /// Notifies the client about the availability of the final accusation option.
        /// </summary>
        [OperationContract]
        void ReceiveFinalAccusationOption(bool isEnabled);

        /// <summary>
        /// Notifies the client about the availability of a common accusation option along with the associated door information.
        /// </summary>
        [OperationContract]
        void ReceiveCommonAccusationOption(bool isEnabled, Door door);

        /// <summary>
        /// Notifies the client about a common accusation made by another gamer.
        /// </summary>
        [OperationContract]
        void ReceiveCommonAccusationByOtherGamer(string[] accusation);

        /// <summary>
        /// Requests the client to show a card from the provided list along with the accuser's information.
        /// </summary>
        [OperationContract]
        void RequestShowCard(List<Card> cards, string accuser);

        /// <summary>
        /// Notifies the client about the movement of pawns on the game board.
        /// </summary>
        [OperationContract]
        void ReceivePawnsMove(Pawn pawn);

        /// <summary>
        /// Notifies the client about the turn status, indicating whether it's their turn or not.
        /// </summary>
        [OperationContract]
        void ReceiveTurn(bool isYourTurn);

        /// <summary>
        /// Notifies the client about an invalid move.
        /// </summary>
        [OperationContract]
        void ReceiveInvalidMove();

        /// <summary>
        /// Notifies the client about leaving the game board.
        /// </summary>
        [OperationContract]
        void LeaveGameBoard();

        /// <summary>
        /// Notifies the client that nobody answered a common accusation.
        /// </summary>
        [OperationContract]
        void ShowNobodyAnswers();

        /// <summary>
        /// Notifies the client about a card being accused along with the card information.
        /// </summary>
        [OperationContract]
        void ReceiveCardAccused(Card card);

        /// <summary>
        /// Notifies the client about the winner of the game along with their gamertag and icon.
        /// </summary>
        [OperationContract]
        void ReceiveWinner(string winnerGamertag, string gamerIcon);
    }

    /// <summary>
    /// Represents a pawn on the game board.
    /// </summary>
    [DataContract]
    public class Pawn
    {
        /// <summary>
        /// Gets or sets the color of the pawn.
        /// </summary>
        [DataMember]
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the X position of the pawn on the game board.
        /// </summary>
        [DataMember]
        public int XPosition { get; set; }

        /// <summary>
        /// Gets or sets the Y position of the pawn on the game board.
        /// </summary>
        [DataMember]
        public int YPosition { get; set; }
    }

    /// <summary>
    /// Represents a node on the game board grid.
    /// </summary>
    [DataContract]
    public class GridNode
    {
        /// <summary>
        /// Gets or sets the X position of the grid node.
        /// </summary>
        [DataMember]
        public int Xposition { get; set; }

        /// <summary>
        /// Gets or sets the Y position of the grid node.
        /// </summary>
        [DataMember]
        public int Yposition { get; set; }
    }

    /// <summary>
    /// Represents a door on the game board.
    /// </summary>
    [DataContract]
    public class Door
    {
        /// <summary>
        /// Gets or sets the name of the zone associated with the door.
        /// </summary>
        [DataMember]
        public string ZoneName { get; set; }

        /// <summary>
        /// Gets or sets the X position of the door on the game board.
        /// </summary>
        [DataMember]
        public int Xposition { get; set; }

        /// <summary>
        /// Gets or sets the Y position of the door on the game board.
        /// </summary>
        [DataMember]
        public int Yposition { get; set; }
    }

    /// <summary>
    /// Represents a card in the game.
    /// </summary>
    [DataContract]
    public class Card
    {
        /// <summary>
        /// Gets or sets the unique ID of the card.
        /// </summary>
        [DataMember]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the type of the card.
        /// </summary>
        [DataMember]
        public string Type { get; set; }
    }
}
