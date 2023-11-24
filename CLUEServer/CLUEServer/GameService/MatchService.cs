using DataBaseManager;
using GameService.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Services
{
    public partial class GameService : IMatchManager
    {
        public void ConnectToMatch(string gamertag, string code)
        {
            throw new NotImplementedException();
        }

        public void CreateMatch(string gamertag)
        {
            using (var databaseContext = new SpiderClueDbEntities())
            {
                var match = new DataBaseManager.match
                {
                    codeMatch = GenerateMatchCode(gamertag),
                    createdBy = gamertag,
                };
                databaseContext.matches.Add(match);
                databaseContext.SaveChanges();
            }
        }

        private string GenerateMatchCode(string gamertag)
        {
            DateTime currentDate = DateTime.Now;
            string date = currentDate.ToString("yyyyMMddHHmmss");
            Random random = new Random();
            string randomNumber = random.Next(10, 100).ToString();
            string matchCode = ( gamertag + date + randomNumber);
            return matchCode;
        }

        public void GetGamersInMatch(string gamertag, string code)
        {
            throw new NotImplementedException();
        }

        public Match GetMatchInformation(string code)
        {
            using (var databaseContext = new SpiderClueDbEntities())
            {
                Match result = new Match();
                var matchExist = databaseContext.matches.FirstOrDefault(match => match.codeMatch == code);
                if (matchExist != null)
                {
                    result.Code = matchExist.codeMatch;
                    result.CreatedBy = matchExist.createdBy;
                }
                return result;
            }
        }

        public bool KickPlayer(string gamertag)
        {
            throw new NotImplementedException();
        }

        public void LeaveMatch(string gamertag, string code)
        {
            throw new NotImplementedException();
        }
    }
}
