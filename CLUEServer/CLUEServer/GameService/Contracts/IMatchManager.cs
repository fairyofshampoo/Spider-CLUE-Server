using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Contracts
{
    [ServiceContract(CallbackContract = typeof(IMatchManagerCallback))]
    public interface IMatchManager
    {
        [OperationContract]
        Match GetMatchInformation(string code);

        [OperationContract(IsOneWay = true)]
        void ConnectToMatch(string gamertag, string matchCode);

        [OperationContract]
        void LeaveMatch(string gamertag, string matchCode);

        [OperationContract(IsOneWay = true)]
        void GetGamersInMatch(string gamertag, string code);
    }

    [ServiceContract]
    public interface IMatchManagerCallback
    {
        [OperationContract]
        void ReceiveGamersInMatch(Dictionary<string, Pawn> characters);
    }

    [DataContract]
    public class Match
    {
 
        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }
    }



}
