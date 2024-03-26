using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace GameService.Contracts
{
    /// <summary>
    /// Represents the contract for managing matches in a game service.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IMatchManagerCallback))]
    public interface IMatchManager
    {
        /// <summary>
        /// Retrieves information about a match based on the provided code.
        /// </summary>
        /// <param name="code">The code of the match.</param>
        /// <returns>Match object containing information about the match.</returns>
        [OperationContract]
        Match GetMatchInformation(string code);

        /// <summary>
        /// Connects a gamer to a match using their gamertag and the match code.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer.</param>
        /// <param name="matchCode">The code of the match.</param>
        [OperationContract(IsOneWay = true)]
        void ConnectToMatch(string gamertag, string matchCode);

        /// <summary>
        /// Allows a gamer to leave a match using their gamertag and the match code.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer.</param>
        /// <param name="matchCode">The code of the match.</param>
        [OperationContract]
        void LeaveMatch(string gamertag, string matchCode);

        /// <summary>
        /// Retrieves the list of gamers in a match based on the provided gamertag and match code.
        /// </summary>
        /// <param name="gamertag">The gamertag of the requesting gamer.</param>
        /// <param name="code">The code of the match.</param>
        [OperationContract(IsOneWay = true)]
        void GetGamersInMatch(string gamertag, string code);
    }

    /// <summary>
    /// Represents the callback contract for communication from the server to the client.
    /// </summary>
    [ServiceContract]
    public interface IMatchManagerCallback
    {
        /// <summary>
        /// Notifies the client about the gamers present in a match.
        /// </summary>
        /// <param name="characters">Dictionary of gamertags and associated Pawn objects.</param>
        [OperationContract]
        void ReceiveGamersInMatch(Dictionary<string, Pawn> characters);
    }

    /// <summary>
    /// Represents information about a match, including its code and creator's gamertag.
    /// </summary>
    [DataContract]
    public class Match
    {
        /// <summary>
        /// Gets or sets the code associated with the match.
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the gamertag of the gamer who created the match.
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }
    }
}
