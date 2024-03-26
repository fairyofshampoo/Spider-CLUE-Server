using DataBaseManager;
using GameService.Contracts;
using GameService.Utilities;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;

namespace GameService.Services
{
    /// <summary>
    /// Partial class for the GameService, implementing the IFriendRequestManager interface.
    /// Manages friend request-related functionality such as retrieving friend requests,
    /// creating friend requests, responding to friend requests, and deleting friend requests.
    /// </summary>
    public partial class GameService : IFriendRequestManager
    {
        /// <summary>
        /// Retrieves friend requests for the specified gamertag.
        /// </summary>
        /// <param name="gamertag">The gamertag for which to retrieve friend requests.</param>
        /// <returns>An array of gamertags representing friend requests in "Pending" status.</returns>
        public string[] GetFriendsRequest(string gamertag)
        {
            HostBehaviorManager.ChangeToSingle();
            Utilities.LoggerManager loggerManager = new Utilities.LoggerManager(this.GetType());
            string[] friendsRequest = null;
            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    friendsRequest = databaseContext.friendRequests
                            .Where(friendRequest => friendRequest.receiverGamertag == gamertag && friendRequest.friendRequestStatus == "Pending")
                            .Select(friendRequest => friendRequest.senderGamertag)
                            .ToArray();
                }
            }
            catch (SqlException sqlException)
            {
                loggerManager.LogError(sqlException);
            }
            catch (EntityException entityException)
            {
                loggerManager.LogError(entityException);
            }

            HostBehaviorManager.ChangeToReentrant();
            return friendsRequest;
        }

        /// <summary>
        /// Creates a friend request from the specified gamertag to the target friend gamertag.
        /// </summary>
        /// <param name="gamertag">The gamertag initiating the friend request.</param>
        /// <param name="friendGamertag">The gamertag of the friend receiving the friend request.</param>
        /// <returns>An integer indicating the operation result. Constants.SuccessInOperation if successful, otherwise Constants.ErrorInOperation.</returns>
        public int CreateFriendRequest(string gamertag, string friendGamertag)
        {
            int result = Constants.ErrorInOperation;
            HostBehaviorManager.ChangeToSingle();
            Utilities.LoggerManager loggerManager = new Utilities.LoggerManager(this.GetType());
            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    var friendRequest = new DataBaseManager.friendRequest
                    {
                        senderGamertag = gamertag,
                        receiverGamertag = friendGamertag,
                        friendRequestStatus = "Pending"
                    };
                    databaseContext.friendRequests.Add(friendRequest);
                    result = databaseContext.SaveChanges();
                }
            }
            catch (SqlException sqlException)
            {
                loggerManager.LogError(sqlException);
            }
            catch (EntityException entityException)
            {
                loggerManager.LogError(entityException);
            }
            HostBehaviorManager.ChangeToReentrant();

            return result;
        }

        /// <summary>
        /// Responds to a friend request by updating its status.
        /// </summary>
        /// <param name="gamertag">The gamertag responding to the friend request.</param>
        /// <param name="friendGamertag">The gamertag of the friend who initiated the request.</param>
        /// <param name="response">The response to the friend request ("Accepted" or "Rejected").</param>
        /// <returns>An integer indicating the operation result. Constants.SuccessInOperation if successful, otherwise Constants.ErrorInOperation.</returns>
        public int ResponseFriendRequest(string gamertag, string friendGamertag, string response)
        {
            HostBehaviorManager.ChangeToSingle();
            Utilities.LoggerManager loggerManager = new Utilities.LoggerManager(this.GetType());
            int result = Constants.ErrorInOperation;
            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    var friendRequest = databaseContext.friendRequests.FirstOrDefault(friendRequestPending => friendRequestPending.senderGamertag == friendGamertag && friendRequestPending.receiverGamertag == gamertag);
                    if (friendRequest != null)
                    {
                        friendRequest.friendRequestStatus = response;
                        result = databaseContext.SaveChanges();
                    }
                }
            }
            catch (SqlException sqlException)
            {
                loggerManager.LogError(sqlException);
            }
            catch (EntityException entityException)
            {
                loggerManager.LogError(entityException);
            }
            HostBehaviorManager.ChangeToReentrant();

            return result;
        }

        /// <summary>
        /// Deletes a friend request between two gamertags.
        /// </summary>
        /// <param name="gamertag">The gamertag initiating the friend request.</param>
        /// <param name="friendGamertag">The gamertag of the friend receiving the friend request.</param>
        /// <returns>An integer indicating the operation result. Constants.SuccessInOperation if successful, otherwise Constants.ErrorInOperation.</returns>
        public int DeleteFriendRequest(string gamertag, string friendGamertag)
        {
            HostBehaviorManager.ChangeToSingle();
            Utilities.LoggerManager loggerManager = new Utilities.LoggerManager(this.GetType());
            int result = Constants.ErrorInOperation;
            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    var friendRequest = databaseContext.friendRequests
                            .Where(friend => friend.senderGamertag == gamertag && friend.receiverGamertag == friendGamertag);

                    var secondfriendRequest = databaseContext.friendRequests
                        .Where(second => second.senderGamertag == friendGamertag && second.receiverGamertag == gamertag);

                    if (friendRequest.Any() || secondfriendRequest.Any())
                    {
                        if (secondfriendRequest.Any())
                        {
                            databaseContext.friendRequests.RemoveRange(secondfriendRequest);
                            result = Constants.SuccessInOperation;
                        }

                        if (friendRequest.Any())
                        {
                            databaseContext.friendRequests.RemoveRange(friendRequest);
                            result = Constants.SuccessInOperation;
                        }
                    }
                }
            }
            catch (SqlException sqlException)
            {
                loggerManager.LogError(sqlException);
            }
            catch (EntityException entityException)
            {
                loggerManager.LogError(entityException);
            }
            
            HostBehaviorManager.ChangeToReentrant();

            return result;
        }
    }
}

 