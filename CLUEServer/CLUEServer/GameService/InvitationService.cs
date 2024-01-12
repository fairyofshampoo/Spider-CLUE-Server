using GameService.Contracts;

namespace GameService.Services
{
    public partial class GameService : IInvitationManager
    {
        /// <summary>
        /// Sends an invitation email to a specified email address for joining a game match.
        /// </summary>
        /// <param name="email">Email address of the recipient.</param>
        /// <param name="matchCode">Code of the game match being joined.</param>
        /// <param name="gamertag">Gamertag of the user sending the invitation.</param>
        /// <returns>True if the invitation email is successfully sent; otherwise, false.</returns>

        public bool SendInvitation(string email, string matchCode, string gamertag)
        {
            EmailService emailService = new EmailService();
            return emailService.SendEmailWithInvitation(email, matchCode, gamertag);
        }
    }
}
