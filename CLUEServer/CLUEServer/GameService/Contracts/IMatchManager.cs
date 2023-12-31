﻿using System;
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
        void ConnectToMatch(string gamertag, string code);

        [OperationContract]
        void LeaveMatch(string gamertag, string code);

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
        private string code;
        private string createdBy;

        [DataMember]
        public string Code { get { return code; } set { code = value; } }

        [DataMember]
        public string CreatedBy { get {  return createdBy; } set {  createdBy = value; } }
    }

}
