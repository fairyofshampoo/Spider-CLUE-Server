using GameService.Contracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace GameService.Services
{
    public partial class GameService : ISessionManager
    {
        public void Connect(string gamertag)
        {
            if (!UsersConnected.Contains(gamertag))
            {
                UsersConnected.Add(gamertag);
                UpdateConnectedFriends(gamertag);
            }
        }

        public bool IsGamerAlreadyOnline(string gamertag)
        {
            return UsersConnected.Contains(gamertag); ;
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
                KickPlayer(gamertag);
            }
        }
    }
}
