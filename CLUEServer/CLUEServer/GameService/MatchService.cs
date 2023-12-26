using DataBaseManager;
using GameService.Contracts;
using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using GameService.Utilities;

namespace GameService.Services
{
    public partial class GameService : IMatchManager
    {
        private static readonly Dictionary<string, string> gamersInMatch = new Dictionary<string, string>();
        private static readonly Dictionary<string, IMatchManagerCallback> gamersMatchCallback = new Dictionary<string, IMatchManagerCallback>();

        public void ConnectToMatch(string gamertag, string matchCode)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            try
            {
                gamersInMatch.Add(gamertag, matchCode);
                gamersMatchCallback.Add(gamertag, OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>());
                SetCharacterColor(gamertag, matchCode);
                ShowPlayerProfilesInMatch(matchCode);
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
            }
            
        }

        private void ShowPlayerProfilesInMatch(string matchCode)
        {
            lock (gamersMatchCallback)
            {
                foreach (var gamer in gamersInMatch.ToList())
                {
                    if (gamer.Value.Equals(matchCode))
                    {
                        string gamertag = gamer.Key;

                        if (gamersMatchCallback.ContainsKey(gamertag))
                        {
                            gamersMatchCallback[gamertag].ReceiveGamersInMatch(GetCharactersInMatch(matchCode));
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
            OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>().ReceiveGamersInMatch(GetCharactersInMatch(code));
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
            charactersPerGamer.Remove(gamertag);
            ShowPlayerProfilesInMatch(matchCode);
        }

        private static readonly Dictionary<string, Pawn> charactersPerGamer = new Dictionary<string, Pawn>();

        private void SetCharacterColor(string gamertag, string matchCode)
        {
            List<Pawn> characters = CreatePawns();

            if (!charactersPerGamer.ContainsKey(gamertag))
            {
                Random random = new Random();
                Pawn assignedCharacter = null;

                while (assignedCharacter == null || !IsCharacterAvailable(matchCode, assignedCharacter))
                {
                    int index = random.Next(characters.Count);
                    assignedCharacter = characters[index];
                }

                charactersPerGamer.Add(gamertag, assignedCharacter);
            }
        }


        private bool IsCharacterAvailable(string matchCode, Pawn characterSelected)
        {
            var assignedCharactersInMatch = charactersPerGamer
                .Where(gamerPair => gamersInMatch.ContainsKey(gamerPair.Key) && gamersInMatch[gamerPair.Key] == matchCode)
                .Select(gamerPair => gamerPair.Value)
                .ToList();

            return !assignedCharactersInMatch.Contains(characterSelected);
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
