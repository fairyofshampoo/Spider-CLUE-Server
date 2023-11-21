using DataBaseManager;
using GameService.Contracts;
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
        public string[] GetFriendsRequets(string gamertag)
        {
            using(var databaseConext = new SpiderClueDbEntities())
            {
                var friendsRequest = databaseConext.friendRequests
                    .Where(friendrequest => friendrequest.receiverGamertag == gamertag && friendrequest.friendRequestStatus == "Pending")
                    .Select(friendRequest => friendRequest.senderGamertag )
                    .ToArray();
                return friendsRequest;
            }
        }   

        public void CreateFriendRequest(string gamertag, string friendGamertag)
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

        public void ResponseFriendRequest(string gamertag, string friendGamertag, string response)
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
    }
}

 