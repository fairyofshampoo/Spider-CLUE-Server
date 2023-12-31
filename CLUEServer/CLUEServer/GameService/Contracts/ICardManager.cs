using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Contracts
{
    [ServiceContract]
    internal interface ICardManager
    {
        [OperationContract]
        void CreatCards(string matchCode);

        [OperationContract]
        List<Card> ChooseCards(List<Card> firstDeck, List<Card> secondDeck, List<Card> thirdDeck, string matchCode);

        [OperationContract]
        List<Card> ShuffleCards(List<Card> cards);

        [OperationContract]
        void DealCards(List<string> gamers, List<Card> card);

    }

    [DataContract]
    public class Card
    {
        private string id;
        private string type;

        [DataMember]
        public string ID { get { return id; } set { id = value; } }

        [DataMember]
        public string Type { get { return type; } set { type = value; } }
    }
}
