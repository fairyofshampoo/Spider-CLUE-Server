using DataBaseManager;
using GameService.Contracts;
using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;

namespace GameService.Services
{
    public partial class GameService : ILobbyManager
    {
        public void KickPlayer(string gamertag)
        {
            if (gamersLobbyCallback.ContainsKey(gamertag))
            {
                gamersLobbyCallback[gamertag].KickPlayerFromMatch(gamertag);
                string matchCode = gamersInMatch[gamertag];
                LeaveMatch(gamertag, matchCode);
            }
        }
    }
}
