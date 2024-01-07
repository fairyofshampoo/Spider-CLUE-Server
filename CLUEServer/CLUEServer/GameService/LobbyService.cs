using DataBaseManager;
using GameService.Contracts;
using GameService.Utilities;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System;

namespace GameService.Services
{
    public partial class GameService : ILobbyManager
    {

        private static readonly Dictionary<string, ILobbyManagerCallback> gamersLobbyCallback = new Dictionary<string, ILobbyManagerCallback>();
        public void BeginMatch(string matchCode)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToReentrant();
            foreach (var gamer in gamersInMatch
                .Where(entry => entry.Value.Equals(matchCode))
                .Where(entry => gamersLobbyCallback.ContainsKey(entry.Key)))
            {
                if (gamersLobbyCallback.ContainsKey(gamer.Key))
                {
                    try
                    {
                        gamersLobbyCallback[gamer.Key].StartGame();
                    }
                    catch (SqlException sqlException)
                    {
                        loggerManager.LogError(sqlException);
                    }
                    catch (EntityException entityException)
                    {
                        loggerManager.LogError(entityException);
                    }
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
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            bool isOwner = false;
            try
            {
                using (var context = new SpiderClueDbEntities())
                {
                    HostBehaviorManager.ChangeToSingle();
                    isOwner = context.matches.Any(match => match.codeMatch == matchCode && match.createdBy == gamertag);
                    HostBehaviorManager.ChangeToReentrant();
                }
            }
            catch (SqlException sqlException)
            {
                loggerManager.LogError(sqlException);
            }
            catch (EntityException entityException)
            {
                loggerManager.LogError(entityException);
            }

            return isOwner;
        }

        public void KickPlayer(string gamertag)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToReentrant();
            if (gamersLobbyCallback.ContainsKey(gamertag))
            {
                try
                {
                    gamersLobbyCallback[gamertag].KickPlayerFromMatch(gamertag);
                }
                catch (CommunicationException communicationException)
                {
                    loggerManager.LogError(communicationException);
                }
                catch (TimeoutException timeoutException)
                {
                    loggerManager.LogError(timeoutException);
                }
            }
        }
    }
}
