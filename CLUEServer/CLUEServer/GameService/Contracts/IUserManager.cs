using DataBaseManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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
        int UpdatePassword(String gamertag,  String password);

        [OperationContract]
        bool IsGamertagExisting (String gamertag);

        [OperationContract]
        Gamer GetGamerByGamertag(string gamertag);

        [OperationContract]
        Gamer GetGamerByEmail(string gamertag);

        [OperationContract]
        int ModifyAccount (String gamertag, String firstName, String lastName);
        
        [OperationContract]
        int ChangeIcon (string gamertag, String titleIcon);

        [OperationContract]
        string GetIcon (String gamertag);
    }

    [DataContract]
    public class Gamer : AccessAccount
    {
        private string firstName;
        private string lastName;
        private int level;
        private string imageCode;


        [DataMember]
        public string FirstName { get { return firstName; } set { firstName = value; } }

        [DataMember]
        public string LastName { get { return lastName; } set { lastName = value; } }

        [DataMember]
        public int Level { get { return level; } set { level = value; } }

        [DataMember]
        public string ImageCode { get { return imageCode; } set { imageCode = value; } }

    }

    [DataContract]
    public class AccessAccount
    {
        private string email;
        private string gamertag;
        private string password;
        private int isBanned;

        [DataMember]
        public string Email { get { return email; } set { email = value; } }

        [DataMember]
        public string Gamertag { get { return gamertag; } set { gamertag = value; } }

        [DataMember]
        public string Password { get { return password; } set { password = value; } }

        [DataMember]
        public int IsBanned { get { return isBanned; } set { isBanned = value; } }

    }
}
