using System;
using System.Collections.Generic;
using System.Linq;
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
        void CreateFriendRequest(string gamertag, string friendGamertag);

        [OperationContract]
        void AcceptFriendRequest(string gamertag, string friendGamertag);

        [OperationContract]
        void DeclineFriendRequest(string gamertg, string friendGamertag);
    }
}
