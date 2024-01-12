using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace GameService.Contracts
{
    /// <summary>
    /// Service contract for managing user-related operations.
    /// </summary>
    [ServiceContract]
    public interface IUserManager
    {
        /// <summary>
        /// Adds a user transaction and returns the result.
        /// </summary>
        [OperationContract]
        int AddUserTransaction(Gamer gamer);

        /// <summary>
        /// Authenticates an account using the provided gamertag and password.
        /// </summary>
        [OperationContract]
        bool AuthenticateAccount(string gamertag, string password);

        /// <summary>
        /// Requests a guest player and returns the generated code.
        /// </summary>
        [OperationContract]
        string RequestGuestPlayer();

        /// <summary>
        /// Checks if an email already exists in the system.
        /// </summary>
        [OperationContract]
        bool IsEmailExisting(String email);

        /// <summary>
        /// Updates the password for the specified gamertag.
        /// </summary>
        [OperationContract]
        int UpdatePassword(String gamertag, String password);

        /// <summary>
        /// Checks if a gamertag already exists in the system.
        /// </summary>
        [OperationContract]
        bool IsGamertagExisting(String gamertag);

        /// <summary>
        /// Retrieves a gamer profile by gamertag.
        /// </summary>
        [OperationContract]
        Gamer GetGamerByGamertag(string gamertag);

        /// <summary>
        /// Retrieves a gamer profile by email.
        /// </summary>
        [OperationContract]
        Gamer GetGamerByEmail(string email);

        /// <summary>
        /// Modifies the account details for the specified gamertag.
        /// </summary>
        [OperationContract]
        int ModifyAccount(String gamertag, String firstName, String lastName);

        /// <summary>
        /// Changes the icon associated with the specified gamertag.
        /// </summary>
        [OperationContract]
        int ChangeIcon(string gamertag, String titleIcon);

        /// <summary>
        /// Retrieves the icon associated with the specified gamertag.
        /// </summary>
        [OperationContract]
        string GetIcon(String gamertag);

        /// <summary>
        /// Deletes a guest player profile by gamertag.
        /// </summary>
        [OperationContract]
        int DeleteGuestPlayer(string gamertag);
    }

    /// <summary>
    /// Represents a gamer with additional properties.
    /// </summary>
    [DataContract]
    public class Gamer : AccessAccount
    {
        /// <summary>
        /// Gets or sets the first name of the gamer.
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the gamer.
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the number of games won by the gamer.
        /// </summary>
        [DataMember]
        public int GamesWon { get; set; }

        /// <summary>
        /// Gets or sets the image code associated with the gamer.
        /// </summary>
        [DataMember]
        public string ImageCode { get; set; }
    }

    /// <summary>
    /// Represents an access account with basic properties.
    /// </summary>
    [DataContract]
    public class AccessAccount
    {
        /// <summary>
        /// Gets or sets the email associated with the access account.
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the gamertag associated with the access account.
        /// </summary>
        [DataMember]
        public string Gamertag { get; set; }

        /// <summary>
        /// Gets or sets the password associated with the access account.
        /// </summary>
        [DataMember]
        public string Password { get; set; }
    }
}
