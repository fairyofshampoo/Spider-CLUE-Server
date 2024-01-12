using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;


namespace GameService.Contracts
{
    /// <summary>
    /// Represents the contract for managing winners in a game service.
    /// </summary>
    [ServiceContract]
    public interface IWinnersManager
    {
        /// <summary>
        /// Retrieves a list of top global winners.
        /// </summary>
        /// <returns>List of Winner objects representing top global winners.</returns>
        [OperationContract]
        List<Winner> GetTopGlobalWinners();
    }

    /// <summary>
    /// Represents information about a winner, including icon, gamertag, and games won.
    /// </summary>
    [DataContract]
    public class Winner
    {
        /// <summary>
        /// Gets or sets the icon associated with the winner.
        /// </summary>
        [DataMember]
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the gamertag of the winner.
        /// </summary>
        [DataMember]
        public string Gamertag { get; set; }

        /// <summary>
        /// Gets or sets the number of games won by the winner.
        /// </summary>
        [DataMember]
        public int GamesWon { get; set; }
    }

}
