using System.Collections.Generic;
using System.ServiceModel;

namespace GameService.Contracts
{
    /// <summary>
    /// Service contract for managing friends and their connections.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IFriendsManagerCallback))]
    public interface IFriendsManager
    {
        /// <summary>
        /// Notifies the server to retrieve the list of connected friends for a given gamertag.
        /// </summary>
        /// <param name="gamertag">The gamertag for which to retrieve the connected friends.</param>
        [OperationContract(IsOneWay = true)]
        void GetConnectedFriends(string gamertag);

        /// <summary>
        /// Notifies the server that a user is joining the connected friends group.
        /// </summary>
        /// <param name="gamertag">The gamertag of the user joining the connected friends group.</param>
        [OperationContract(IsOneWay = true)]
        void JoinFriendsConnected(string gamertag);
    }

    /// <summary>
    /// Callback contract for receiving notifications related to connected friends.
    /// </summary>
    [ServiceContract]
    public interface IFriendsManagerCallback
    {
        /// <summary>
        /// Receives the list of connected friends.
        /// </summary>
        /// <param name="connectedFriends">The list of connected friends' gamertags.</param>
        [OperationContract]
        void ReceiveConnectedFriends(List<string> connectedFriends);
    }
}
