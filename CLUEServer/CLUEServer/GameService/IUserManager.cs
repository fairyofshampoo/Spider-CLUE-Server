using DataBaseManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService
{

    [ServiceContract]
    interface IUserManager
    {
        [OperationContract]
        int AddUserTransaction(Gamer gamer);

        [OperationContract]
        bool IsAccountExisting(string email);

        [OperationContract]
        bool AuthenticateAccount(string gamertag, string password);

        [OperationContract]
        string RequestGuessPlayer();

        [OperationContract]
        int AuthenticateGamertag(String gamertag);

        [OperationContract]
        int AuthenticateEmail(String email);

        [OperationContract]
        Boolean IsAccessAccountExisting (String user, String Password);

        [OperationContract]
        Boolean isEmailExisting(String email);
        
        [OperationContract]
        Boolean isGamertagExisting (String gamertag);
    }


    [DataContract]
    public class Gamer : AccessAccount
    {
        private string firstName;
        private string lastName;
        private int level;

        [DataMember]
        public string FirstName { get { return firstName; } set { firstName = value; } }

        [DataMember]
        public string LastName { get { return lastName; } set { lastName = value; } }

        [DataMember]
        public int Level { get { return level; } set { level = value; } }
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
