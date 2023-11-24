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
        private static readonly Dictionary<string, string> gamersInMatch = new Dictionary<string, string>();

        public void ConnectToMatch(string gamertag, string code)
        {
            gamersInMatch.Add(gamertag, code);
        }

        public void CreateMatch(Match newMatch)
        {
            throw new NotImplementedException();
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
