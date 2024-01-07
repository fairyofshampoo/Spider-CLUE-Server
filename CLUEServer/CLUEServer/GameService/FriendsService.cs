using DataBaseManager;
using GameService.Contracts;
using GameService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace GameService.Services
{
    public partial class GameService : IFriendsManager
    {
        private static readonly Dictionary<string, IFriendsManagerCallback> gamersFriendsManagerCallback = new Dictionary<string, IFriendsManagerCallback>();

        private static readonly List<string> UsersConnected = new List<string>();

        public void GetConnectedFriends(string gamertag)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToReentrant();
            List<string> connectedFriends = SetConnectedFriendsList(gamertag);

            try
            {
                OperationContext.Current.GetCallbackChannel<IFriendsManagerCallback>().ReceiveConnectedFriends(connectedFriends);
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

        public void JoinFriendsConnected(string gamertag)
        {
            HostBehaviorManager.ChangeToReentrant();
            IFriendsManagerCallback callback = OperationContext.Current.GetCallbackChannel<IFriendsManagerCallback>();
            if (!gamersFriendsManagerCallback.ContainsKey(gamertag))
            {
                gamersFriendsManagerCallback.Add(gamertag, callback);
            }
        }

        private List<string> SetConnectedFriendsList(string gamertag)
        {
            List<string> friendList = GetFriendList(gamertag);

            return friendList
                .Where(friend => UsersConnected.Contains(friend))
                .ToList();
        }

    }
}