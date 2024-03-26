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
        /// <summary>
        /// Connects a gamer to the game service, updating the list of connected users and notifying friends.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer to connect.</param>
        /// <returns>The operation result code: Constants.SuccessInOperation or Constants.ErrorInOperation.</returns>
        public int Connect(string gamertag)
        {
            HostBehaviorManager.ChangeToSingle();
            int result = Constants.ErrorInOperation;

            if (!UsersConnected.Contains(gamertag))
            {
                UsersConnected.Add(gamertag);
                UpdateConnectedFriends(gamertag);
                result = Constants.SuccessInOperation;
            }

            return result;
        }

        /// <summary>
        /// Disconnects a gamer from the game service, updating the list of connected users and notifying friends.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer to disconnect.</param>
        public void Disconnect(string gamertag)
        {
            HostBehaviorManager.ChangeToSingle();
            RemoveFromUsersConnected(gamertag);
        }

        /// <summary>
        /// Checks if a gamer is already connected to the game service.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer to check.</param>
        /// <returns>True if the gamer is already online, false otherwise.</returns>
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
