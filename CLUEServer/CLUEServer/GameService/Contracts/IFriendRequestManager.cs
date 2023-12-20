using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Contracts
{
    [ServiceContract]
    interface IFriendRequestManager
    {
        [OperationContract]
        string [] GetFriendsRequets(string gamertag);

        [OperationContract]
        void CreateFriendRequest(string gamertag, string friendGamertag);

        [OperationContract]
        void ResponseFriendRequest(string gamertag, string friendGamertag, string response);

        [OperationContract]
        void DeleteFriendRequest(string gamertag, string friend);
    }

    [DataContract]
    public class FriendRequest
    {
        private string senderGamertag;
        private string receiverGamertag;
        private string friendRequestStatus;

        [DataMember]
        public string SenderGamertag { get { return senderGamertag; } set { senderGamertag = value; } }

        [DataMember]
        public string ReceiverGamertag { get { return receiverGamertag; } set { receiverGamertag = value; } }

        [DataMember]
        public string FriendRequestStatus { get { return friendRequestStatus; } set { friendRequestStatus = value; } }
    }

}
