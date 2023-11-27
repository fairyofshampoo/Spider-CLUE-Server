using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Contracts
{
    [ServiceContract]
    public interface ILobbyManager
    {

        [OperationContract(IsOneWay = true)]
        void KickPlayer(string gamertag);
    }


    [ServiceContract]
    public interface ILobbyManagerCallback
    {
        [OperationContract]
        void KickPlayerFromMatch(string username);
        [OperationContract]
        void StartMatch();
    }
}
