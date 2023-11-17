using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Contracts
{
    [ServiceContract]
    public interface IFriendRequestManager
    {
        [OperationContract]
        List<string> GetFriendList(string gamertag);

        [OperationContract]
        void DeleteFriend(string gamertag, string friendGamertag);
    }
}
