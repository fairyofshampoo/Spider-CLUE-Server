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
    public interface IFriendshipManager
    {
        [OperationContract]
        List<string> GetFriendList(string gamertag);

        [OperationContract]
        int DeleteFriend(string gamertag, string friendGamertag);

        [OperationContract]
        int AddFriend(string gamertag, string friendGamertag);

        [OperationContract]
        bool AreNotFriends(string gamertag, string friendGamertag);

        [OperationContract]
        bool ThereIsNoFriendRequest(string gamertag, string friendGamertag);
    }
}
