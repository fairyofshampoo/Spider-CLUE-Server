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
        void DeleteFriend(string gamertag, string friendGamertag);

        [OperationContract]
        void AddFriend(string gamertag, string friendGamertag);

        [OperationContract]
        Boolean AreFriends(string gamertag, string friendGamertag);
    }
}
