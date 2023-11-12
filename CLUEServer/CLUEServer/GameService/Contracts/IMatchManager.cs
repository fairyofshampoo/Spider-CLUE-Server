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
    internal interface IMatchManager
    {
        [OperationContract]
        Match getMatchInformation(string code);
    }

    [DataContract]
    public class Match
    {
        private string code;
        private string createdBy;

        [DataMember]
        public string Code { get { return code; } set { code = value; } }

        [DataMember]
        public string CreatedBy { get {  return createdBy; } set {  createdBy = value; } }
    }
}
