using GameService.Contracts;
using System.IO;

namespace GameService.Services
{
    public partial class GameService : IInvitationManager
    {
        public bool SendInvitation(string email, string matchCode, string gamertag)
        {
            EmailService emailService = new EmailService();
            return emailService.SendEmailWithInvitation(email, matchCode, gamertag);
        }
    }
}
