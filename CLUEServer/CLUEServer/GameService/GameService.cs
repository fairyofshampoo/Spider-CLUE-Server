using DataBaseManager;
using GameService.Contracts;
using GameService.Utilities;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.ServiceModel;
using System.Data.Entity.Core;
using System.Data.SqlClient;

namespace GameService.Services
{
    public class GamerLeftAndRight
    {
        public string Gamertag { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
    }

    public partial class GameService : IGameManager
    {
        private static Dictionary<string, Timer> turnTimers = new Dictionary<string, Timer>();
        private static readonly Dictionary<string, IGameManagerCallback> GamersInGameBoardCallback = new Dictionary<string, IGameManagerCallback>();
        private static readonly Dictionary<string, string> GamersInGameBoard = new Dictionary<string, string>();
        private static readonly Dictionary<string, List<GamerLeftAndRight>> DirectionInGameBoard = new Dictionary<string, List<GamerLeftAndRight>>();
        private static readonly Dictionary<string, string> TurnsInGameboard = new Dictionary<string, string>(); 
        private static readonly Dictionary<string, int> GameBoardDiceRoll = new Dictionary<string, int>();

        public void ConnectGamerToGameBoard(string gamertag, string matchCode)
        {
            HostBehaviorManager.ChangeToReentrant();
            var callback = OperationContext.Current.GetCallbackChannel<IGameManagerCallback>();
            if (!GamersInGameBoardCallback.ContainsKey(gamertag))
            {
                GamersInGameBoardCallback.Add(gamertag, callback);
                GamersInGameBoard.Add(gamertag, matchCode);
                StartMatchInGameBoard(matchCode);
            }
        }

        private void StartMatchInGameBoard(string matchCode)
        {
            if (AreAllPlayersConnected(matchCode))
            {
                SetTurns(matchCode);
                CreateCards(matchCode);
                SendFirstTurn(matchCode);
                StartTurnTimer(matchCode);
            }
        }

        private void StartTurnTimer(string matchCode)
        {
            Timer turnTimer = new Timer(30000);
            turnTimer.Elapsed += (sender, e) => OnTurnTimerElapsed(matchCode);
            turnTimer.AutoReset = false;
            turnTimer.Start();

            turnTimers[matchCode] = turnTimer;
        }

        private void OnTurnTimerElapsed(string matchCode)
        {
            ChangeTurn(matchCode, TurnsInGameboard[matchCode]);
        }

        private bool AreAllPlayersConnected(string matchCode)
        {
            bool areAllPlayersConnected = false;
            List<string> gamersInLobby = GetGamersByMatch(matchCode);
            int playersCount = gamersInLobby.Count(gamer => GamersInGameBoard.ContainsKey(gamer));

            if (playersCount == 3)
            {
                areAllPlayersConnected = true;
            }

            return areAllPlayersConnected;
        }

        private void SendFirstTurn(string matchCode)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            List<GamerLeftAndRight> turnsList = DirectionInGameBoard[matchCode];
            string gamertag = turnsList[0].Gamertag;
            TurnsInGameboard.Add(matchCode, gamertag );

            try
            {
                GamersInGameBoardCallback[gamertag].ReceiveTurn(true);
            }
            catch (CommunicationException communicationException)
            {
                loggerManager.LogError(communicationException);
            }
            catch (TimeoutException timeoutException)
            {
                loggerManager.LogError(timeoutException);
            }
            
        }

        private List<string> GetGamersByGameBoard(string matchCode)
        {
            return GamersInGameBoard.Where(gamer => gamer.Value == matchCode).Select(gamer => gamer.Key).ToList();
        }

        private void RemoveFromGameboard(string gamertag)
        {
            decks.Remove(gamertag);
            GamersInGameBoard.Remove(gamertag);
            GamersInGameBoardCallback.Remove(gamertag);
        }

        public void DisconnectFromBoard(string gamertag)
        {
            RemoveFromMatch(gamertag);
            RemoveFromGameboard(gamertag);
        }

        public void EndGame(string matchCode)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToReentrant();
            List<string> gamerByBoard = GetGamersByGameBoard(matchCode);
            foreach(string gamer in gamerByBoard)
            {
                try
                {
                    GamersInGameBoardCallback[gamer].LeaveGameBoard();
                    DisconnectFromBoard(gamer);
                }
                catch (CommunicationException communicationException)
                {
                    loggerManager.LogError(communicationException);
                }
                catch (TimeoutException timeoutException)
                {
                    loggerManager.LogError(timeoutException);
                }
            }

            RemoveGameboard(matchCode);
        }

        private void RemoveGameboard(string matchCode)
        {
            DirectionInGameBoard.Remove(matchCode);
            GameBoardDiceRoll.Remove(matchCode);
            clueDeckByMatch.Remove(matchCode);
        }

        private void SetTurns(string matchCode)
        {
            List<string> gamerByBoard = GetGamersByGameBoard(matchCode);
            string gamer1 = gamerByBoard[0];
            string gamer2 = gamerByBoard[1];
            string gamer3 = gamerByBoard[2];

            List<GamerLeftAndRight> gamerLeftAndRights = new List<GamerLeftAndRight>()
            {
                new GamerLeftAndRight { Gamertag = gamer1, Left = gamer2, Right = gamer3},
                new GamerLeftAndRight { Gamertag = gamer2, Left = gamer3, Right = gamer1},
                new GamerLeftAndRight { Gamertag = gamer3, Left = gamer1, Right =  gamer2}
            };
            DirectionInGameBoard.Add(matchCode, gamerLeftAndRights);
        }

        public void MovePawn(int column, int row, string gamertag, string matchCode)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToReentrant();
            Pawn pawn = new Pawn();
            if (IsAValidMove(column, row, gamertag, matchCode))
            {
                pawn.XPosition = column;
                pawn.YPosition = row;
                if (charactersPerGamer.ContainsKey(gamertag))
                {
                    pawn.Color = charactersPerGamer[gamertag].Color;
                    charactersPerGamer[gamertag].XPosition = column;
                    charactersPerGamer[gamertag].YPosition = row;
                }
                ShowMovePawn(pawn, GetMatchCode(gamertag));
                ChangeTurn(matchCode, gamertag);
            }
            else
            {
                ShowMoveIsInvalid(gamertag);

                try
                {
                    GamersInGameBoardCallback[gamertag].ReceiveTurn(true);
                }
                catch (CommunicationException communicationException)
                {
                    loggerManager.LogError(communicationException);
                }
                catch (TimeoutException timeoutException)
                {
                    loggerManager.LogError(timeoutException);
                }
            }
        }

        private string GetMatchCode(string gamertag)
        {
            string matchCode = string.Empty;
            foreach (var gamer in GamersInGameBoard)
            {
                if (gamer.Key == gamertag)
                {
                    matchCode = gamer.Value;
                }
            }
            return matchCode;
        }

        public void ChangeTurn(string matchCode, string gamertag)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());

            if (turnTimers.ContainsKey(matchCode))
            {
                turnTimers[matchCode].Stop();
            }

            foreach (var turn in DirectionInGameBoard)
            {
                if (turn.Key.Equals(matchCode))
                {
                    List<GamerLeftAndRight> GamersTurns = turn.Value;
                    foreach (var gamer in GamersTurns)
                    {
                        if (gamer.Gamertag == gamertag)
                        {
                            if (GamersInGameBoardCallback.ContainsKey(gamer.Left) && GamersInGameBoardCallback.ContainsKey(gamertag))
                            {
                                try
                                {
                                    GamersInGameBoardCallback[gamertag].ReceiveTurn(false);
                                    TurnsInGameboard[matchCode] = gamer.Left;
                                    GamersInGameBoardCallback[gamer.Left].ReceiveTurn(true);
                                    StartTurnTimer(matchCode);
                                }
                                catch (CommunicationException communicationException)
                                {
                                    loggerManager.LogError(communicationException);
                                }
                                catch (TimeoutException timeoutException)
                                {
                                    loggerManager.LogError(timeoutException);
                                }
                            }

                            break;
                        }
                    }
                    break;
                }
            }
        }

        public void ShowMovePawn(Pawn pawn, string matchCode)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            foreach (var gamer in GamersInGameBoard 
                .Where(entry => entry.Value.Equals(matchCode))
                .Select(entry => entry.Key)
                .Where(gamertag => GamersInGameBoardCallback.ContainsKey(gamertag)))
            {
                if (GamersInGameBoardCallback.ContainsKey(gamer))
                {
                    try
                    {
                        GamersInGameBoardCallback[gamer].ReceivePawnsMove(pawn);
                    }
                    catch (CommunicationException communicationException)
                    {
                        loggerManager.LogError(communicationException);
                    }
                    catch (TimeoutException timeoutException)
                    {
                        loggerManager.LogError(timeoutException);
                    }
                }
            }

            ResetDice(matchCode);

        }

        private void ResetDice(string matchCode)
        {
            if (GameBoardDiceRoll.ContainsKey(matchCode))
            {
                GameBoardDiceRoll[matchCode] = 0;
            }
        }

        public void ShowMoveIsInvalid(string gamertag)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());

            if (GamersInGameBoard.ContainsKey(gamertag) && GamersInGameBoardCallback.ContainsKey(gamertag))
            {
                try
                {
                    GamersInGameBoardCallback[gamertag].ReceiveInvalidMove();
                }
                catch (CommunicationException communicationException)
                {
                    loggerManager.LogError(communicationException);
                }
                catch (TimeoutException timeoutException)
                {
                    loggerManager.LogError(timeoutException);
                }
            }
        }

        public GridNode GetPawnPosition(string gamertag)
        {
            GridNode node = new GridNode();
            if (charactersPerGamer.ContainsKey(gamertag))
            {
                Pawn pawn = charactersPerGamer[gamertag];
                node.Xposition = pawn.XPosition;
                node.Yposition = pawn.YPosition;
            }
            return node;
        }

        private int GetGameBoardRollDice(string matchCode)
        {
            int rollDice = 0;
            if (GameBoardDiceRoll.ContainsKey(matchCode))
            {
                rollDice = GameBoardDiceRoll[matchCode];
            }
            return rollDice;
        }

        private void SendAccusationOption(string gamertag, Door door)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());

            if (GamersInGameBoardCallback.ContainsKey(gamertag))
            {
                try
                {
                    GamersInGameBoardCallback[gamertag].ReceiveCommonAccusationOption(true, door);
                }
                catch (CommunicationException communicationException)
                {
                    loggerManager.LogError(communicationException);
                }
                catch (TimeoutException timeoutException)
                {
                    loggerManager.LogError(timeoutException);
                }
            }
        }

        private Door GetDoor(int column, int row)
        {
            Door doorData = new Door();
            foreach (var door in Doors)
            {
                if (door.Xposition == column && door.Yposition == row)
                {
                    doorData.Xposition = door.Xposition;
                    doorData.Yposition = door.Yposition;
                    doorData.ZoneName = door.ZoneName;
                    break;
                }
            }
            return doorData;
        }

        public bool IsAValidMove(int column, int row, string gamertag, string matchCode)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            bool isAValidMove = false;
            int rollDice = GetGameBoardRollDice(matchCode);
            if (IsADoor(column, row)) 
            {
                GridNode start = GetPawnPosition(gamertag);
                GridNode finish = new GridNode
                {
                    Xposition = column,
                    Yposition = row,
                };
                isAValidMove = AreTheStepsValid(start, finish, rollDice);

                if (isAValidMove)
                {
                    Door door = GetDoor(column, row);
                    SendAccusationOption(gamertag, door);
                } 
            }
            else if (IsAnInvalidZone(column, row)) 
            {
                if (IsAValidCorner(column, row))
                {
                    GridNode start = GetPawnPosition(gamertag);
                    GridNode finish = new GridNode
                    {
                        Xposition = column,
                        Yposition = row,
                    };
                    isAValidMove = AreTheStepsValid(start, finish, rollDice);
                }
            }
            else if(IsTheCenter(column, row))
            {
                GridNode start = GetPawnPosition(gamertag);
                GridNode finish = new GridNode
                {
                    Xposition = column,
                    Yposition = row,
                };
                isAValidMove = AreTheStepsValid(start, finish, rollDice);
                if(isAValidMove)
                {
                    try
                    {
                        GamersInGameBoardCallback[gamertag].ReceiveFinalAccusationOption(true);
                    }
                    catch (CommunicationException communicationException)
                    {
                        loggerManager.LogError(communicationException);
                        isAValidMove = false;
                    }
                    catch (TimeoutException timeoutException)
                    {
                        loggerManager.LogError(timeoutException);
                        isAValidMove = false;
                    }
                }
            } 
            else
            {
                GridNode start = GetPawnPosition(gamertag);
                GridNode finish = new GridNode
                {
                    Xposition = column,
                    Yposition = row,
                };
                isAValidMove = AreTheStepsValid(start, finish, rollDice);
            }
            return isAValidMove;
        }

        private bool IsTheCenter(int column, int row)
        {
            bool isTheCenter = false;
            if(column >= 8 && column <= 12 && row >= 7 && row <= 13)
            {
                isTheCenter = true;
            }
            return isTheCenter;
        }

        private bool AreTheStepsValid(GridNode start, GridNode finish, int rollDice)
        {
            bool isValidStep = false;
            if(rollDice != 0)
            {
                isValidStep = SearchMoves(start, start, finish, rollDice, new List<GridNode>(), new Queue<GridNode>());
            }
            return isValidStep;
        }

        private bool SearchMoves(GridNode start, GridNode current, GridNode end, int steps, List<GridNode> visitedNodes, Queue<GridNode> nextNodes)
        {
            if (current.Xposition == end.Xposition && current.Yposition == end.Yposition && GetNumberOfSteps(start, current) >= 0)
            {
                return true;
            }

            if (GetNumberOfSteps(start, current) > steps)
            {
                return false;
            }      
            visitedNodes.Add(current);
            nextNodes = GetNeighbors(current, nextNodes, visitedNodes);
            GridNode nextNode = nextNodes.Dequeue();
            return SearchMoves(start, nextNode, end, steps, visitedNodes, nextNodes);
        }

        private int GetNumberOfSteps(GridNode start, GridNode end)
        {
            int differenceColumn = start.Xposition - end.Xposition;
            int differentRow = start.Yposition - end.Yposition;
            int total = Math.Abs(differenceColumn) + Math.Abs(differentRow);
            return total;
        }

        private Queue<GridNode> GetNeighbors(GridNode current, Queue<GridNode> nextNodes, List<GridNode> visitedNodes)
        {
            List<GridNode> neighbors = new List<GridNode>() {
                new GridNode() { Xposition = current.Xposition, Yposition = current.Yposition - 1 },
                new GridNode() { Xposition = current.Xposition, Yposition = current.Yposition + 1 },
                new GridNode() { Xposition = current.Xposition - 1, Yposition = current.Yposition },
                new GridNode() { Xposition = current.Xposition + 1, Yposition = current.Yposition },
            };

            foreach (GridNode neighborNode in neighbors.Where(node => IsNeighborValid(node, visitedNodes)).ToList())
            {
                nextNodes.Enqueue(neighborNode);
            }

            return nextNodes;
        }

        private bool IsNeighborValid (GridNode neighbor, List<GridNode> visitedNodes)
        {
            bool isNeighborValid = false;
            if (!IsNeighborInVisitedNodes(neighbor, visitedNodes) && !IsNeighborInInvalidZone(neighbor))
            {
                isNeighborValid = true;
            }
            return isNeighborValid;
        }

        private bool IsNeighborInVisitedNodes(GridNode neighbor, List<GridNode> visitedNodes)
        {
            bool isNeighborVisited = false;
            foreach (GridNode node in visitedNodes) 
            {
                if(node.Xposition == neighbor.Xposition && node.Yposition == neighbor.Yposition)
                {
                    isNeighborVisited = true;
                }
            }
            return isNeighborVisited;
        }

        private bool IsNeighborInInvalidZone(GridNode neighbor)
        {
            bool isNeighborInvalid = false;
            foreach (GridNode node in InvalidZones)
            {
                if (node.Xposition == neighbor.Xposition && node.Yposition == neighbor.Yposition)
                {
                    isNeighborInvalid = true;
                }
            }

            return isNeighborInvalid;
        }

        public int RollDice(string matchCode)
        {
            Random random = new Random();
            int rollDice = random.Next(2, 13);
            if (GameBoardDiceRoll.ContainsKey(matchCode))
            {
                GameBoardDiceRoll[matchCode] = rollDice;
            }
            else
            {
                GameBoardDiceRoll.Add(matchCode, rollDice);
            }
            return rollDice;
        }

        public bool IsAnInvalidZone(int column, int row)
        {
            bool isAnInvalidZone;

            if (column < 6)
            {
                isAnInvalidZone = IsAnInvalidZoneOfTheFirstSection(column, row);
            }
            else if (column >= 7 && column <= 14)
            {
                isAnInvalidZone = IsAnInvalidZoneOfTheSecondSection(column, row);
            }
            else
            {
                isAnInvalidZone = IsAnInvalidZoneOfTheThirdSection(column, row);
            }
            return isAnInvalidZone;
        }

        private bool IsAnInvalidZoneOfTheFirstSection (int column, int row)
        {
            bool isAnInvalidZone = false;
            if (row < 3) 
            {
                isAnInvalidZone = true;
            }
            else if (row > 4 && row < 10) 
            {
                isAnInvalidZone = true;
            }
            else if (row > 10 && row < 16 && column < 5) 
            {
                isAnInvalidZone = true;
            }
            else if (column < 5 && row > 17) 
            {
                isAnInvalidZone = true;
            }
            return isAnInvalidZone;
        }

        private bool IsAnInvalidZoneOfTheSecondSection(int column, int row)
        {
            bool isAnInvalidZone = false;
            if (row < 6 && column > 7 && column < 14)
            {
                isAnInvalidZone = true;
            }
            else if (row > 15) 
            {
                isAnInvalidZone = true;
            }
            return isAnInvalidZone;
        }

        private bool IsAnInvalidZoneOfTheThirdSection(int column, int row)
        {
            bool isAnInvalidZone = false;
            if (column > 15 && row < 5) 
            {
                isAnInvalidZone = true;
            }
            else if (row > 7 && row < 15 && column >= 15) 
            {
                isAnInvalidZone = true;
            }
            else if (column > 16 && row > 16) 
            {
                isAnInvalidZone = true;
            }
            return isAnInvalidZone;
        }

        public bool IsADoor(int column, int row)
        {
            bool isADoor = false;
            foreach (var door in Doors)
            {
                if (door.Xposition == column && door.Yposition == row)
                {
                    isADoor = true;
                    break;
                }
            }
            return isADoor;
        }

        public bool IsAValidCorner(int column, int row)
        {
            bool isAValidCorner = false;
            foreach (var grid in AllowedCorners)
            {
                if (grid.Xposition.Equals(column) && grid.Yposition.Equals(row))
                {
                    isAValidCorner = true;
                }
            }
            return isAValidCorner;
        }

        public void MakeFinalAccusation(List<string> cards, string matchCode, string gamertag)
        {
            HostBehaviorManager.ChangeToReentrant();
            int cardsCount = CountMatchingCards(cards, matchCode);

            if (cardsCount == 3)
            {
                NotifyWinner(gamertag, matchCode);
                UpdateGamesWonByGamer(gamertag);
            }
            else
            {
                RemoveFromTurns(gamertag, matchCode);
            }
        }

        private int CountMatchingCards(List<string> cards, string matchCode)
        {
            int cardsCount = 0;

            if (clueDeckByMatch.TryGetValue(matchCode, out List<Card> clueDeck))
            {
                cardsCount = clueDeck.Count(card => cards.Contains(card.ID));
            }

            return cardsCount;
        }

        private void NotifyWinner(string winnerGamertag, string matchCode)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            string winnerIcon = GetIcon(winnerGamertag);

            foreach (var gamerEntry in GamersInGameBoard.Where(entry => entry.Value.Equals(matchCode)))
            {
                string gamerFound = gamerEntry.Key;

                if (GamersInGameBoardCallback.ContainsKey(gamerFound))
                {
                    try
                    {
                        GamersInGameBoardCallback[gamerFound].ReceiveWinner(winnerGamertag, winnerIcon);
                    }
                    catch (CommunicationException communicationException)
                    {
                        loggerManager.LogError(communicationException);
                    }
                    catch (TimeoutException timeoutException)
                    {
                        loggerManager.LogError(timeoutException);
                    }
                }
            }
        }

        private int UpdateGamesWonByGamer(string gamertag)
        {
            HostBehaviorManager.ChangeToSingle();
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            int result = Constants.ErrorInOperation;

            try
            {
                using (var dataBaseContext = new SpiderClueDbEntities())
                {
                    var gamer = dataBaseContext.gamers.FirstOrDefault(player => player.gamertag == gamertag);
                    if (gamer != null)
                    {
                        gamer.gamesWon++;
                        dataBaseContext.SaveChanges();
                        result = Constants.SuccessInOperation;
                    }
                    else
                    {
                        result = Constants.ErrorInOperation;
                    }
                }
            }
            catch (SqlException sqlException)
            {
                loggerManager.LogError(sqlException);
                result = Constants.ErrorInOperation;
            }
            catch (EntityException entityException)
            {
                loggerManager.LogError(entityException);
                result = Constants.ErrorInOperation;
            }
            
            HostBehaviorManager.ChangeToReentrant();
            return result;
        }

        private void RemoveFromTurns(string gamertag, string matchCode)
        {
            string leftGamer = string.Empty;
            string rightGamer = string.Empty;
            int index = 0;
            if (DirectionInGameBoard.ContainsKey(matchCode))
            {
                foreach (var gamer in DirectionInGameBoard[matchCode])
                {
                    if (gamer.Gamertag == gamertag)
                    {
                        leftGamer = gamer.Left;
                        rightGamer = gamer.Right;
                        break;
                    }
                }

                foreach(var gamer in DirectionInGameBoard[matchCode])
                {
                    if(gamer.Gamertag == rightGamer)
                    {
                        break;
                    }
                    index++;
                }
                DirectionInGameBoard[matchCode][index].Left = leftGamer;
            }
        }

        public void ShowCard(Card card, string matchCode, string accuser)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToReentrant();
            try
            {
                GamersInGameBoardCallback[accuser].ReceiveCardAccused(card);
            }
            catch (SqlException sqlException)
            {
                loggerManager.LogError(sqlException);
            }
            catch (EntityException entityException)
            {
                loggerManager.LogError(entityException);
            }
        }

        public void ShowCommonAccusation(string[] accusation, string matchCode, string accuser)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToReentrant();
            foreach (var gamerEntry in GamersInGameBoard.Where(entry => entry.Value.Equals(matchCode)))
            {
                string gamertag = gamerEntry.Key;

                if (GamersInGameBoardCallback.ContainsKey(gamertag))
                {
                    try
                    {
                        GamersInGameBoardCallback[gamertag].ReceiveCommonAccusationByOtherGamer(accusation);
                    }
                    catch (SqlException sqlException)
                    {
                        loggerManager.LogError(sqlException);
                    }
                    catch (EntityException entityException)
                    {
                        loggerManager.LogError(entityException);
                    }
                }
            }

            string leftGamer = GetLeftGamer(matchCode, accuser);
            IsLeftOwnerOfCards(accusation, accuser, leftGamer, matchCode);
        }

        private void IsLeftOwnerOfCards(string[] accusation, string accuser, string leftGamer, string matchCode)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());

            if (accuser != leftGamer)
            {
                List<Card> leftGamerDeck = GetDeck(leftGamer);
                List<Card> cardsInCommon = FindCardsInCommon(accusation, leftGamerDeck);

                if (cardsInCommon.Any())
                {
                    try
                    {
                        GamersInGameBoardCallback[leftGamer].RequestShowCard(cardsInCommon, accuser);
                    }
                    catch (CommunicationException communicationException)
                    {
                        loggerManager.LogError(communicationException);
                    }
                    catch (TimeoutException timeoutException)
                    {
                        loggerManager.LogError(timeoutException);
                    }
                }
                else
                {
                    string leftOfLeftGamer = GetLeftGamer(matchCode, leftGamer);
                    IsLeftOwnerOfCards(accusation, accuser, leftOfLeftGamer, matchCode);
                }
            }
            else
            {
                ShowNobodyAnswers(matchCode);
            }
        }

        private List<Card> FindCardsInCommon(string[] accusation, List<Card> deck)
        {
            return deck.Where(card => accusation.Contains(card.ID)).ToList();
        }

        private void ShowNobodyAnswers(string matchCode)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            foreach (var gamer in GamersInGameBoard.ToList())
            {
                if (gamer.Value.Equals(matchCode))
                {
                    string gamertag = gamer.Key;
                    try
                    {
                        if (GamersInGameBoardCallback.ContainsKey(gamertag))
                        {
                            GamersInGameBoardCallback[gamertag].ShowNobodyAnswers();
                        }
                    }
                    catch (CommunicationException communicationException)
                    {
                        loggerManager.LogError(communicationException);
                    }
                    catch (TimeoutException timeoutException)
                    {
                        loggerManager.LogError(timeoutException);
                    }
                }
            }
        }

        private string GetLeftGamer(string matchCode, string gamertag)
        {
            string leftGamer = string.Empty;
            if(DirectionInGameBoard.ContainsKey(matchCode))
            {
                foreach (var gamer in DirectionInGameBoard[matchCode])
                {
                    if(gamer.Right == gamertag)
                    {
                        leftGamer = gamer.Gamertag;
                    }
                }
            }
            return leftGamer;
        }

        public List<Pawn> CreatePawns()
        {
            Pawn bluePawn = new Pawn { Color = "BluePawn.png", XPosition = 0, YPosition = 17 };
            Pawn purplePawn = new Pawn { Color = "PurplePawn.png", XPosition = 0, YPosition = 4 };
            Pawn whitePawn = new Pawn { Color = "WhitePawn.png", XPosition = 13, YPosition = 22 };
            Pawn redPawn = new Pawn { Color = "RedPawn.png", XPosition = 15, YPosition = 0 };
            Pawn yellowPawn = new Pawn { Color = "YellowPawn.png", XPosition = 21, YPosition = 6 };
            Pawn greenPawn = new Pawn { Color = "GreenPawn.png", XPosition = 8, YPosition = 22 };

            List<Pawn> pawns = new List<Pawn>();
            pawns.Add(bluePawn);
            pawns.Add(purplePawn);
            pawns.Add(whitePawn);
            pawns.Add(redPawn);
            pawns.Add(yellowPawn);
            pawns.Add(greenPawn);
            return pawns;
        }

        public List<Card> GetDeck(string gamertag)
        {
            List<Card> gamerDeck = new List<Card>();
            if (decks.ContainsKey(gamertag))
            {
                gamerDeck = decks[gamertag];
            }
            return gamerDeck;
        }

        private readonly List<GridNode> AllowedCorners = new List<GridNode>()
        {
            new GridNode { Xposition = 5, Yposition = 5,},
            new GridNode { Xposition = 5, Yposition = 9,},
            new GridNode { Xposition = 4, Yposition = 18,},
            new GridNode { Xposition = 8, Yposition = 22,},
            new GridNode { Xposition = 7, Yposition = 22,},
            new GridNode { Xposition = 13, Yposition = 22,},
            new GridNode { Xposition = 14, Yposition = 22,},
            new GridNode { Xposition = 17, Yposition = 14,},
            new GridNode { Xposition = 16, Yposition = 14,},
            new GridNode { Xposition = 15, Yposition = 14,}
        };

        public List<GridNode> InvalidZones { get; set; } = new List<GridNode>()
        {
            new GridNode { Xposition = 0, Yposition = 2, },
            new GridNode { Xposition = 1, Yposition = 2, },
            new GridNode { Xposition = 2, Yposition = 2, },
            new GridNode { Xposition = 3, Yposition = 2, },
            new GridNode { Xposition = 4, Yposition = 2, },
            new GridNode { Xposition = 5, Yposition = 1, },
            new GridNode { Xposition = 5, Yposition = 0, },
            new GridNode { Xposition = 4, Yposition = 1, },

            new GridNode { Xposition = 0, Yposition = 5, },
            new GridNode { Xposition = 1, Yposition = 5, },
            new GridNode { Xposition = 2, Yposition = 5, },
            new GridNode { Xposition = 3, Yposition = 5, },
            new GridNode { Xposition = 4, Yposition = 5, },
            new GridNode { Xposition = 5, Yposition = 6, },
            new GridNode { Xposition = 5, Yposition = 8, },
            new GridNode { Xposition = 4, Yposition = 9, },
            new GridNode { Xposition = 3, Yposition = 9, },
            new GridNode { Xposition = 1, Yposition = 9, },
            new GridNode { Xposition = 0, Yposition = 9, },
            new GridNode { Xposition = 4, Yposition = 7, },
            new GridNode { Xposition = 2, Yposition = 8, },

            new GridNode { Xposition = 0, Yposition = 12, },
            new GridNode { Xposition = 1, Yposition = 11, },
            new GridNode { Xposition = 2, Yposition = 11, },
            new GridNode { Xposition = 3, Yposition = 11, },
            new GridNode { Xposition = 4, Yposition = 11, },
            new GridNode { Xposition = 4, Yposition = 12, },
            new GridNode { Xposition = 4, Yposition = 13, },
            new GridNode { Xposition = 3, Yposition = 14, },
            new GridNode { Xposition = 0, Yposition = 15, },
            new GridNode { Xposition = 1, Yposition = 15, },
            new GridNode { Xposition = 2, Yposition = 15, },
            new GridNode { Xposition = 3, Yposition = 15, },
            new GridNode { Xposition = 4, Yposition = 15, },

            new GridNode { Xposition = 0, Yposition = 18, },
            new GridNode { Xposition = 1, Yposition = 18, },
            new GridNode { Xposition = 2, Yposition = 18, },
            new GridNode { Xposition = 4, Yposition = 19, },
            new GridNode { Xposition = 4, Yposition = 20, },
            new GridNode { Xposition = 4, Yposition = 21, },
            new GridNode { Xposition = 3, Yposition = 19, },

            new GridNode { Xposition = 8, Yposition = 0, },
            new GridNode { Xposition = 8, Yposition = 1, },
            new GridNode { Xposition = 8, Yposition = 2, },
            new GridNode { Xposition = 9, Yposition = 3, },
            new GridNode { Xposition = 8, Yposition = 4, },
            new GridNode { Xposition = 8, Yposition = 5, },
            new GridNode { Xposition = 9, Yposition = 5, },
            new GridNode { Xposition = 10, Yposition = 4, },
            new GridNode { Xposition = 11, Yposition = 4, },
            new GridNode { Xposition = 12, Yposition = 5, },
            new GridNode { Xposition = 13, Yposition = 5, },
            new GridNode { Xposition = 13, Yposition = 4, },
            new GridNode { Xposition = 13, Yposition = 3, },
            new GridNode { Xposition = 13, Yposition = 2, },
            new GridNode { Xposition = 13, Yposition = 1, },
            new GridNode { Xposition = 13, Yposition = 0, },

            new GridNode { Xposition = 7, Yposition = 16, },
            new GridNode { Xposition = 7, Yposition = 17, },
            new GridNode { Xposition = 8, Yposition = 18, },
            new GridNode { Xposition = 7, Yposition = 19, },
            new GridNode { Xposition = 7, Yposition = 20, },
            new GridNode { Xposition = 7, Yposition = 21, },
            new GridNode { Xposition = 8, Yposition = 21, },
            new GridNode { Xposition = 9, Yposition = 22, },
            new GridNode { Xposition = 9, Yposition = 16, },
            new GridNode { Xposition = 10, Yposition = 16, },
            new GridNode { Xposition = 11, Yposition = 16, },
            new GridNode { Xposition = 12, Yposition = 16, },
            new GridNode { Xposition = 13, Yposition = 17, },
            new GridNode { Xposition = 14, Yposition = 16, },
            new GridNode { Xposition = 14, Yposition = 17, },
            new GridNode { Xposition = 13, Yposition = 18, },
            new GridNode { Xposition = 14, Yposition = 19, },
            new GridNode { Xposition = 14, Yposition = 20, },
            new GridNode { Xposition = 14, Yposition = 21, },
            new GridNode { Xposition = 13, Yposition = 21, },
            new GridNode { Xposition = 12, Yposition = 22, },

            new GridNode { Xposition = 16, Yposition = 0, },
            new GridNode { Xposition = 16, Yposition = 1, },
            new GridNode { Xposition = 16, Yposition = 2, },
            new GridNode { Xposition = 16, Yposition = 3, },
            new GridNode { Xposition = 17, Yposition = 4, },
            new GridNode { Xposition = 18, Yposition = 4, },
            new GridNode { Xposition = 19, Yposition = 4, },
            new GridNode { Xposition = 20, Yposition = 4, },
            new GridNode { Xposition = 21, Yposition = 4, },


            new GridNode { Xposition = 15, Yposition = 8, },
            new GridNode { Xposition = 15, Yposition = 9, },
            new GridNode { Xposition = 15, Yposition = 10, },
            new GridNode { Xposition = 16, Yposition = 11, },
            new GridNode { Xposition = 15, Yposition = 12, },
            new GridNode { Xposition = 15, Yposition = 13, },
            new GridNode { Xposition = 16, Yposition = 13, },
            new GridNode { Xposition = 17, Yposition = 13, },
            new GridNode { Xposition = 18, Yposition = 14, },
            new GridNode { Xposition = 19, Yposition = 14, },
            new GridNode { Xposition = 20, Yposition = 14, },
            new GridNode { Xposition = 21, Yposition = 14, },
            new GridNode { Xposition = 16, Yposition = 9, },
            new GridNode { Xposition = 17, Yposition = 8, },
            new GridNode { Xposition = 18, Yposition = 8, },
            new GridNode { Xposition = 19, Yposition = 8, },
            new GridNode { Xposition = 20, Yposition = 8, },
            new GridNode { Xposition = 21, Yposition = 8, },

            new GridNode { Xposition = 21, Yposition = 17, },
            new GridNode { Xposition = 20, Yposition = 17, },
            new GridNode { Xposition = 19, Yposition = 17, },
            new GridNode { Xposition = 17, Yposition = 18, },
            new GridNode { Xposition = 17, Yposition = 19, },
            new GridNode { Xposition = 17, Yposition = 20, },
            new GridNode { Xposition = 17, Yposition = 21, },
            new GridNode { Xposition = 17, Yposition = 17, },
            new GridNode { Xposition = 18, Yposition = 18, },

            new GridNode { Xposition = -1, Yposition = 17, },
            new GridNode { Xposition = -1, Yposition = 16, },
            new GridNode { Xposition = -1, Yposition = 10, },
            new GridNode { Xposition = -1, Yposition = 4, },
            new GridNode { Xposition = -1, Yposition = 3, },
            new GridNode { Xposition = 6, Yposition = -1, },
            new GridNode { Xposition = 7, Yposition = -1, },
            new GridNode { Xposition = 14, Yposition = -1, },
            new GridNode { Xposition = 15, Yposition = -1, },
            new GridNode { Xposition = 22, Yposition = 5, },
            new GridNode { Xposition = 22, Yposition = 6, },
            new GridNode { Xposition = 22, Yposition = 7, },
            new GridNode { Xposition = 22, Yposition = 15, },
            new GridNode { Xposition = 22, Yposition = 16, },
            new GridNode { Xposition = 16, Yposition = 22, },
            new GridNode { Xposition = 15, Yposition = 23, },
            new GridNode { Xposition = 14, Yposition = 23, },
            new GridNode { Xposition = 13, Yposition = 23, },
            new GridNode { Xposition = 8, Yposition = 23, },
            new GridNode { Xposition = 7, Yposition = 23, },
            new GridNode { Xposition = 6, Yposition = 23, },
            new GridNode { Xposition = 5, Yposition = 21, },
        };

        public List<Door> Doors { get; set; } = new List<Door>
        {
            new Door { Xposition = 5, Yposition = 2, ZoneName = "place6.png" },
            new Door { Xposition = 5, Yposition = 7, ZoneName = "place1.png" },
            new Door { Xposition = 2, Yposition = 9, ZoneName = "place1.png" },
            new Door { Xposition = 4, Yposition = 14, ZoneName = "place8.png" },
            new Door { Xposition = 3, Yposition = 18, ZoneName = "place7.png" },
            new Door { Xposition = 8, Yposition = 3, ZoneName = "place9.png" },
            new Door { Xposition = 10, Yposition = 5, ZoneName = "place9.png" },
            new Door { Xposition = 11, Yposition = 5, ZoneName = "place9.png" },
            new Door { Xposition = 8, Yposition = 16, ZoneName = "place2.png" },
            new Door { Xposition = 13, Yposition = 16, ZoneName = "place2.png" },
            new Door { Xposition = 7, Yposition = 18, ZoneName = "place2.png" },
            new Door { Xposition = 14, Yposition = 18, ZoneName = "place2.png" },
            new Door { Xposition = 16, Yposition = 4, ZoneName = "place4.png" },
            new Door { Xposition = 16, Yposition = 8, ZoneName = "place5.png" },
            new Door { Xposition = 15, Yposition = 11, ZoneName = "place5.png" },
            new Door { Xposition = 18, Yposition = 17, ZoneName = "place3.png" }
        };

    }
}