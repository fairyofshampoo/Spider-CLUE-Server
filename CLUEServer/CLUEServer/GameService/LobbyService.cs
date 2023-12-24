using DataBaseManager;
using GameService.Contracts;
using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using GameService.Utilities;

namespace GameService.Services
{
    public partial class GameService : ILobbyManager
    {

        private static readonly Dictionary<string, ILobbyManagerCallback> gamersLobbyCallback = new Dictionary<string, ILobbyManagerCallback>();
        public void BeginMatch(string matchCode)
        {
            foreach (var gamer in gamersInMatch)
            {
                if (gamer.Value.Equals(matchCode))
                {
                    string gamertag = gamer.Key;

                    if (gamersLobbyCallback.ContainsKey(gamertag))
                    {
                        gamersLobbyCallback[gamertag].StartGame();
                    }
                }
            }
        }

        public bool IsOwnerOfTheMatch(string gamertag, string matchCode)
        {

            using (var context = new SpiderClueDbEntities()) 
            {
                bool isOwner = context.matches.Any(match => match.codeMatch == matchCode && match.createdBy == gamertag);

                return isOwner;
            }
        }


        public void KickPlayer(string gamertag)
        {
            if (gamersLobbyCallback.ContainsKey(gamertag))
            {
                gamersLobbyCallback[gamertag].KickPlayerFromMatch(gamertag);
            }
        }

        private readonly List<Character> characters = new List<Character>
        {
            new Character { CharacterName = "BlueCharacter", PawnName = "BluePawn" },
            new Character { CharacterName = "GreenCharacter", PawnName = "GreenPawn" },
            new Character { CharacterName = "PurpleCharacter", PawnName = "PurplePawn"},
            new Character { CharacterName = "RedCharacter", PawnName = "RedPawn" },
            new Character { CharacterName = "WhiteCharacter", PawnName = "WhitePawn" },
            new Character { CharacterName = "YellowCharacter", PawnName = "YellowPawn"}
        };

        private readonly Dictionary<string, Character> charactersPerGamer = new Dictionary<string, Character>();

        private void SetCharacterColor(string gamertag, string matchCode)
        {
            Random random = new Random();
            Character assignedCharacter = null;

            while (assignedCharacter == null || !IsCharacterAvailable(matchCode, assignedCharacter))
            {
                int index = random.Next(characters.Count);
                assignedCharacter = characters[index];
            }

            charactersPerGamer.Add(gamertag, assignedCharacter);
            Console.WriteLine(gamertag + " color asignado: " + assignedCharacter.CharacterName);

        }

        public void ConnectToLobby(string gamertag, string matchCode)
        {
            LoggerManager logger = new LoggerManager(this.GetType());

            try
            {
                gamersLobbyCallback.Add(gamertag, OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>());
                Console.WriteLine(gamertag + " conectado a lobby");
                SetCharacterColor(gamertag, matchCode);
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
            }

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

        public Character GetCharacterPerGamer(string gamertag)
        {
            Character character = null;

            if(charactersPerGamer.ContainsKey(gamertag))
            {
                character = charactersPerGamer[gamertag];
            }

            return character;
        }
    }
}
