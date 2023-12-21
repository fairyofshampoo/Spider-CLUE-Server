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
        void CreatCards();

        [OperationContract]
        void ChooseCards(List<Card> first, List<Card> second, List<Card> third);

        [OperationContract]
        void ShuffleCards(List<Card> cards);

        [OperationContract]
        void DealCards(string gamer1, string gamer2, string gamer3);
    }

    [DataContract]
    public class Card
    {
        private string id;
        private string type;
        private string name;

        [DataMember]
        public string ID { get { return id; } set { id = value; } }

        [DataMember]
        public string Type { get { return type; } set { type = value; } }

        [DataMember]
        public string Name { get { return name; } set { name = value; } }
    }
}
