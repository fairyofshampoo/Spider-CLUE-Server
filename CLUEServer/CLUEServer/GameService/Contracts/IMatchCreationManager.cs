using System.ServiceModel;

namespace GameService.Contracts
{
    /// <summary>
    /// Service contract for managing the creation of matches in a gaming system.
    /// </summary>
    [ServiceContract]
    public interface IMatchCreationManager
    {
        /// <summary>
        /// Creates a match and returns the match code for the specified player with the given gamertag.
        /// </summary>
        /// <param name="gamertag">The gamertag of the player creating the match.</param>
        /// <returns>The unique code identifying the created match.</returns>
        [OperationContract]
        string CreateMatch(string gamertag);
    }
}
