using GameService.Contracts;
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
            RemoveFromChatCallbacks(gamertag);
        }

        private void RemoveFromUsersConnected(string gamertag)
        {
            if (UsersConnected.Contains(gamertag))
            {
                UsersConnected.Remove(gamertag);
            }
        }

        private void RemoveFromChatCallbacks(string gamertag)
        {
            if (chatCallbacks.ContainsKey(gamertag))
            {
                chatCallbacks.Remove(gamertag);
            }
        }
    }
}
