using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;


namespace GameService.Contracts
{
    [ServiceContract]
    public interface IWinnersManager
    {
        [OperationContract]
        List<Winner> GetTopGlobalWinners();
    }

    [DataContract]
    public class Winner
    {
        private string icon;
        private string gamertag;
        private int gamesWon;

        [DataMember]
        public string Icon { get { return icon; } set { icon = value; } }

        [DataMember]
        public string Gamertag { get { return gamertag; } set { gamertag = value; } }

        [DataMember]
        public int GamesWon { get { return gamesWon; } set { gamesWon = value; } }
    }
}
