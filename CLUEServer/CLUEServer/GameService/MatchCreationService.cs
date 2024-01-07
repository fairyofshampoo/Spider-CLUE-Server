using DataBaseManager;
using GameService.Contracts;
using GameService.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;

namespace GameService.Services
{
    public partial class GameService : IMatchCreationManager
    {
        public string CreateMatch(string gamertag)
        {
            HostBehaviorManager.ChangeToSingle();
            LoggerManager logger = new LoggerManager(this.GetType());
            string matchcreationResult = string.Empty;
            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    var match = new DataBaseManager.match
                    {
                        codeMatch = GenerateMatchCode(),
                        createdBy = gamertag,
                    };

                    databaseContext.matches.Add(match);
                    if (databaseContext.SaveChanges() > 0)
                    {
                        matchcreationResult = match.codeMatch;
                        AddPawnsToMatch(match.codeMatch, CreatePawns());
                    }
                }
            }
            catch (EntityException entityException)
            {
                logger.LogError(entityException);
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
            }
            HostBehaviorManager.ChangeToReentrant();
            return matchcreationResult;
        }

        private void AddPawnsToMatch(string matchCode, List<Pawn> pawns)
        {
            if (!pawnsAvailableInMatch.ContainsKey(matchCode))
            {
                pawnsAvailableInMatch.Add(matchCode, pawns);
            }
            else
            {
                pawnsAvailableInMatch[matchCode] = pawns;
            }
        }

        private string GenerateMatchCode()
        {
            string matchCode;
            do
            {
                string allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                Random random = new Random();
                matchCode = new string(Enumerable.Repeat(allowedCharacters, 6)
                    .Select(selection => selection[random.Next(selection.Length)]).ToArray());
            } while (!IsCodeValid(matchCode));
            return matchCode;
        }

        private bool IsCodeValid(string matchCode)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            bool validation = false;
            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    var isCodeExisting = databaseContext.matches.FirstOrDefault(match => match.codeMatch == matchCode);
                    if (isCodeExisting == null)
                    {
                        validation = true;
                    }
                }
            }
            catch (CommunicationException communicationException)
            {
                loggerManager.LogError(communicationException);
                validation = false;
            }
            catch (TimeoutException timeoutException)
            {
                loggerManager.LogError(timeoutException);
                validation = false;
            }

            return validation;
        }
    }
}
