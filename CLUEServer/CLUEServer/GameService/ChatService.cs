using GameService.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Services
{
    
    public partial class GameService : IChatManager
    {
        private static readonly Dictionary<string, IChatManagerCallback> chatCallbacks = new Dictionary<string, IChatManagerCallback>();
        private static readonly Dictionary<String, List<Message>> messagesforMatch = new Dictionary<String, List<Message>>();
        public void ConnectToChat(string gamertag, String matchCode)
        {
            chatCallbacks.Add(gamertag, OperationContext.Current.GetCallbackChannel<IChatManagerCallback>());
            DisplayMessages(matchCode);
        }

        private void DisplayMessages(String matchCode)
        {
            foreach(var gamer in gamersInMatch)
            {
                string gamertag = gamer.Key;
                if (chatCallbacks.ContainsKey(gamertag))
                {
                    chatCallbacks[gamertag].ReceiveMessages(RetrieveMessagesForMatch(matchCode));
                }
            }
        }

        private List<Message> RetrieveMessagesForMatch(String matchCode)
        {
            if (!messagesforMatch.ContainsKey(matchCode))
            {
                List<Message> messages = new List<Message>();
                messagesforMatch.Add(matchCode, messages);
            }
            return messagesforMatch[matchCode];
        }

        public void DisconnectFromChat(string gamertag)
        {
            lock(chatCallbacks)
            {
                if (chatCallbacks.ContainsKey(gamertag))
                {
                    chatCallbacks.Remove(gamertag);
                }
            }
        }

        public void BroadcastMessage(String matchCode, Message message)
        {
            messagesforMatch[matchCode].Add(message);
            DisplayMessages(matchCode);
        }
    }
}
