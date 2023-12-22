using GameService.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Services
{
    public partial class GameService : ICardManager
    {
        List<Card> FirstDeck;
        List<Card> SecondDeck;
        List<Card> ThirdDeck;
        List<Card> clueDeck;

        Dictionary <string, List<Card>> decks = new Dictionary<string, List<Card>>();
        public void ChooseCards()
        {
            Random random = new Random();
            int cardChoosed = random.Next(0, 7);

            Card characterCard = FirstDeck[cardChoosed];
            FirstDeck.RemoveAt(cardChoosed);
            clueDeck.Add(characterCard);

            cardChoosed = random.Next(0, 10);
            Card placeCard = SecondDeck[cardChoosed];
            SecondDeck.RemoveAt(cardChoosed);
            clueDeck.Add(placeCard);

            cardChoosed = random.Next(0, 7);
            Card motiveCard = ThirdDeck[cardChoosed];
            ThirdDeck.RemoveAt(cardChoosed);
            clueDeck.Add(motiveCard);
        }

        public void CreatCards(string gamer1, string gamer2, string gamer3)
        {
            CreateCharacterCards();
            CreatePlaceCards();
            CreateMotiveCards();

            ChooseCards();
            DealCards(gamer1, gamer2, gamer3, ShuffleCards());
        }

        public void CreateCharacterCards()
        {
            Card card1 =  new Card { ID = "Character1", Type = "Character"};
            Card card2 =  new Card { ID = "Character2", Type = "Character" };
            Card card3 =  new Card { ID = "Character3", Type = "Character" };
            Card card4 =  new Card { ID = "Character4", Type = "Character" };
            Card card5 =  new Card { ID = "Character5", Type = "Character" };
            Card card6 =  new Card { ID = "Character6", Type = "Character" };

            FirstDeck.Add(card1);
            FirstDeck.Add(card2);
            FirstDeck.Add(card3);
            FirstDeck.Add(card4);
            FirstDeck.Add(card5);
            FirstDeck.Add(card6);
        }   

        public void CreatePlaceCards()
        {
            Card card7 = new Card { ID = "place1", Type = "Place" };
            Card card8 = new Card { ID = "place2", Type = "Place" };
            Card card9 = new Card { ID = "place3", Type = "Place" };
            Card card10 = new Card { ID = "place4", Type = "Place" };
            Card card11 = new Card { ID = "place5", Type = "Place" };
            Card card12 = new Card { ID = "place6", Type = "Place" };
            Card card13 = new Card { ID = "place7", Type = "Place" };
            Card card14 = new Card { ID = "place8", Type = "Place" };
            Card card15 = new Card { ID = "place9", Type = "Place" };

            SecondDeck.Add(card7);
            SecondDeck.Add(card8);
            SecondDeck.Add(card9);
            SecondDeck.Add(card10);
            SecondDeck.Add(card11);
            SecondDeck.Add(card12);
            SecondDeck.Add(card13);
            SecondDeck.Add(card14);
            SecondDeck.Add(card15);
        }

        public void CreateMotiveCards()
        {
            Card card16 = new Card { ID = "motive1", Type = "Motive" };
            Card card17 = new Card { ID = "motive2", Type = "Motive" };
            Card card18 = new Card { ID = "motive3", Type = "Motive" };
            Card card19 = new Card { ID = "motive4", Type = "Motive" };
            Card card20 = new Card { ID = "motive5", Type = "Motive" };
            Card card21 = new Card { ID = "motive6", Type = "Motive" };

            ThirdDeck.Add(card16);
            ThirdDeck.Add(card17);
            ThirdDeck.Add(card18);
            ThirdDeck.Add(card19);
            ThirdDeck.Add(card20);
            ThirdDeck.Add(card21);
        }

        public void DealCards(string gamer1, string gamer2, string gamer3, List<Card> cards)
        {
            FirstDeck.Clear();
            SecondDeck.Clear();
            ThirdDeck.Clear();

            FirstDeck = cards.GetRange(0, 6);
            SecondDeck = cards.GetRange(6, 6);
            ThirdDeck = cards.GetRange(12, 6);

            decks.Add(gamer1, FirstDeck);
            decks.Add(gamer2, SecondDeck);
            decks.Add(gamer3, ThirdDeck);
        }

        public List<Card> ShuffleCards()
        {
            List<Card> cards =  FirstDeck.Concat(SecondDeck).Concat(ThirdDeck).ToList();
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
