using DataBaseManager;
using GameService.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Services
{
    public partial class GameService : IMatchCreationManager
    {
        public void CreateMatch(string gamertag)
        {
            using (var databaseContext = new SpiderClueDbEntities())
            {
                var match = new DataBaseManager.match
                {
                    codeMatch = GenerateMatchCode(),
                    createdBy = gamertag,
                };
                databaseContext.matches.Add(match);
                databaseContext.SaveChanges();
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
            } while (isCodeValid(matchCode) == false);
            return matchCode;
        }

        private Boolean isCodeValid(string matchCode)
        {
            Boolean validation = false;
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
