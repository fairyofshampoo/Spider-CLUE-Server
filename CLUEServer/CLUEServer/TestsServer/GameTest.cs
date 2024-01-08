using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TestsServer.SpiderClueService;
using Xunit;

namespace TestsServer
{
    public class GameTest
    {
        private static GameManagerClient _gameProxy;
        private static LobbyManagerClient _lobbyProxy;
        private static GameManagerImplementation _implementation;
        private static MatchManagerClient _matchProxy;

        public GameTest()
        {
            _implementation = new GameManagerImplementation();
            _gameProxy = new GameManagerClient(new InstanceContext(_implementation));
            _lobbyProxy = new LobbyManagerClient(new InstanceContext(_implementation));
            _matchProxy = new MatchManagerClient(new InstanceContext(_implementation));
        }

        [Fact]
        public async void MoveInvalidZoneTest()
        {
            string gamertag = "Jake";
            string gamertag2 = "soobin";
            string gamertag3 = "michito";
            SpiderClueService.IMatchCreationManager matchClient = new SpiderClueService.MatchCreationManagerClient();
            string matchCode = matchClient.CreateMatch(gamertag);
            _matchProxy.ConnectToMatch(gamertag, matchCode);
            _matchProxy.ConnectToMatch(gamertag2, matchCode);
            _matchProxy.ConnectToMatch(gamertag3, matchCode);
            await Task.Delay(4000);
            await _lobbyProxy.ConnectToLobbyAsync(gamertag);
            await _lobbyProxy.ConnectToLobbyAsync(gamertag2);
            await _lobbyProxy.ConnectToLobbyAsync(gamertag3);
            _lobbyProxy.BeginMatch(matchCode);
            await Task.Delay(4000);
            Assert.True(_implementation.isGameStarted);
            await _gameProxy.ConnectGamerToGameBoardAsync(gamertag, matchCode);
            await _gameProxy.ConnectGamerToGameBoardAsync(gamertag2, matchCode);
            await _gameProxy.ConnectGamerToGameBoardAsync(gamertag3, matchCode);
            int column = 16;
            int row = 4;
            if (_implementation.isMyTurn)
            {
                await _gameProxy.RollDiceAsync(gamertag);
                await _gameProxy.MovePawnAsync(column, row, gamertag, matchCode);
            }

            Assert.False(_implementation.isAnInvalidZone);
        }

    }

    public class GameManagerImplementation : ILobbyManagerCallback, IMatchManagerCallback, IGameManagerCallback
    {
        public bool isKicked { get; set; }
        public string kickedPlayer { get; set; }
        public bool isGameStarted { get; set; }
        public bool isConnectedToMatch { get; set; }
        public bool isMyTurn { get; set; }

        public bool isAnInvalidZone {  get; set; }

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

        public void LeaveGameBoard()
        {

        }

        public void ReceiveCardAccused(Card card)
        {

        }

        public void ReceiveCommonAccusationByOtherGamer(string[] accusation)
        {

        }

        public void ReceiveCommonAccusationOption(bool isEnabled, Door door)
        {

        }

        public void ReceiveFinalAccusationOption(bool isEnabled)
        {

        }


        public void ReceiveInvalidMove()
        {

        }

        public void ReceivePawnsMove(Pawn pawn)
        {

        }

        public void ReceiveTurn(bool isYourTurn)
        {
            isMyTurn = isYourTurn;
        }

        public void ReceiveWinner(string winnerGamertag, string gamerIcon)
        {

        }

        public void RequestShowCard(Card[] cards, string accuser)
        {

        }

        public void ShowNobodyAnswers()
        {

        }
    }
}
