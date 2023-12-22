using DataBaseManager;
using GameService.Contracts;
using GameService.Utilities;
using System;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;

namespace GameService.Services
{
    public partial class GameService : IMatchCreationManager
    {
        public string CreateMatch(string gamertag)
        {
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
            catch (Exception exception)
            {
                logger.LogFatal(exception);
            }

            return matchcreationResult;
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
            } while (IsCodeValid(matchCode) == false);
            return matchCode;
        }

        private bool IsCodeValid(string matchCode)
        {
            bool validation = false;
            using (var databaseContext = new SpiderClueDbEntities())
            {
                var isCodeExisting = databaseContext.matches.FirstOrDefault(match => match.codeMatch == matchCode);
                if (isCodeExisting == null)
                {
                    validation = true;
                }
                return validation;
            }
        }
    }
}
