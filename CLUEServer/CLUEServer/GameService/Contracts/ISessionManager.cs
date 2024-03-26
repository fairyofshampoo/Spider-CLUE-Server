using System.ServiceModel;

namespace GameService.Contracts
{
    /// <summary>
    /// Service contract for managing user sessions.
    /// </summary>
    [ServiceContract]
    public interface ISessionManager
    {
        /// <summary>
        /// Connects a gamer to the session.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer to connect.</param>
        /// <returns>An integer representing the connection status.</returns>
        [OperationContract]
        int Connect(string gamertag);

        /// <summary>
        /// Disconnects a gamer from the session.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer to disconnect.</param>
        [OperationContract]
        void Disconnect(string gamertag);

        /// <summary>
        /// Checks if a gamer is already online in the session.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer to check.</param>
        /// <returns>True if the gamer is already online; otherwise, false.</returns>
        [OperationContract]
        bool IsGamerAlreadyOnline(string gamertag);
    }
}