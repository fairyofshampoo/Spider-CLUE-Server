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
        string [] GetFriendsRequest(string gamertag);

        [OperationContract]
        int CreateFriendRequest(string gamertag, string friendGamertag);

        [OperationContract]
        int ResponseFriendRequest(string gamertag, string friendGamertag, string response);

        [OperationContract]
        int DeleteFriendRequest(string gamertag, string friend);
    }

    [DataContract]
    public class FriendRequest
    {

        [DataMember]
        public string SenderGamertag { get; set; }

        [DataMember]
        public string ReceiverGamertag { get; set; }

        [DataMember]
        public string FriendRequestStatus { get; set; }
    }

}
