using System.Collections.Generic;
using System.ServiceModel;

namespace GameService.Contracts
{
    /// <summary>
    /// Service contract for managing friendships and friend requests.
    /// </summary>
    [ServiceContract]
    public interface IFriendshipManager
    {
        /// <summary>
        /// Retrieves the friend list for a given gamertag.
        /// </summary>
        /// <param name="gamertag">The gamertag for which to retrieve the friend list.</param>
        /// <returns>A list of gamertags representing friends.</returns>
        [OperationContract]
        List<string> GetFriendList(string gamertag);

        /// <summary>
        /// Deletes a friend from the friend list.
        /// </summary>
        /// <param name="gamertag">The gamertag of the user initiating the request.</param>
        /// <param name="friendGamertag">The gamertag of the friend to be removed.</param>
        /// <returns>An integer result code indicating the operation's success or failure.</returns>
        [OperationContract]
        int DeleteFriend(string gamertag, string friendGamertag);

        /// <summary>
        /// Adds a friend to the friend list.
        /// </summary>
        /// <param name="gamertag">The gamertag of the user initiating the request.</param>
        /// <param name="friendGamertag">The gamertag of the friend to be added.</param>
        /// <returns>An integer result code indicating the operation's success or failure.</returns>
        [OperationContract]
        int AddFriend(string gamertag, string friendGamertag);

        /// <summary>
        /// Checks if two gamertags are not friends.
        /// </summary>
        /// <param name="gamertag">The gamertag of the first user.</param>
        /// <param name="friendGamertag">The gamertag of the second user.</param>
        /// <returns>True if the users are not friends; otherwise, false.</returns>
        [OperationContract]
        bool AreNotFriends(string gamertag, string friendGamertag);

        /// <summary>
        /// Checks if there is no friend request between two users.
        /// </summary>
        /// <param name="gamertag">The gamertag of the first user.</param>
        /// <param name="friendGamertag">The gamertag of the second user.</param>
        /// <returns>True if there is no friend request; otherwise, false.</returns>
        [OperationContract]
        bool ThereIsNoFriendRequest(string gamertag, string friendGamertag);
    }
}