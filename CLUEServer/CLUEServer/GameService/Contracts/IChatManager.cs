using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace GameService.Contracts
{
    [ServiceContract(CallbackContract = typeof(IChatManagerCallback))]
    public interface IChatManager
    {
        [OperationContract(IsOneWay = true)]
        void ConnectToChat(string gamertag, String matchCode);

        [OperationContract]
        void DisconnectFromChat(string gamertag);

        [OperationContract(IsOneWay = true)]
        void BroadcastMessage(String matchCode, Message message);
    }

    [ServiceContract]
    public interface IChatManagerCallback
    {
        [OperationContract]
        void ReceiveMessages(List<Message> messages);
    }

    [DataContract]
    public class Message
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public string GamerTag { get; set; }
    }
}
