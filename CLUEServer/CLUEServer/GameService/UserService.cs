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

        public int AddUserTransaction(Gamer gamer)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            int result = Constants.ERROR_IN_OPERATION;
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

                            result = Constants.SUCCESS_IN_OPERATION;
                        }
                        catch (DbUpdateException updateException)
                        {
                            logger.LogError(updateException);
                            dataBaseContextTransaction.Rollback();
                            result = Constants.ERROR_IN_OPERATION;
                        }
                        catch (SqlException sQLException)
                        {
                            logger.LogError(sQLException);
                            dataBaseContextTransaction.Rollback();
                            result = Constants.ERROR_IN_OPERATION;
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

        public int CreateGuestGamer(string gamertag)
        {
            HostBehaviorManager.ChangeToSingle();
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            int result = Constants.ERROR_IN_OPERATION;

            using (var dbContext = new SpiderClueDbEntities())
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var newGamer = new DataBaseManager.gamer
                        {
                            firstName = Constants.DEFAULT_GUEST_NAME,
                            lastName = Constants.DEFAULT_GUEST_LAST_NAME,
                            gamertag = gamertag,
                            gamesWon = Constants.DEFAULT_GAMES_WON,
                            imageCode = Constants.DEFAULT_ICON
                        };

                        dbContext.gamers.Add(newGamer);

                        dbContext.SaveChanges();

                        transaction.Commit();
                        result = Constants.SUCCESS_IN_OPERATION;
                    }
                    catch (SqlException sQLException)
                    {
                        transaction.Rollback();
                        loggerManager.LogError(sQLException);
                        result = Constants.ERROR_IN_OPERATION;
                    }
                    catch (EntityException ex)
                    {
                        transaction.Rollback();
                        loggerManager.LogError(ex);
                        result = Constants.ERROR_IN_OPERATION;
                    }
                    catch (DataException ex)
                    {
                        transaction.Rollback();
                        loggerManager.LogFatal(ex);
                        result = Constants.ERROR_IN_OPERATION;
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

        public int ModifyAccount(string gamertag, string firstName, string lastName)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            int result = Constants.ERROR_IN_OPERATION;
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
                        result = Constants.SUCCESS_IN_OPERATION;
                    }
                    else
                    {
                        result = Constants.ERROR_IN_OPERATION;
                    }
                }
            }
            catch (EntityException entityException)
            {
                logger.LogError(entityException);
                result = Constants.ERROR_IN_OPERATION;
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                result = Constants.ERROR_IN_OPERATION;
            }
            
            return result;
        }

        public int UpdatePassword(String gamertag, String password)
        {
            HostBehaviorManager.ChangeToSingle();
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            int result = Constants.ERROR_IN_OPERATION;

            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                try
                {
                    var existingAccessAccount = dataBaseContext.accessAccounts.FirstOrDefault(account => account.gamertag == gamertag);

                    if (existingAccessAccount != null)
                    {
                        existingAccessAccount.password = password;
                        dataBaseContext.SaveChanges();
                        result = Constants.SUCCESS_IN_OPERATION;
                    }
                }
                catch (SqlException sQLException)
                {
                    loggerManager.LogError(sQLException);
                    result = Constants.ERROR_IN_OPERATION;
                }
            }
            return result;
        }

        public int ChangeIcon(string gamertag, string titleIcon)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            int result = Constants.ERROR_IN_OPERATION;
            try
            {
                using (var dataBaseContext = new SpiderClueDbEntities())
                {
                    var gamer = dataBaseContext.gamers.FirstOrDefault(player => player.gamertag == gamertag);
                    if (gamer != null)
                    {
                        gamer.imageCode = titleIcon;
                        dataBaseContext.SaveChanges();
                        result = Constants.SUCCESS_IN_OPERATION;
                    }
                    else
                    {
                        result = Constants.ERROR_IN_OPERATION;
                    }
                }
            }
            catch (EntityException entityException)
            {
                logger.LogError(entityException);
                result = Constants.ERROR_IN_OPERATION;
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                result = Constants.ERROR_IN_OPERATION;
            }
            
            return result;
        }

        public string GetIcon(string gamertag)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            HostBehaviorManager.ChangeToSingle();
            string imageCode = Constants.DEFAULT_ICON;
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
                imageCode = Constants.DEFAULT_ICON;
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                imageCode = Constants.DEFAULT_ICON;
            }
            return imageCode;
        }

        public int DeleteGuestPlayer(string gamertag)
        {
            HostBehaviorManager.ChangeToSingle();
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            int result = Constants.ERROR_IN_OPERATION;

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
                            result = Constants.SUCCESS_IN_OPERATION;
                        }
                        else
                        {
                            result = Constants.ERROR_IN_OPERATION;
                        }
                    }
                    catch (SqlException sqlException)
                    {
                        dataBaseContextTransaction.Rollback();
                        loggerManager.LogError(sqlException);
                        result = Constants.ERROR_IN_OPERATION;
                    }
                    catch (DataException dataException)
                    {
                        dataBaseContextTransaction.Rollback();
                        loggerManager.LogFatal(dataException);
                        result = Constants.ERROR_IN_OPERATION;
                    }
                }
            }

            return result;
        }

    }
}