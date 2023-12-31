﻿using System;
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
        void CreatCards(string gamer1, string gamer2, string gamer3);

        [OperationContract]
        List<Card> ChooseCards(List<Card> firstDeck, List<Card> secondDeck, List<Card> thirdDeck);

        [OperationContract]
        List<Card> ShuffleCards(List<Card> cards);

        [OperationContract]
        void DealCards(string gamer1, string gamer2, string gamer3, List<Card> card);

        [OperationContract]
        List<Card> GetDeck(string gamertag);

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
