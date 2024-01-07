using GameService.Contracts;
using GameService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace GameService.Services
{
    public partial class GameService : ISessionManager
    {
        public int Connect(string gamertag)
        {
            HostBehaviorManager.ChangeToSingle();
            int result = Constants.ERROR_IN_OPERATION;

            if (!UsersConnected.Contains(gamertag))
            {
                UsersConnected.Add(gamertag);
                UpdateConnectedFriends(gamertag);
                result = Constants.SUCCESS_IN_OPERATION;
            } else
            {
                result = Constants.ERROR_IN_OPERATION;
            }

            return result;
        }


        public void Disconnect(string gamertag)
        {
            HostBehaviorManager.ChangeToSingle();
            RemoveFromUsersConnected(gamertag);
        }

        public bool IsGamerAlreadyOnline(string gamertag)
        {
            HostBehaviorManager.ChangeToSingle();
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
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            List<string> connectedFriends = SetConnectedFriendsList(gamertag);

            foreach (var connectedFriend in connectedFriends
                .Where(friend => gamersFriendsManagerCallback.ContainsKey(friend)))
            {
                try
                {
                    gamersFriendsManagerCallback[connectedFriend].ReceiveConnectedFriends(SetConnectedFriendsList(connectedFriend));
                }
                catch (CommunicationException communicationException)
                {
                    loggerManager.LogError(communicationException);
                }
                catch (TimeoutException timeoutException)
                {
                    loggerManager.LogError(timeoutException);
                }
            }
        }

    }
}
