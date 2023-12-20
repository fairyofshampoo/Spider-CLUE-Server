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
        public void BeginMatch(string matchCode)
        {
            foreach (var gamer in gamersInMatch)
            {
                if (gamer.Value.Equals(matchCode))
                {
                    string gamertag = gamer.Key;

                    if (gamersLobbyCallback.ContainsKey(gamertag))
                    {
                        gamersLobbyCallback[gamertag].StartGame();
                    }
                }
            }
        }

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
