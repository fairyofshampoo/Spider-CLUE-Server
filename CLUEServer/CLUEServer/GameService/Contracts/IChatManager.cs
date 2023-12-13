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
        void ConnectToChat(string username, int code);

        [OperationContract]
        void DisconnectFromChat(string username);

        [OperationContract(IsOneWay = true)]
        void SendMessage(int code, Message message);
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
