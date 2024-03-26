using System.Runtime.Serialization;
using System.ServiceModel;

namespace GameService.Contracts
{
    /// <summary>
    /// Service contract for managing friend requests between gamers.
    /// </summary>
    [ServiceContract]
    interface IFriendRequestManager
    {
        /// <summary>
        /// Retrieves pending friend requests for the specified gamertag.
        /// </summary>
        [OperationContract]
        string[] GetFriendsRequest(string gamertag);

        /// <summary>
        /// Creates a friend request from one gamertag to another.
        /// </summary>
        [OperationContract]
        int CreateFriendRequest(string gamertag, string friendGamertag);

        /// <summary>
        /// Responds to a friend request with the specified response (Accept/Decline).
        /// </summary>
        [OperationContract]
        int ResponseFriendRequest(string gamertag, string friendGamertag, string response);

        /// <summary>
        /// Deletes a friend request between the specified gamertag and friend.
        /// </summary>
        [OperationContract]
        int DeleteFriendRequest(string gamertag, string friend);
    }

    /// <summary>
    /// Represents a friend request between two gamers.
    /// </summary>
    [DataContract]
    public class FriendRequest
    {
        /// <summary>
        /// Gets or sets the gamertag of the friend request sender.
        /// </summary>
        [DataMember]
        public string SenderGamertag { get; set; }

        /// <summary>
        /// Gets or sets the gamertag of the friend request receiver.
        /// </summary>
        [DataMember]
        public string ReceiverGamertag { get; set; }

        /// <summary>
        /// Gets or sets the status of the friend request (Pending/Accepted/Declined).
        /// </summary>
        [DataMember]
        public string FriendRequestStatus { get; set; }
    }
}
