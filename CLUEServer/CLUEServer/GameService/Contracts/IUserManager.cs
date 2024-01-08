using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace GameService.Contracts
{

    [ServiceContract]
    public interface IUserManager
    {
        [OperationContract]
        int AddUserTransaction(Gamer gamer);

        [OperationContract]
        bool AuthenticateAccount(string gamertag, string password);

        [OperationContract]
        string RequestGuestPlayer();

        [OperationContract]
        bool IsEmailExisting(String email);

        [OperationContract]
        int UpdatePassword(String gamertag, String password);

        [OperationContract]
        bool IsGamertagExisting(String gamertag);

        [OperationContract]
        Gamer GetGamerByGamertag(string gamertag);

        [OperationContract]
        Gamer GetGamerByEmail(string email);

        [OperationContract]
        int ModifyAccount(String gamertag, String firstName, String lastName);

        [OperationContract]
        int ChangeIcon(string gamertag, String titleIcon);

        [OperationContract]
        string GetIcon(String gamertag);

        [OperationContract]
        int DeleteGuestPlayer(string gamertag);
    }

    [DataContract]
    public class Gamer : AccessAccount
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public int GamesWon { get; set; }

        [DataMember]
        public string ImageCode { get; set; }
    }

    [DataContract]
    public class AccessAccount
    {

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Gamertag { get; set; }

        [DataMember]
        public string Password { get; set; }

    }
}
