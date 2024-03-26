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

        /// <summary>
        /// Connects the user to the chat service for a specific game.
        /// </summary>
        /// <param name="gamertag">The gamertag of the user connecting.</param>
        /// <param name="matchCode">The unique code of the game being joined.</param>
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
                    catch (TimeoutException timeoutException)
                    {
                        loggerManager.LogError(timeoutException);
                    }
                }
            }
        }

        private List<Message> RetrieveMessagesForMatch(string matchCode)
        {
            if (!messagesforMatch.ContainsKey(matchCode))
            {
                List<Message> messages = new List<Message>();
                messagesforMatch.Add(matchCode, messages);
            }
            return messagesforMatch[matchCode];
        }

        /// <summary>
        /// Disconnects the user from the chat service for a specific game.
        /// </summary>
        /// <param name="gamertag">The gamertag of the user to disconnect.</param>
        public void DisconnectFromChat(string gamertag)
        {
            HostBehaviorManager.ChangeToReentrant();
            if (chatCallbacks.ContainsKey(gamertag))
            {
                chatCallbacks.Remove(gamertag);
            }
        }


        /// <summary>
        /// Broadcasts a message to all users in a specific game.
        /// </summary>
        /// <param name="matchCode">The unique code of the game to broadcast the message to.</param>
        /// <param name="message">The message to be broadcasted.</param>
        public void BroadcastMessage(string matchCode, Message message)
        {
            HostBehaviorManager.ChangeToReentrant();
            messagesforMatch[matchCode].Add(message);
            DisplayMessages(matchCode);
        }
    }
}
