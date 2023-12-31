﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Contracts
{
    [ServiceContract(CallbackContract = typeof(ILobbyManagerCallback))]
    public interface ILobbyManager
    {

        [OperationContract(IsOneWay = true)]
        void KickPlayer(string gamertag);

        [OperationContract(IsOneWay = true)]
        void BeginMatch(string matchCode);

        [OperationContract]
        bool IsOwnerOfTheMatch(string gamertag,string matchCode);
    }


    [ServiceContract]
    public interface ILobbyManagerCallback
    {
        [OperationContract]
        void KickPlayerFromMatch(string gamertag);

        [OperationContract]
        void StartGame();
    }
}
