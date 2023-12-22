using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Contracts
{
    [ServiceContract]
    public interface ISessionManager
    {
        [OperationContract]
        void Connect(string gamertag);

        [OperationContract]
        void Disconnect(string gamertag);
    }
}
