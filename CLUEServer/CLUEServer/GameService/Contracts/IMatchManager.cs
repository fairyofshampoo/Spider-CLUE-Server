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
    [ServiceContract]
    internal interface IMatchManager
    {
        [OperationContract]
        Match GetMatchInformation(string code);

        [OperationContract(IsOneWay = true)]
        void ConnectToMatch(string gamertag, string code);

        [OperationContract]
        void CreateMatch(string gamertag);

        [OperationContract]
        void LeaveMatch(string gamertag, string code);

        [OperationContract(IsOneWay = true)]
        bool KickPlayer(string gamertag);

        [OperationContract(IsOneWay = true)]
        void GetGamersInMatch(string gamertag, string code);
    }

    [ServiceContract]
    public interface IMatchManagerCallback
    {
        [OperationContract]
        void KickPlayerFromMatch(string username);
        [OperationContract]
        void ReceiveGamersInMatch(List<string> gamertags);
        [OperationContract]
        void StartMatch();
    }

    [DataContract]
    public class Match
    {
        private string code;
        private string createdBy;
        private int totalPlayers;

        [DataMember]
        public string Code { get { return code; } set { code = value; } }

        [DataMember]
        public string CreatedBy { get {  return createdBy; } set {  createdBy = value; } }

        [DataMember] public int TotalPlayers { get { return totalPlayers; } set {  totalPlayers = value; } }
    }
}
