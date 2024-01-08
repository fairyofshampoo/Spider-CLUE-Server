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
    public class Gamer : AccessAccount, IEquatable<Gamer>
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public int GamesWon { get; set; }

        [DataMember]
        public string ImageCode { get; set; }

        public bool Equals(Gamer other)
        {
            if (other == null)
                return false;

            // Comparar solo los atributos relevantes para tu prueba
            return base.Equals(other) &&
                   FirstName == other.FirstName &&
                   LastName == other.LastName &&
                   GamesWon == other.GamesWon &&
                   ImageCode == other.ImageCode;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Gamer);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + base.GetHashCode();
                hash = hash * 23 + (FirstName?.GetHashCode() ?? 0);
                hash = hash * 23 + (LastName?.GetHashCode() ?? 0);
                hash = hash * 23 + GamesWon.GetHashCode();
                hash = hash * 23 + (ImageCode?.GetHashCode() ?? 0);
                return hash;
            }
        }
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
