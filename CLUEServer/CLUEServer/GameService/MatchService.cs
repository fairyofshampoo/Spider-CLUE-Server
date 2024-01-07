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
    public partial class GameService : IMatchManager
    {
        private static readonly Dictionary<string, string> gamersInMatch = new Dictionary<string, string>();
        private static readonly Dictionary<string, IMatchManagerCallback> gamersMatchCallback = new Dictionary<string, IMatchManagerCallback>();
        private static readonly Dictionary<string, List<Pawn>> pawnsAvailableInMatch = new Dictionary<string, List<Pawn>>();

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

        public Pawn GetCharacterPerGamer(string gamertag)
        {
            Pawn character = null;

            if (charactersPerGamer.ContainsKey(gamertag))
            {
                character = charactersPerGamer[gamertag];
            }

            return character;
        }

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
