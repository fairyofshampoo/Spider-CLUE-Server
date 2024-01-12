using DataBaseManager;
using GameService.Contracts;
using GameService.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Services
{
    public partial class GameService : IFriendshipManager
    {
        public List<string> GetFriendList(string gamertag)
        {
            Utilities.LoggerManager loggerManager = new Utilities.LoggerManager(this.GetType());
            List<string> friendList = new List<string>();
            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    HostBehaviorManager.ChangeToSingle();
                    friendList = databaseContext.friendLists.Where(user => user.gamertag == gamertag)
                        .Select(user => user.friend).ToList();


                    HostBehaviorManager.ChangeToReentrant();
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

            return friendList;
        }

        public int DeleteFriend(string gamertag, string friendGamertag)
        {
            int result = Constants.ErrorInOperation;

            Utilities.LoggerManager loggerManager = new Utilities.LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    var friendEliminated = databaseContext.friendLists
                            .Where(friend => friend.gamertag == gamertag && friend.friend == friendGamertag);

                    if (friendEliminated.Any())
                    {
                        databaseContext.friendLists.RemoveRange(friendEliminated);
                        result = databaseContext.SaveChanges();
                        result = result + DeleteFriend(friendGamertag, gamertag);
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

        public int AddFriend(string gamertag, string friendGamertag)
        {
            int result = Constants.ErrorInOperation;
            HostBehaviorManager.ChangeToSingle();
            Utilities.LoggerManager loggerManager = new Utilities.LoggerManager(this.GetType());

            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    var existingFriendship = databaseContext.friendLists
                            .FirstOrDefault(f => f.gamertag == gamertag && f.friend == friendGamertag);

                    if (existingFriendship == null)
                    {
                        var newFriends = new DataBaseManager.friendList
                        {
                            gamertag = gamertag,
                            friend = friendGamertag
                        };
                        databaseContext.friendLists.Add(newFriends);
                        result = databaseContext.SaveChanges();
                        result = result + AddFriend(friendGamertag, gamertag);

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

        public bool AreNotFriends(string gamertag, string friendGamertag)
        {
            Utilities.LoggerManager loggerManager = new Utilities.LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            bool areNotFriends = true;
            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    var existingFriendship = databaseContext.friendLists
                        .FirstOrDefault(friends => friends.gamertag == gamertag && friends.friend == friendGamertag);
                    if (existingFriendship != null)
                    {
                        areNotFriends = false;
                    }
                }
            }
            catch (SqlException sqlException)
            {
                loggerManager.LogError(sqlException);
                areNotFriends = true;
            }
            catch (EntityException entityException)
            {
                loggerManager.LogError(entityException);
                areNotFriends = true;
            }
            HostBehaviorManager.ChangeToReentrant();
            return areNotFriends;
        }

        public bool ThereIsNoFriendRequest(string gamertag, string friendGamertag)
        {
            Utilities.LoggerManager loggerManager = new Utilities.LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            bool thereIsNotFriendRequest = true;

            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    var existingFriendRequest = databaseContext.friendRequests
                            .FirstOrDefault(friendRequest => friendRequest.senderGamertag == gamertag && friendRequest.receiverGamertag == friendGamertag);
                    if (existingFriendRequest != null)
                    {
                        thereIsNotFriendRequest = false;
                    }
                }
            }
            catch (SqlException sqlException)
            {
                loggerManager.LogError(sqlException);
                thereIsNotFriendRequest = true;
            }
            catch (EntityException entityException)
            {
                loggerManager.LogError(entityException);
                thereIsNotFriendRequest = true;
            }
            HostBehaviorManager.ChangeToReentrant();
            return thereIsNotFriendRequest;
        }
    }
}
