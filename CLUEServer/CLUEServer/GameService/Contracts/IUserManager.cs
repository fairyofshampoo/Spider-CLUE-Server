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
        bool IsEmailExisting(string email);

        [OperationContract]
        int UpdatePassword(string gamertag,  string password);

        [OperationContract]
        bool IsGamertagExisting (string gamertag);

        [OperationContract]
        Gamer GetGamerByGamertag(string gamertag);

        [OperationContract]
        Gamer GetGamerByEmail(string email);

        [OperationContract]
        int ModifyAccount (string gamertag, string firstName, string lastName);
        
        [OperationContract]
        int ChangeIcon (string gamertag, string titleIcon);

        [OperationContract]
        string GetIcon (string gamertag);

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
