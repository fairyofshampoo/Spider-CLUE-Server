using System.ServiceModel;

namespace GameService.Contracts
{
    [ServiceContract]
    interface IInvitationManager
    {
        [OperationContract]
        bool SendInvitation(string email, string matchCode, string gamertag);
    }
}