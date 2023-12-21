using DataBaseManager;
using GameService.Contracts;
using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;

namespace GameService.Services
{
    public partial class GameService : IMatchManager
    {
        private static readonly Dictionary<string, string> gamersInMatch = new Dictionary<string, string>();
        private static readonly Dictionary<string, IMatchManagerCallback> gamersMatchCallback = new Dictionary<string, IMatchManagerCallback>();
        private static readonly Dictionary<string, ILobbyManagerCallback> gamersLobbyCallback = new Dictionary<string, ILobbyManagerCallback>();

        public void ConnectToMatch(string gamertag, string matchCode)
        {
            gamersInMatch.Add(gamertag, matchCode);
            gamersMatchCallback.Add(gamertag, OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>());
            gamersLobbyCallback.Add(gamertag, OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>());
            ShowPlayerProfilesInMatch(matchCode);
        }

        private void ShowPlayerProfilesInMatch(string matchCode)
        {
            foreach(var gamer in gamersInMatch)
            {
                if (gamer.Value.Equals(matchCode))
                {
                    string gamertag = gamer.Key;
                    gamersMatchCallback[gamertag].ReceiveGamersInMatch(GetGamersByMatch(matchCode));
                }
            }
        }

        private List<string> GetGamersByMatch(string matchCode)
        {
            return gamersInMatch.Where(gamer => gamer.Value == matchCode).Select(gamer => gamer.Key).ToList();
        }


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
            string allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string matchCode = new string(Enumerable.Repeat(allowedCharacters, 6)
                .Select(selection => selection[random.Next(selection.Length)]).ToArray());  
            return matchCode;
        }

        public void GetGamersInMatch(string gamertag, string code)
        {
            if (gamersInMatch.ContainsKey(gamertag))
            {
                OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>().ReceiveGamersInMatch(new List<string>(gamersInMatch.Keys));
            }
            else
            {
                OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>().ReceiveGamersInMatch(GetGamersByMatch(code));
            }
        }

        public Match GetMatchInformation(string code)
        {
            using (var databaseContext = new SpiderClueDbEntities())
            {
                Match result = new Match();
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
                return result;
            }
        }

        public void LeaveMatch(string gamertag, string matchCode)
        {
            gamersInMatch.Remove(gamertag);
            gamersMatchCallback.Remove(gamertag);
            gamersLobbyCallback.Remove(gamertag);
            ShowPlayerProfilesInMatch(matchCode);
        }
    }
}
