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
        public void ChooseCards(List<Card> first, List<Card> second, List<Card> third)
        {
            throw new NotImplementedException();
        }

        public void CreatCards()
        {
            throw new NotImplementedException();
        }

        public void CreateCharacterCards()
        {
            Card card1 =  new Card { ID = "Character1", Name = "Doc Och", Type = "Character"};
            Card card2 =  new Card { ID = "Character2", Name = "MysteRevo", Type = "Character" };
            Card card3 =  new Card { ID = "Character3", Name = "RhinoSidro", Type = "Character" };
            Card card4 =  new Card { ID = "Character4", Name = "Electro-Juan", Type = "Character" };
            Card card5 =  new Card { ID = "Character5", Name = "Xand-Man", Type = "Character" };
            Card card6 =  new Card { ID = "Character6", Name = "Vernom", Type = "Character" };
        }

        public void CreatePlaceCards()
        {
            Card card7 = new Card { ID = "Character1", Name = "Doc Och", Type = "Character" };
            Card card8 = new Card { ID = "Character1", Name = "Doc Och", Type = "Character" };
            Card card9 = new Card { ID = "Character1", Name = "Doc Och", Type = "Character" };
            Card card10 = new Card { ID = "Character1", Name = "Doc Och", Type = "Character" };
            Card card11 = new Card { ID = "Character1", Name = "Doc Och", Type = "Character" };
            Card card12 = new Card { ID = "Character1", Name = "Doc Och", Type = "Character" };
            Card card13 = new Card { ID = "Character1", Name = "Doc Och", Type = "Character" };
            Card card14 = new Card { ID = "Character1", Name = "Doc Och", Type = "Character" };
            Card card15 = new Card { ID = "Character1", Name = "Doc Och", Type = "Character" };
        }

        public void CreateMotiveCards()
        {

        }

        public void DealCards(string gamer1, string gamer2, string gamer3)
        {
            throw new NotImplementedException();
        }

        public void ShuffleCards(List<Card> cards)
        {
            throw new NotImplementedException();
        }
    }
}
