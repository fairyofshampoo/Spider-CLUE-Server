using DataBaseManager;
using GameService.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

    namespace GameService.Services
    {
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
        public partial class GameService : IFriendsManager
        {
            private static List<string> usersConnected = new List<string>();

            public void Connect(string gamertag)
            {
                usersConnected.Add(gamertag);
            }

            public void Disconnect(string gamertag)
            {
                if (usersConnected.Contains(gamertag))
                {
                    usersConnected.Remove(gamertag);
                }
            }

            public void GetConnectedFriends(string gamertag)
            {
                List<string> friendList = GetFriendList(gamertag);
                List<string> connectedFriends = new List<string>();
                foreach (string friend in friendList)
                {
                    if (usersConnected.Contains(friend))
                    {
                        connectedFriends.Add(friend);
                    }
                }
                OperationContext.Current.GetCallbackChannel<IFriendsManagerCallback>().ReceiveConnectedFriends(connectedFriends);
            }

        public List<string> GetFriendList(string gamertag)
        {
            using (var databaseContext = new SpiderClueDbEntities())
            {
                var friendList = databaseContext.friendLists.Where(user => user.gamertag == gamertag)
                    .Select(user => user.friend).ToList();
                return friendList;
            }
        }
        }
    }