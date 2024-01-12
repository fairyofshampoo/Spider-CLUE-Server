using System.ServiceModel;

namespace GameService.Contracts
{
    /// <summary>
    /// Service contract for managing invitations in a gaming system.
    /// </summary>
    [ServiceContract]
    interface IInvitationManager
    {
        /// <summary>
        /// Sends an invitation to a specified email address for joining a match with a given match code and gamertag.
        /// </summary>
        /// <param name="email">The email address to which the invitation will be sent.</param>
        /// <param name="matchCode">The code identifying the match associated with the invitation.</param>
        /// <param name="gamertag">The gamertag of the user sending the invitation.</param>
        /// <returns>True if the invitation is successfully sent; otherwise, false.</returns>
        [OperationContract]
        bool SendInvitation(string email, string matchCode, string gamertag);
    }
}
