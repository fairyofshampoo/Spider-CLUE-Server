using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestsServer.SpiderClueService;
using System.ServiceModel;
using Xunit;
using System.Runtime.InteropServices;

namespace TestsServer
{
    public class LobbyTest : IDisposable
    {
        private static LobbyManagerClient _lobbyProxy;
        private static LobbyManagerImplementation _implementation;
        private static MatchManagerClient _matchProxy;

        public LobbyTest()
        {
            _implementation = new LobbyManagerImplementation();
            _lobbyProxy = new LobbyManagerClient(new InstanceContext(_implementation));
            _matchProxy = new MatchManagerClient(new InstanceContext(_implementation));
        }

        public void Dispose()
        {

        }

        [Fact]
        public async void KickPlayerTest()
        {
            string gamertag = "Star3oy";
            _lobbyProxy.ConnectToLobby(gamertag);
            await Task.Delay(4000);
            _lobbyProxy.KickPlayer(gamertag);
            await Task.Delay(4000);
            Assert.True(_implementation.isKicked);
            Assert.Equal(gamertag, _implementation.kickedPlayer);
        }

        [Fact]
        public async void StartGameTest()
        {
            string gamertag = "Jake";
            SpiderClueService.IMatchCreationManager matchClient = new SpiderClueService.MatchCreationManagerClient();
            string matchCode = matchClient.CreateMatch(gamertag);
            _matchProxy.ConnectToMatch(gamertag, matchCode);
            await Task.Delay(4000);
            await _lobbyProxy.ConnectToLobbyAsync(gamertag);
            Assert.True(_implementation.isConnectedToMatch);
            _lobbyProxy.BeginMatch(matchCode);
            await Task.Delay(4000);
            Assert.True(_implementation.isGameStarted);
        }

        [Fact]
        public async void CreateMatchTest()
        {
            string gamertag = "Jake";
            SpiderClueService.IMatchCreationManager matchClient = new SpiderClueService.MatchCreationManagerClient();
            string matchCode = matchClient.CreateMatch(gamertag);
            _matchProxy.ConnectToMatch(gamertag, matchCode);
            await Task.Delay(4000);
            await _lobbyProxy.ConnectToLobbyAsync(gamertag);
            Assert.True(_implementation.isConnectedToMatch);
        }
    }

    public class LobbyManagerImplementation : ILobbyManagerCallback, IMatchManagerCallback
    {
        public bool isKicked { get; set; }
        public string kickedPlayer { get; set; }
        public bool isGameStarted { get; set; }
        public bool isConnectedToMatch { get; set; }

        public void KickPlayerFromMatch(string gamertag)
        {
            isKicked = true;
            kickedPlayer = gamertag;
        }

        public void ReceiveGamersInMatch(Dictionary<string, Pawn> characters)
        {
            isConnectedToMatch = true;
        }

        public void StartGame()
        {
            isGameStarted = true;
        }
    }
}
