using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseManager;
using BCrypt;
using GameService.Contracts;
using System.Security.Cryptography;
using System.ServiceModel.Configuration;
using System.Data.SqlClient;
using System.ServiceModel;
using System.Diagnostics;
using GameService.Utilities;
using System.Data.Entity.Core;
using System.Data;
using log4net.Repository.Hierarchy;
using System.Data.Entity.Infrastructure;

namespace GameService.Services
{
    
    public partial class GameService : IUserManager
    {

        /// <summary>
        /// Adds a new gamer and access account to the database within a transaction.
        /// </summary>
        /// <param name="gamer">The gamer object containing user details to be added.</param>
        /// <returns>
        /// Returns Constants.SuccessInOperation (1) if the operation is successful,
        /// Constants.ErrorInOperation (0) if an error occurs during the operation.
        /// </returns>
        public int AddUserTransaction(Gamer gamer)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            int result = Constants.ErrorInOperation;
            try
            {
                using (var dataBaseContext = new SpiderClueDbEntities())
                {
                    using (var dataBaseContextTransaction = dataBaseContext.Database.BeginTransaction())
                    {
                        try
                        {
                            var newAccessAccount = new DataBaseManager.accessAccount
                            {
                                password = gamer.Password,
                                gamertag = gamer.Gamertag,
                                email = gamer.Email,
                            };

                            var newGamer = new DataBaseManager.gamer
                            {
                                firstName = gamer.FirstName,
                                lastName = gamer.LastName,
                                gamertag = gamer.Gamertag,
                                gamesWon = gamer.GamesWon,
                                imageCode = gamer.ImageCode
                            };

                            dataBaseContext.accessAccounts.Add(newAccessAccount);
                            dataBaseContext.gamers.Add(newGamer);

                            dataBaseContext.SaveChanges();
                            dataBaseContextTransaction.Commit();

                            result = Constants.SuccessInOperation;
                        }
                        catch (DbUpdateException updateException)
                        {
                            logger.LogError(updateException);
                            dataBaseContextTransaction.Rollback();
                            result = Constants.ErrorInOperation;
                        }
                        catch (SqlException sQLException)
                        {
                            logger.LogError(sQLException);
                            dataBaseContextTransaction.Rollback();
                            result = Constants.ErrorInOperation;
                        }
                        
                    }
                }
            }
            catch (EntityException entityException)
            {
                logger.LogError(entityException);
            }

            return result;
        }

        /// <summary>
        /// Authenticates a user account by verifying the provided gamertag and password.
        /// </summary>
        /// <param name="gamertag">The gamertag of the user account to be authenticated.</param>
        /// <param name="password">The password associated with the user account.</param>
        /// <returns>
        /// Returns true if the provided gamertag and password match an existing account,
        /// otherwise returns false. Returns false in case of errors during authentication.
        /// </returns>
        public bool AuthenticateAccount(string gamertag, string password)
        {
            bool result = false;
            LoggerManager logger = new LoggerManager(this.GetType());
            try
            {
                using (var context = new SpiderClueDbEntities())
                {
                    HostBehaviorManager.ChangeToSingle();
                    var existingAccount = context.accessAccounts.FirstOrDefault(accessAccount => accessAccount.gamertag == gamertag);
                    result = existingAccount != null && existingAccount.password == password;
                }
            }
            catch (EntityException entityException)
            {
                logger.LogError(entityException);
                result = false;
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Requests a unique guest player gamertag by generating a random username
        /// and ensuring it does not already exist. Creates a guest gamer with the
        /// generated gamertag before returning it.
        /// </summary>
        /// <returns>The unique guest player gamertag.</returns>
        public string RequestGuestPlayer()
        {
            HostBehaviorManager.ChangeToSingle();
            string guestGamertag;

            do
            {
                guestGamertag = CreateRandomUserName();

            } while (IsGamertagExisting(guestGamertag));

            CreateGuestGamer(guestGamertag);

            return guestGamertag;
        }

        /// <summary>
        /// Creates a guest gamer with the provided gamertag, assigning default values
        /// for the guest's name, games won, and image code. The operation is performed
        /// within a transaction for data consistency.
        /// </summary>
        /// <param name="gamertag">The gamertag for the guest gamer.</param>
        /// <returns>An integer indicating the result of the operation (success or error).</returns>
        public int CreateGuestGamer(string gamertag)
        {
            HostBehaviorManager.ChangeToSingle();
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            int result = Constants.ErrorInOperation;

            using (var dbContext = new SpiderClueDbEntities())
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var newGamer = new DataBaseManager.gamer
                        {
                            firstName = Constants.DefaultGuestName,
                            lastName = Constants.DefaultLastName,
                            gamertag = gamertag,
                            gamesWon = Constants.DefaultGamesWon,
                            imageCode = Constants.DefaultIcon
                        };

                        dbContext.gamers.Add(newGamer);

                        dbContext.SaveChanges();

                        transaction.Commit();
                        result = Constants.SuccessInOperation;
                    }
                    catch (SqlException sQLException)
                    {
                        transaction.Rollback();
                        loggerManager.LogError(sQLException);
                        result = Constants.ErrorInOperation;
                    }
                    catch (EntityException ex)
                    {
                        transaction.Rollback();
                        loggerManager.LogError(ex);
                        result = Constants.ErrorInOperation;
                    }
                    catch (DataException ex)
                    {
                        transaction.Rollback();
                        loggerManager.LogFatal(ex);
                        result = Constants.ErrorInOperation;
                    }
                }
            }

            return result;
        }

        private string CreateRandomUserName()
        {
            HostBehaviorManager.ChangeToSingle();
            int lengthGuestGamertag = 8;
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();

            string randomUsername = new string(Enumerable.Repeat(validChars, lengthGuestGamertag)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomUsername;
        }

        /// <summary>
        /// Checks if an email address already exists in the database.
        /// </summary>
        /// <param name="email">The email address to check for existence.</param>
        /// <returns>True if the email address exists; otherwise, false.</returns>
        public bool IsEmailExisting(string email)
        {
            bool result = false;
            LoggerManager logger = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            try
            {
                using (var dataBaseContext = new SpiderClueDbEntities())
                {
                    var existingAccount = dataBaseContext.accessAccounts.FirstOrDefault(accessAccount => accessAccount.email == email);
                    result = existingAccount != null;
                }
            }
            catch (EntityException entityException)
            {
                logger.LogError(entityException);
                result = false;
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Checks if a gamer's gamertag already exists in the database.
        /// </summary>
        /// <param name="gamertag">The gamertag to check for existence.</param>
        /// <returns>True if the gamertag exists; otherwise, false.</returns>
        public bool IsGamertagExisting(string gamertag)
        {
            bool result = false;
            LoggerManager logger = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            try
            {
                using (var dataBaseContext = new SpiderClueDbEntities())
                {
                    result = dataBaseContext.accessAccounts.Any(accessAccount => accessAccount.gamertag == gamertag);
                }
            }
            catch (EntityException entityException)
            {
                logger.LogError(entityException);
                result = false;
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Retrieves gamer information based on the provided gamertag from the database.
        /// </summary>
        /// <param name="gamertag">The gamertag to retrieve gamer information.</param>
        /// <returns>A Gamer object containing the retrieved gamer information or null if not found.</returns>
        public Gamer GetGamerByGamertag(string gamertag)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            Gamer gamer = new Gamer();

            try
            {
                using (var dataBaseContext = new SpiderClueDbEntities())
                {
                    var gamerInformation = dataBaseContext.gamers.FirstOrDefault(player => player.gamertag == gamertag);
                    var accessAcountInformation = dataBaseContext.accessAccounts.FirstOrDefault(accessAccount => accessAccount.gamertag == gamertag);
                    
                    if (gamerInformation != null && accessAcountInformation != null)
                    {
                        gamer.Gamertag = gamerInformation.gamertag;
                        gamer.FirstName = gamerInformation.firstName;
                        gamer.GamesWon = gamerInformation.gamesWon;
                        gamer.LastName = gamerInformation.lastName;
                        gamer.Email = accessAcountInformation.email;
                        gamer.ImageCode = gamerInformation.imageCode;

                    }
                    else
                    {
                        gamer = null;
                    }
                }
            }
            catch (EntityException entityException)
            {
                logger.LogError(entityException);
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
            }

            return gamer;
        }

        /// <summary>
        /// Retrieves gamer information based on the provided email from the database.
        /// </summary>
        /// <param name="email">The email to retrieve gamer information.</param>
        /// <returns>A Gamer object containing the retrieved gamer information or null if not found.</returns>
        public Gamer GetGamerByEmail(string email)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            Gamer gamer = new Gamer();
            try
            {
                using (var dataBaseContext = new SpiderClueDbEntities())
                {
                    var accessAcountInformation = dataBaseContext.accessAccounts.FirstOrDefault(accessAccount => accessAccount.email == email);

                    if (accessAcountInformation != null)
                    {
                        var gamerInformation = dataBaseContext.gamers.FirstOrDefault(player => player.gamertag == accessAcountInformation.gamertag);
                        if (gamerInformation != null && accessAcountInformation != null)
                        {
                            gamer.Gamertag = gamerInformation.gamertag;
                            gamer.FirstName = gamerInformation.firstName;
                            gamer.GamesWon = gamerInformation.gamesWon;
                            gamer.LastName = gamerInformation.lastName;
                            gamer.Email = accessAcountInformation.email;
                        } 
                        else
                        {
                            gamer = null;
                        }
                    } 
                    else
                    {
                        gamer = null;
                    }
                }
            }
            catch (EntityException entityException)
            {
                logger.LogError(entityException);
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
            }
            return gamer;
        }

        /// <summary>
        /// Modifies the first and last name of a gamer identified by the provided gamertag in the database.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer to be modified.</param>
        /// <param name="firstName">The updated first name for the gamer.</param>
        /// <param name="lastName">The updated last name for the gamer.</param>
        /// <returns>An integer indicating the result of the modification operation (Constants.SuccessInOperation or Constants.ErrorInOperation).</returns>
        public int ModifyAccount(string gamertag, string firstName, string lastName)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            int result = Constants.ErrorInOperation;
            try
            {
                using (var dataBaseContext = new SpiderClueDbEntities())
                {
                    var gamer = dataBaseContext.gamers.FirstOrDefault(player => player.gamertag == gamertag);
                    if (gamer != null)
                    {
                        gamer.firstName = firstName;
                        gamer.lastName = lastName;
                        dataBaseContext.SaveChanges();
                        result = Constants.SuccessInOperation;
                    }
                    else
                    {
                        result = Constants.ErrorInOperation;
                    }
                }
            }
            catch (EntityException entityException)
            {
                logger.LogError(entityException);
                result = Constants.ErrorInOperation;
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                result = Constants.ErrorInOperation;
            }
            
            return result;
        }

        /// <summary>
        /// Updates the password of a gamer identified by the provided gamertag in the database.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer whose password needs to be updated.</param>
        /// <param name="password">The new password for the gamer.</param>
        /// <returns>An integer indicating the result of the password update operation (Constants.SuccessInOperation or Constants.ErrorInOperation).</returns>
        public int UpdatePassword(String gamertag, String password)
        {
            HostBehaviorManager.ChangeToSingle();
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            int result = Constants.ErrorInOperation;

            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                try
                {
                    var existingAccessAccount = dataBaseContext.accessAccounts.FirstOrDefault(account => account.gamertag == gamertag);

                    if (existingAccessAccount != null)
                    {
                        existingAccessAccount.password = password;
                        dataBaseContext.SaveChanges();
                        result = Constants.SuccessInOperation;
                    }
                }
                catch (SqlException sQLException)
                {
                    loggerManager.LogError(sQLException);
                    result = Constants.ErrorInOperation;
                }
            }
            return result;
        }

        /// <summary>
        /// Changes the icon of a gamer identified by the provided gamertag in the database.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer whose icon needs to be changed.</param>
        /// <param name="titleIcon">The new icon code for the gamer.</param>
        /// <returns>An integer indicating the result of the icon change operation (Constants.SuccessInOperation or Constants.ErrorInOperation).</returns>
        public int ChangeIcon(string gamertag, string titleIcon)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            int result = Constants.ErrorInOperation;
            try
            {
                using (var dataBaseContext = new SpiderClueDbEntities())
                {
                    var gamer = dataBaseContext.gamers.FirstOrDefault(player => player.gamertag == gamertag);
                    if (gamer != null)
                    {
                        gamer.imageCode = titleIcon;
                        dataBaseContext.SaveChanges();
                        result = Constants.SuccessInOperation;
                    }
                    else
                    {
                        result = Constants.ErrorInOperation;
                    }
                }
            }
            catch (EntityException entityException)
            {
                logger.LogError(entityException);
                result = Constants.ErrorInOperation;
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                result = Constants.ErrorInOperation;
            }
            
            return result;
        }

        /// <summary>
        /// Retrieves the icon code associated with a gamer identified by the provided gamertag.
        /// </summary>
        /// <param name="gamertag">The gamertag of the gamer whose icon code is to be retrieved.</param>
        /// <returns>The icon code associated with the specified gamer. Returns the default icon code if not found or in case of an error.</returns>
        public string GetIcon(string gamertag)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            string imageCode = Constants.DefaultIcon;
            try
            {
                using (var dataBaseContext = new SpiderClueDbEntities())
                {
                    imageCode = dataBaseContext.gamers
                        .Where(player => player.gamertag == gamertag)
                        .Select(player => player.imageCode)
                        .FirstOrDefault();
                }
            }
            catch (EntityException entityException)
            {
                logger.LogError(entityException);
                imageCode = Constants.DefaultIcon;
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                imageCode = Constants.DefaultIcon;
            }
            return imageCode;
        }

        /// <summary>
        /// Deletes a guest player record from the database based on the provided gamertag.
        /// </summary>
        /// <param name="gamertag">The gamertag of the guest player to be deleted.</param>
        /// <returns>An operation result code. Returns success if the guest player is deleted, otherwise returns an error code.</returns>
        public int DeleteGuestPlayer(string gamertag)
        {
            HostBehaviorManager.ChangeToSingle();
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            int result = Constants.ErrorInOperation;

            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                using (var dataBaseContextTransaction = dataBaseContext.Database.BeginTransaction())
                {
                    try
                    {
                        var existingGamer = dataBaseContext.gamers.FirstOrDefault(gamer => gamer.gamertag == gamertag);

                        if (existingGamer != null)
                        {
                            dataBaseContext.gamers.Remove(existingGamer);
                            dataBaseContext.SaveChanges();

                            dataBaseContextTransaction.Commit();
                            result = Constants.SuccessInOperation;
                        }
                        else
                        {
                            result = Constants.ErrorInOperation;
                        }
                    }
                    catch (SqlException sqlException)
                    {
                        dataBaseContextTransaction.Rollback();
                        loggerManager.LogError(sqlException);
                        result = Constants.ErrorInOperation;
                    }
                    catch (DataException dataException)
                    {
                        dataBaseContextTransaction.Rollback();
                        loggerManager.LogFatal(dataException);
                        result = Constants.ErrorInOperation;
                    }
                }
            }

            return result;
        }

    }
}