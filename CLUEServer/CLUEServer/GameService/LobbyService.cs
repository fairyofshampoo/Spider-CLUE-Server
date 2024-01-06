﻿using DataBaseManager;
using GameService.Contracts;
using GameService.Utilities;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;

namespace GameService.Services
{
    public partial class GameService : ILobbyManager
    {

        private static readonly Dictionary<string, ILobbyManagerCallback> gamersLobbyCallback = new Dictionary<string, ILobbyManagerCallback>();
        public void BeginMatch(string matchCode)
        {
            HostBehaviorManager.ChangeToReentrant();
            foreach (var gamer in gamersInMatch
                .Where(entry => entry.Value.Equals(matchCode))
                .Where(entry => gamersLobbyCallback.ContainsKey(entry.Key)))
            {
                if (gamersLobbyCallback.ContainsKey(gamer.Key))
                {
                    gamersLobbyCallback[gamer.Key].StartGame();
                }
            }
        }

        public void ConnectToLobby(string gamertag)
        {
            HostBehaviorManager.ChangeToReentrant();
            if (!gamersLobbyCallback.ContainsKey(gamertag))
            {
                gamersLobbyCallback.Add(gamertag, OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>());
            }
        }

        public bool IsOwnerOfTheMatch(string gamertag, string matchCode)
        {
            using (var context = new SpiderClueDbEntities()) 
            {
                HostBehaviorManager.ChangeToSingle();
                bool isOwner = context.matches.Any(match => match.codeMatch == matchCode && match.createdBy == gamertag);
                HostBehaviorManager.ChangeToReentrant();
                return isOwner;
            }
        }

        public void KickPlayer(string gamertag)
        {
            HostBehaviorManager.ChangeToReentrant();
            if (gamersLobbyCallback.ContainsKey(gamertag))
            {
                gamersLobbyCallback[gamertag].KickPlayerFromMatch(gamertag);
            }
        }
    }
}
