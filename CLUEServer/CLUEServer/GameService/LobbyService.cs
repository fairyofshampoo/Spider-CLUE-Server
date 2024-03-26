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

        /// <summary>
        /// Initiates the specified match by notifying connected gamers and starting the game.
        /// </summary>
        /// <param name="matchCode">The unique code of the match to begin.</param>
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

        /// <summary>
        /// Connects a gamer to the lobby, adding them to the lobby callback dictionary.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer to connect.</param>
        public void ConnectToLobby(string gamertag)
        {
            HostBehaviorManager.ChangeToReentrant();
            if (!gamersLobbyCallback.ContainsKey(gamertag))
            {
                gamersLobbyCallback.Add(gamertag, OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>());
            }
        }

        /// <summary>
        /// Checks if a gamer is the owner of the specified match.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer to check.</param>
        /// <param name="matchCode">The unique code of the match to check.</param>
        /// <returns>True if the gamer is the owner, false otherwise.</returns>
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
            catch (CommunicationException communicationException)
            {
                loggerManager.LogError(communicationException);
                isOwner = false;
            }
            catch (TimeoutException timeoutException)
            {
                loggerManager.LogError(timeoutException);
                isOwner = false;
            }

            return isOwner;
        }

        /// <summary>
        /// Kicks a player from the match by notifying them through their lobby callback.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer to kick.</param>
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
