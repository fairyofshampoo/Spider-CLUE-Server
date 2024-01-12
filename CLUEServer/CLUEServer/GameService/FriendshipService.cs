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
        /// <summary>
        /// Retrieves the friend list for the specified gamertag.
        /// </summary>
        /// <param name="gamertag">The gamertag for which the friend list is requested.</param>
        /// <returns>
        /// A list of gamertags representing the friends of the specified gamertag.
        /// </returns>
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

        /// <summary>
        /// Deletes the friendship connection between the specified gamertag and friendGamertag.
        /// </summary>
        /// <param name="gamertag">The gamertag initiating the friend removal.</param>
        /// <param name="friendGamertag">The gamertag of the friend to be removed.</param>
        /// <returns>
        /// An integer indicating the result of the operation. 
        /// It returns Constants.SuccessInOperation if successful, otherwise Constants.ErrorInOperation.
        /// </returns>
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

        /// <summary>
        /// Attempts to add a friend connection between two gamers in the database.
        /// If the friendship doesn't already exist, it is added bidirectionally.
        /// Returns an integer code indicating the success or failure of the operation.
        /// </summary>
        /// <param name="gamertag">The gamertag of the user initiating the friend request.</param>
        /// <param name="friendGamertag">The gamertag of the user receiving the friend request.</param>
        /// <returns>An integer code: Constants.SuccessInOperation if successful, Constants.ErrorInOperation otherwise.</returns>
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

        /// <summary>
        /// Checks if two gamers are not friends based on their gamertags.
        /// Returns true if they are not friends, false otherwise.
        /// </summary>
        /// <param name="gamertag">The gamertag of the first gamer.</param>
        /// <param name="friendGamertag">The gamertag of the second gamer.</param>
        /// <returns>True if the gamers are not friends, false if they are friends or an error occurs.</returns>
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

        /// <summary>
        /// Checks if there is no pending friend request between two gamers based on their gamertags.
        /// Returns true if there is no pending friend request, false otherwise.
        /// </summary>
        /// <param name="gamertag">The gamertag of the potential sender of the friend request.</param>
        /// <param name="friendGamertag">The gamertag of the potential receiver of the friend request.</param>
        /// <returns>True if there is no pending friend request, false if there is a request or an error occurs.</returns>
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
