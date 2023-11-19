using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Contracts
{
    [ServiceContract (CallbackContract = typeof(IFriendRequestManager))]
    public interface IFriendRequestManager
    {
        [OperationContract]
        void showAllFriendsRequest(string gamertag);
 
    }

    [ServiceContract]
    public interface IFriendRequestManagerCallback
    {
        [OperationContract]
        void ReceiveFriendsRequests();
    }
}
