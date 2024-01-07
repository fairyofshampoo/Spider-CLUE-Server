using GameService.Contracts;
using GameService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Services
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class GameService : IChatManager
    {
        private static readonly Dictionary<string, IChatManagerCallback> chatCallbacks = new Dictionary<string, IChatManagerCallback>();
        private static readonly Dictionary<String, List<Message>> messagesforMatch = new Dictionary<String, List<Message>>();
        public void ConnectToChat(string gamertag, String matchCode)
        {
            HostBehaviorManager.ChangeToReentrant();
            chatCallbacks.Add(gamertag, OperationContext.Current.GetCallbackChannel<IChatManagerCallback>());
            DisplayMessages(matchCode);
        }

        private void DisplayMessages(String matchCode)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            foreach (var gamertag in gamersInMatch.Select(gamer => gamer.Key))
            {
                if (chatCallbacks.ContainsKey(gamertag))
                {
                    try
                    {
                        chatCallbacks[gamertag].ReceiveMessages(RetrieveMessagesForMatch(matchCode));
                    }
                    catch (CommunicationException communicationException)
                    {
                        loggerManager.LogError(communicationException);
                    }
                    catch(TimeoutException timeoutException)
                    {
                        loggerManager.LogError(timeoutException);
                    }
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
            HostBehaviorManager.ChangeToReentrant();
            if (chatCallbacks.ContainsKey(gamertag))
            {
                chatCallbacks.Remove(gamertag);
            }
        }

        public void BroadcastMessage(String matchCode, Message message)
        {
            HostBehaviorManager.ChangeToReentrant();
            messagesforMatch[matchCode].Add(message);
            DisplayMessages(matchCode);
        }
    }
}
