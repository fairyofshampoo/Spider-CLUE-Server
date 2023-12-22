using GameService.Contracts;
using System.Collections.Generic;
using System.ServiceModel;

namespace GameService.Services
{
    public partial class GameService : ISessionManager
    {
        public void Connect(string gamertag)
        {
            UsersConnected.Add(gamertag);
        }

        public void Disconnect(string gamertag)
        {
            RemoveFromUsersConnected(gamertag);
            RemoveFromMatch(gamertag);
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
            foreach (var connectedFriend in connectedFriends)
            {
                if (gamersFriendsManagerCallback.ContainsKey(connectedFriend))
                {
                    gamersFriendsManagerCallback[connectedFriend].ReceiveConnectedFriends(SetConnectedFriendsList(connectedFriend));
                }
            }
        }

        private void RemoveFromMatch(string gamertag)
        {
            if (gamersInMatch.ContainsKey(gamertag))
            {
                string matchCode = gamersInMatch[gamertag];
                LeaveMatch(gamertag, matchCode);
            }
        }
    }
}
