using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Contracts
{
    [ServiceContract(CallbackContract = typeof(IFriendsManagerCallback))]
    public interface IFriendsManager
    {
        [OperationContract(IsOneWay = true)]
        void GetConnectedFriends(string gamertag);

        [OperationContract(IsOneWay = true)]
        void JoinFriendsConnected(string gamertag);
    }

    [ServiceContract]
    public interface IFriendsManagerCallback
    {
        [OperationContract]
        void ReceiveConnectedFriends(List<string> connectedFriends);
    }
}
