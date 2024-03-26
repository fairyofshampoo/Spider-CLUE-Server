using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace GameService.Contracts
{
    /// <summary>
    /// Defines the contract for a chat manager service with callback capabilities.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IChatManagerCallback))]
    public interface IChatManager
    {
        /// <summary>
        /// Connects a gamer to a chat within a specified match.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer.</param>
        /// <param name="matchCode">The code of the match.</param>
        [OperationContract(IsOneWay = true)]
        void ConnectToChat(string gamertag, string matchCode);

        /// <summary>
        /// Disconnects a gamer from the chat.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer to disconnect.</param>
        [OperationContract]
        void DisconnectFromChat(string gamertag);

        /// <summary>
        /// Broadcasts a message to all participants in a match chat.
        /// </summary>
        /// <param name="matchCode">The code of the match.</param>
        /// <param name="message">The message to be broadcasted.</param>
        [OperationContract(IsOneWay = true)]
        void BroadcastMessage(string matchCode, Message message);
    }

    /// <summary>
    /// Defines the callback contract for the chat manager.
    /// </summary>
    [ServiceContract]
    public interface IChatManagerCallback
    {
        /// <summary>
        /// Receives a list of messages from the chat.
        /// </summary>
        /// <param name="messages">The list of messages to receive.</param>
        [OperationContract]
        void ReceiveMessages(List<Message> messages);
    }

    /// <summary>
    /// Represents a chat message with text and associated gamer tag.
    /// </summary>
    [DataContract]
    public class Message
    {
        /// <summary>
        /// Gets or sets the text content of the message.
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the gamertag of the gamer who sent the message.
        /// </summary>
        [DataMember]
        public string GamerTag { get; set; }
    }
}
