using DataBaseManager;
using GameService.Contracts;
using GameService.Utilities;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;

namespace GameService.Services
{
    public partial class GameService : IFriendRequestManager
    {
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

        public void CreateFriendRequest(string gamertag, string friendGamertag)
        {
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
                    databaseContext.SaveChanges();
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
        }

        public void ResponseFriendRequest(string gamertag, string friendGamertag, string response)
        {
            HostBehaviorManager.ChangeToSingle();
            Utilities.LoggerManager loggerManager = new Utilities.LoggerManager(this.GetType());

            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    var friendRequest = databaseContext.friendRequests.FirstOrDefault(friendRequestPending => friendRequestPending.senderGamertag == friendGamertag && friendRequestPending.receiverGamertag == gamertag);
                    if (friendRequest != null)
                    {
                        friendRequest.friendRequestStatus = response;
                        databaseContext.SaveChanges();
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
        }

        public void DeleteFriendRequest(string gamertag, string friendGamertag)
        {
            HostBehaviorManager.ChangeToSingle();
            Utilities.LoggerManager loggerManager = new Utilities.LoggerManager(this.GetType());
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
                            databaseContext.SaveChanges();
                        }

                        if (friendRequest.Any())
                        {
                            databaseContext.friendRequests.RemoveRange(friendRequest);
                            databaseContext.SaveChanges();
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
        }
    }
}

 