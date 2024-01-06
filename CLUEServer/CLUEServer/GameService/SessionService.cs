using GameService.Contracts;
using GameService.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace GameService.Services
{
    public partial class GameService : ISessionManager
    {
        public void Connect(string gamertag)
        {
            HostBehaviorManager.ChangeToReentrant();
            if (!UsersConnected.Contains(gamertag))
            {
                UsersConnected.Add(gamertag);
                UpdateConnectedFriends(gamertag);
            } 
        }

        public void Disconnect(string gamertag)
        {
            HostBehaviorManager.ChangeToReentrant();
            RemoveFromUsersConnected(gamertag);
        }

        public bool IsGamerAlreadyOnline(string gamertag)
        {
            return UsersConnected.Contains(gamertag);
        }

        private void RemoveFromUsersConnected(string gamertag)
        {
            if (UsersConnected.Contains(gamertag))
            {
                UsersConnected.Remove(gamertag);
                UpdateConnectedFriends(gamertag);
            }
        }

        private void UpdateConnectedFriends(string gamertag)
        {
            List<string> connectedFriends = SetConnectedFriendsList(gamertag);

            foreach (var connectedFriend in connectedFriends
                .Where(friend => gamersFriendsManagerCallback.ContainsKey(friend)))
            {
                gamersFriendsManagerCallback[connectedFriend].ReceiveConnectedFriends(SetConnectedFriendsList(connectedFriend));
            }
        }

    }
}
