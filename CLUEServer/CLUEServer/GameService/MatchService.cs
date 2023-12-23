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
                            gamersMatchCallback[gamertag].ReceiveGamersInMatch(GetGamersByMatch(matchCode));
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
            chatCallbacks.Remove(gamertag);
            charactersPerGamer.Remove(gamertag);
            ShowPlayerProfilesInMatch(matchCode);
        }

        private static List<Character> characters = new List<Character>
        {
            new Character { CharacterName = "BlueCharacter", PawnName = "BluePawn" },
            new Character { CharacterName = "GreenCharacter", PawnName = "GreenPawn" },
            new Character { CharacterName = "PurpleCharacter", PawnName = "PurplePawn"},
            new Character { CharacterName = "RedCharacter", PawnName = "RedPawn" },
            new Character { CharacterName = "WhiteCharacter", PawnName = "WhitePawn" },
            new Character { CharacterName = "YellowCharacter", PawnName = "YellowPawn"}
        };

        private static readonly Dictionary<string, Character> charactersPerGamer = new Dictionary<string, Character>();

        public Character GetCharacterColor(string gamertag, string matchCode)
        {
            Random random = new Random();
            Character assignedCharacter = null;

            while (assignedCharacter == null || !IsCharacterAvailable(matchCode, assignedCharacter))
            {
                int index = random.Next(characters.Count);
                assignedCharacter = characters[index];
            }

            charactersPerGamer.Add(gamertag, assignedCharacter);
            return assignedCharacter;
        }


        public bool IsCharacterAvailable(string matchCode, Character characterSelected)
        {
            bool isCharacterAvailable = false;

            var assignedCharactersInMatch = charactersPerGamer
                .Where(gamerCharacterPair => gamersInMatch.ContainsValue(matchCode))
                .Select(gamerCharacterPair => gamerCharacterPair.Value)
                .ToList();

            isCharacterAvailable = !assignedCharactersInMatch.Contains(characterSelected);

            return isCharacterAvailable;
        }

    }
}
