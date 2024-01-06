using DataBaseManager;
using GameService.Contracts;
using GameService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Services
{
    public partial class GameService : IFriendRequestManager
    {
        public string[] GetFriendsRequest(string gamertag)
        {
            using (var databaseConext = new SpiderClueDbEntities())
            {
                HostBehaviorManager.ChangeToSingle();
                var friendsRequest = databaseConext.friendRequests
                    .Where(friendrequest => friendrequest.receiverGamertag == gamertag && friendrequest.friendRequestStatus == "Pending")
                    .Select(friendRequest => friendRequest.senderGamertag )
                    .ToArray();
                HostBehaviorManager.ChangeToReentrant();
                return friendsRequest;
            }
        }   

        public void CreateFriendRequest(string gamertag, string friendGamertag)
        {
            HostBehaviorManager.ChangeToSingle();
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
            HostBehaviorManager.ChangeToReentrant();
        }

        public void ResponseFriendRequest(string gamertag, string friendGamertag, string response)
        {
            HostBehaviorManager.ChangeToSingle();
            using (var databaseContext = new SpiderClueDbEntities())
            {
                var friendRequest = databaseContext.friendRequests.FirstOrDefault(friendRequestPending => friendRequestPending.senderGamertag == friendGamertag && friendRequestPending.receiverGamertag == gamertag);
                if (friendRequest != null)
                {
                    friendRequest.friendRequestStatus = response;
                    databaseContext.SaveChanges();
                }
            }
            HostBehaviorManager.ChangeToReentrant();
        }

        public void DeleteFriendRequest(string gamertag, string friendGamertag)
        {
            HostBehaviorManager.ChangeToSingle();
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

                    if(friendRequest.Any())
                    {
                        databaseContext.friendRequests.RemoveRange(friendRequest);
                        databaseContext.SaveChanges();
                    }
                }
            }
            HostBehaviorManager.ChangeToReentrant();
        }
    }
}

 