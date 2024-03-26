using System.ServiceModel;

namespace GameService.Contracts
{
    /// <summary>
    /// Service contract for managing a lobby in a gaming system.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(ILobbyManagerCallback))]
    public interface ILobbyManager
    {
        /// <summary>
        /// Connects a player with the specified gamertag to the lobby.
        /// </summary>
        /// <param name="gamertag">The gamertag of the player connecting to the lobby.</param>
        [OperationContract(IsOneWay = true)]
        void ConnectToLobby(string gamertag);

        /// <summary>
        /// Kicks a player with the specified gamertag from the lobby.
        /// </summary>
        /// <param name="gamertag">The gamertag of the player to be kicked.</param>
        [OperationContract(IsOneWay = true)]
        void KickPlayer(string gamertag);

        /// <summary>
        /// Initiates the start of a match with the specified match code.
        /// </summary>
        /// <param name="matchCode">The code identifying the match to begin.</param>
        [OperationContract(IsOneWay = true)]
        void BeginMatch(string matchCode);

        /// <summary>
        /// Checks if the player with the specified gamertag is the owner of the match with the given match code.
        /// </summary>
        /// <param name="gamertag">The gamertag of the player.</param>
        /// <param name="matchCode">The code identifying the match.</param>
        /// <returns>True if the player is the owner of the match; otherwise, false.</returns>
        [OperationContract]
        bool IsOwnerOfTheMatch(string gamertag, string matchCode);
    }

    /// <summary>
    /// Callback contract for the lobby manager to communicate with lobby clients.
    /// </summary>
    [ServiceContract]
    public interface ILobbyManagerCallback
    {
        /// <summary>
        /// Notifies a lobby client that a player with the specified gamertag has been kicked from the match.
        /// </summary>
        /// <param name="gamertag">The gamertag of the kicked player.</param>
        [OperationContract]
        void KickPlayerFromMatch(string gamertag);

        /// <summary>
        /// Notifies a lobby client to start the game.
        /// </summary>
        [OperationContract]
        void StartGame();
    }
}
