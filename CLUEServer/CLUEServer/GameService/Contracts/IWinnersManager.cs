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
        [DataMember]
        public string Icon { get; set; }

        [DataMember]
        public string Gamertag { get; set; }

        [DataMember]
        public int GamesWon { get; set; }
    }
}
