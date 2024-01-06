﻿using GameService.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Services
{
    public partial class GameService
    {
        private static readonly Dictionary<string, List<Card>> decks = new Dictionary<string, List<Card>>();
        private static readonly Dictionary<string, List<Card>> clueDeckByMatch = new Dictionary<string, List<Card>>();

        private List<Card> ChooseCards(List<Card> firstDeck, List<Card> secondDeck, List<Card> thirdDeck, string matchCode)
        {
            List<Card> clueDeck = new List<Card>();

            Random random = new Random();
            int cardChoosed = random.Next(0, 6);

            Card characterCard = firstDeck[cardChoosed];
            firstDeck.RemoveAt(cardChoosed);
            clueDeck.Add(characterCard);

            cardChoosed = random.Next(0, 9);
            Card placeCard = secondDeck[cardChoosed];
            secondDeck.RemoveAt(cardChoosed);
            clueDeck.Add(placeCard);

            cardChoosed = random.Next(0, 6);
            Card motiveCard = thirdDeck[cardChoosed];
            thirdDeck.RemoveAt(cardChoosed);
            clueDeck.Add(motiveCard);

            clueDeckByMatch.Add(matchCode, clueDeck);
            List<Card> cards = firstDeck.Concat(secondDeck).Concat(thirdDeck).ToList();
            return cards;
        }

        private void CreateCards(string matchCode)
        {
            List<string> gamerByBoard = GetGamersByGameBoard(matchCode);
            List<Card> firstDeck = CreateCharacterCards();
            List<Card> secondDeck = CreatePlaceCards();
            List<Card> thirdDeck = CreateMotiveCards();
            List<Card> cards = ChooseCards(firstDeck, secondDeck, thirdDeck, matchCode);

            DealCards(gamerByBoard, ShuffleCards(cards));
        }

        private List<Card> CreateCharacterCards()
        {
            Card card1 =  new Card { ID = "DocOchCard.png", Type = "Character"};
            Card card2 =  new Card { ID = "ElectroJuanCard.png", Type = "Character" };
            Card card3 =  new Card { ID = "MysteRevoCard.png", Type = "Character" };
            Card card4 =  new Card { ID = "RhinosidroCard.png", Type = "Character" };
            Card card5 =  new Card { ID = "VernomCard.png", Type = "Character" };
            Card card6 =  new Card { ID = "XandManCard.png", Type = "Character" };

            List<Card> firstDeck = new List<Card>();
            firstDeck.Add(card1);
            firstDeck.Add(card2);
            firstDeck.Add(card3);
            firstDeck.Add(card4);
            firstDeck.Add(card5);
            firstDeck.Add(card6);
            return firstDeck;
        }   
        private List<Card> CreatePlaceCards()
        {
            Card card7 = new Card { ID = "place1.png", Type = "Place" };
            Card card8 = new Card { ID = "place2.png", Type = "Place" };
            Card card9 = new Card { ID = "place3.png", Type = "Place" };
            Card card10 = new Card { ID = "place4.png", Type = "Place" };
            Card card11 = new Card { ID = "place5.png", Type = "Place" };
            Card card12 = new Card { ID = "place6.png", Type = "Place" };
            Card card13 = new Card { ID = "place7.png", Type = "Place" };
            Card card14 = new Card { ID = "place8.png", Type = "Place" };
            Card card15 = new Card { ID = "place9.png", Type = "Place" };

            List<Card> secondDeck = new List<Card>();
            secondDeck.Add(card7);
            secondDeck.Add(card8);
            secondDeck.Add(card9);
            secondDeck.Add(card10);
            secondDeck.Add(card11);
            secondDeck.Add(card12);
            secondDeck.Add(card13);
            secondDeck.Add(card14);
            secondDeck.Add(card15);
            return secondDeck;
        }

        private List<Card> CreateMotiveCards()
        {
            Card card16 = new Card { ID = "motive1.png", Type = "Motive" };
            Card card17 = new Card { ID = "motive2.png", Type = "Motive" };
            Card card18 = new Card { ID = "motive3.png", Type = "Motive" };
            Card card19 = new Card { ID = "motive4.png", Type = "Motive" };
            Card card20 = new Card { ID = "motive5.png", Type = "Motive" };
            Card card21 = new Card { ID = "motive6.png", Type = "Motive" };

            List<Card> thirdDeck = new List<Card>();
            thirdDeck.Add(card16);
            thirdDeck.Add(card17);
            thirdDeck.Add(card18);
            thirdDeck.Add(card19);
            thirdDeck.Add(card20);
            thirdDeck.Add(card21);
            return thirdDeck;
        }

        private void DealCards(List<string> gamers, List<Card> cards)
        {
            List<Card> firstDeck = cards.GetRange(0, 6);
            List<Card> secondDeck = cards.GetRange(6, 6);
            List<Card> thirdDeck = cards.GetRange(12, 6);
            

            decks.Add(gamers[0], firstDeck);
            decks.Add(gamers[1], secondDeck);
            decks.Add(gamers[2], thirdDeck);
        }

        private List<Card> ShuffleCards(List<Card> cards)
        {
            Random random = new Random();
            int index = cards.Count;    

            while (index > 1) 
            { 
                index--;
                int shuffle = random.Next(index + 1);
                Card selectCard = cards[shuffle];
                cards[shuffle] = cards[index];
                cards[index] = selectCard;
            }
            return cards;
        }

    }
}
