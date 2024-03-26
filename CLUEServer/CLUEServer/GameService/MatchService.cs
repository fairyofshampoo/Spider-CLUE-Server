using DataBaseManager;
using GameService.Contracts;
using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using GameService.Utilities;
using log4net.Repository.Hierarchy;
using System.Data.Entity.Core;
using System.Data.SqlClient;

namespace GameService.Services
{
    /// <summary>
    /// Partial class for the GameService, implementing the IMatchManager interface.
    /// Manages game-related functionality such as connecting to a match, retrieving match information,
    /// and handling actions when gamers leave a match.
    /// </summary>
    public partial class GameService : IMatchManager
    {
        private static readonly Dictionary<string, string> gamersInMatch = new Dictionary<string, string>();
        private static readonly Dictionary<string, IMatchManagerCallback> gamersMatchCallback = new Dictionary<string, IMatchManagerCallback>();
        private static readonly Dictionary<string, List<Pawn>> pawnsAvailableInMatch = new Dictionary<string, List<Pawn>>();

        /// <summary>
        /// Connects a gamer to a match, associates the gamer with the match, and sets up initial character properties.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer.</param>
        /// <param name="matchCode">The code of the match.</param>
        public void ConnectToMatch(string gamertag, string matchCode)
        {
            HostBehaviorManager.ChangeToReentrant();
            gamersInMatch.Add(gamertag, matchCode);
            gamersMatchCallback.Add(gamertag, OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>());
            SetCharacterColor(gamertag, matchCode);
            ShowPlayerProfilesInMatch(matchCode);
        }

        private void ShowPlayerProfilesInMatch(string matchCode)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToReentrant();
            lock (gamersMatchCallback)
            {
                foreach (var gamerEntry in gamersInMatch
                    .Where(entry => entry.Value.Equals(matchCode))
                    .Select(entry => entry.Key)
                    .Where(gamertag => gamersMatchCallback.ContainsKey(gamertag))
                    .ToList())
                {
                    if (gamersMatchCallback.ContainsKey(gamerEntry))
                    {
                        try
                        {
                            gamersMatchCallback[gamerEntry].ReceiveGamersInMatch(GetCharactersInMatch(matchCode));
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

        private List<string> GetGamersByMatch(string matchCode)
        {
            return gamersInMatch.Where(gamer => gamer.Value == matchCode).Select(gamer => gamer.Key).ToList();
        }

        /// <summary>
        /// Retrieves the gamers in a match and notifies the calling gamer's callback channel.
        /// </summary>
        /// <param name="gamertag">The gamertag of the calling gamer.</param>
        /// <param name="code">The code identifying the match.</param>
        public void GetGamersInMatch(string gamertag, string code)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToReentrant();
            try
            {
                OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>().ReceiveGamersInMatch(GetCharactersInMatch(code));
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

        /// <summary>
        /// Retrieves information about a specific match.
        /// </summary>
        /// <param name="code">The code of the match.</param>
        /// <returns>The information about the match or null if the match doesn't exist.</returns>
        public Match GetMatchInformation(string code)
        {
            Match result = new Match();
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToReentrant();
            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    var matchExist = databaseContext.matches.FirstOrDefault(match => match.codeMatch == code);
                    if (matchExist == null)
                    {
                        result = null;
                    }
                    else
                    {
                        result.Code = matchExist.codeMatch;
                        result.CreatedBy = matchExist.createdBy;
                    }
                }
            }
            catch (EntityException entityException)
            {
                loggerManager.LogError(entityException);
            }
            catch (SqlException sqlException)
            {
                loggerManager.LogError(sqlException);
            }

            return result;
        }

        /// <summary>
        /// Disconnects a gamer from a match, removes the gamer from associated data structures, and shows updated player profiles in the match.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer to disconnect.</param>
        /// <param name="matchCode">The code of the match.</param>
        public void LeaveMatch(string gamertag, string matchCode)
        {
            HostBehaviorManager.ChangeToReentrant();
            RemoveFromMatch(gamertag);
            ShowPlayerProfilesInMatch(matchCode);
        }

        private void RemoveFromMatch(string gamertag)
        {
            string matchCode = gamersInMatch[gamertag];
            gamersInMatch.Remove(gamertag);
            gamersMatchCallback.Remove(gamertag);
            gamersLobbyCallback.Remove(gamertag);
            DisconnectFromChat(gamertag);
            RestoreCharacterInMatch(gamertag, matchCode);
        }

        private void RestoreCharacterInMatch(string gamertag, string matchCode)
        {

            if (charactersPerGamer.ContainsKey(gamertag))
            {
                Pawn assignedCharacter = charactersPerGamer[gamertag];
                charactersPerGamer.Remove(gamertag);
                List<Pawn> characters = pawnsAvailableInMatch[matchCode];
                characters.Add(assignedCharacter);
                AddPawnsToMatch(matchCode, characters);
            }
        }

        private static readonly Dictionary<string, Pawn> charactersPerGamer = new Dictionary<string, Pawn>();

        private void SetCharacterColor(string gamertag, string matchCode)
        {
            List<Pawn> characters = pawnsAvailableInMatch[matchCode];

            if (!charactersPerGamer.ContainsKey(gamertag))
            {
                Random random = new Random();
                Pawn assignedCharacter = null;

                while (assignedCharacter == null)
                {
                    int index = random.Next(characters.Count);
                    assignedCharacter = characters[index];
                }

                characters.Remove(assignedCharacter);
                AddPawnsToMatch(matchCode, characters);
                charactersPerGamer.Add(gamertag, assignedCharacter);
            }
        }

        /// <summary>
        /// Retrieves the character associated with a specific gamer.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer.</param>
        /// <returns>The character associated with the gamer or null if no character is found.</returns>
        public Pawn GetCharacterPerGamer(string gamertag)
        {
            Pawn character = null;

            if (charactersPerGamer.ContainsKey(gamertag))
            {
                character = charactersPerGamer[gamertag];
            }

            return character;
        }

        /// <summary>
        /// Retrieves the characters of gamers in a specific match.
        /// </summary>
        /// <param name="code">The code of the match.</param>
        /// <returns>A dictionary containing gamertags and their corresponding characters in the match.</returns>
        public Dictionary<string, Pawn> GetCharactersInMatch(string code)
        {
            List<string> gamers = GetGamersByMatch(code);
            Dictionary<string, Pawn> charactersInMatch = new Dictionary<string, Pawn>();

            foreach (string gamer in gamers)
            {
                Pawn character = GetCharacterPerGamer(gamer);
                charactersInMatch.Add(gamer, character);
            }

            return charactersInMatch;
        }
    }
}
