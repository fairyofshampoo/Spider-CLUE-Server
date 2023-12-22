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

namespace GameService.Services
{
    
    public partial class GameService : IUserManager
    {

        public int AddUserTransaction(Gamer gamer)
        { 
            int result = Constants.ERROR_IN_OPERATION;
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
                            isbanned = 0
                        };

                        var newGamer = new DataBaseManager.gamer
                        {
                            firstName = gamer.FirstName,
                            lastName = gamer.LastName,
                            gamertag = gamer.Gamertag,
                            level = gamer.Level,
                            imageCode = gamer.ImageCode
                        };

                        dataBaseContext.accessAccounts.Add(newAccessAccount);
                        dataBaseContext.gamers.Add(newGamer);

                        dataBaseContext.SaveChanges();
                        dataBaseContextTransaction.Commit();

                        result = Constants.SUCCESS_IN_OPERATION;
                    }
                    catch (SqlException sQLException)
                    {
                        dataBaseContextTransaction.Rollback();
                        result = Constants.ERROR_IN_OPERATION;
                        throw sQLException;
                    }
                }
            }
            return result;
        }

        public bool AuthenticateAccount(string gamertag, string password)
        {
            using (var context = new SpiderClueDbEntities())
            {
                    var existingAccount = context.accessAccounts.FirstOrDefault(accessAccount => accessAccount.gamertag == gamertag);
                return existingAccount != null && existingAccount.password == password;
            }
        }

        public string RequestGuestPlayer()
        {
            string guestGamertag = CreateRandomUserName();
            CreateGuestGamer(guestGamertag);
            //aquí si el create es menor a cero puedo lanzar un OperationFailedException (creada por nosotros)
            return guestGamertag;
        }

        public int CreateGuestGamer(String gamertag)
        {
            LoggerManager loggerManager = new LoggerManager(this.GetType());
            int result = Constants.ERROR_IN_OPERATION;

            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                try
                {
                    var newGamer = new DataBaseManager.gamer
                    {
                        firstName = Constants.DEFAULT_GUEST_NAME,
                        lastName = Constants.DEFAULT_GUEST_LAST_NAME,
                        gamertag = gamertag,
                        level = Constants.DEFAULT_LEVEL,
                        imageCode = Constants.DEFAULT_ICON
                    };

                    dataBaseContext.gamers.Add(newGamer);

                    dataBaseContext.SaveChanges();

                    result = Constants.SUCCESS_IN_OPERATION;
                }
                catch (SqlException sQLException)
                {
                    loggerManager.LogError(sQLException);
                    result = Constants.ERROR_IN_OPERATION;
                }
            }
            return result;
        }

        private string CreateRandomUserName()
        {
            int lengthGuestGamertag = 8;
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();

            string randomUsername = new string(Enumerable.Repeat(validChars, lengthGuestGamertag)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomUsername;
        }

        public bool IsEmailExisting(string email)
        {
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                var existingAccount = dataBaseContext.accessAccounts.FirstOrDefault(accessAccount => accessAccount.email == email);
                return existingAccount != null;
            }
        }

        public bool IsGamertagExisting(string gamertag)
        {
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                return dataBaseContext.accessAccounts.Any(accessAccount => accessAccount.gamertag == gamertag);
            }
        }


        public Gamer GetGamerByGamertag(string gamertag)
        {
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                var gamerInformation = dataBaseContext.gamers.FirstOrDefault(player => player.gamertag == gamertag);
                var accessAcountInformation = dataBaseContext.accessAccounts.FirstOrDefault(accessAccount => accessAccount.gamertag == gamertag);
                Gamer gamer = new Gamer();
                if (gamerInformation != null && accessAcountInformation != null)
                {
                    gamer.Gamertag = gamerInformation.gamertag;
                    gamer.FirstName = gamerInformation.firstName;
                    gamer.Level = gamerInformation.level;
                    gamer.LastName = gamerInformation.lastName;
                    gamer.Email = accessAcountInformation.email;
                    gamer.ImageCode = gamerInformation.imageCode;

                }
                else
                {
                    gamer = null;
                }
                return gamer;
            }
        }

        public Gamer GetGamerByEmail(string email)
        {
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                var accessAcountInformation = dataBaseContext.accessAccounts.FirstOrDefault(accessAccount => accessAccount.email == email);
                var gamerInformation = dataBaseContext.gamers.FirstOrDefault(player => player.gamertag == accessAcountInformation.gamertag);
                Gamer gamer = new Gamer();
                if (gamerInformation != null && accessAcountInformation != null)
                {
                    gamer.Gamertag = gamerInformation.gamertag;
                    gamer.FirstName = gamerInformation.firstName;
                    gamer.Level = gamerInformation.level;
                    gamer.LastName = gamerInformation.lastName;
                    gamer.Email = accessAcountInformation.email;

                }
                else
                {
                    gamer = null;
                }
                return gamer;
            }
        }

        public int ModifyAccount(string gamertag, string firstName, string lastName)
        {
            int result = 0;
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                var gamer = dataBaseContext.gamers.FirstOrDefault(player => player.gamertag == gamertag);
                if (gamer != null)
                {
                    gamer.firstName = firstName;
                    gamer.lastName = lastName;
                    dataBaseContext.SaveChanges();
                    result = Constants.SUCCESS_IN_OPERATION; ;
                }
                else
                {
                    result = Constants.ERROR_IN_OPERATION;
                }
            }
            return result;
        }

        public int UpdatePassword(String gamertag, String password)
        {
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
                    }

                    dataBaseContext.SaveChanges();

                    result = Constants.SUCCESS_IN_OPERATION; ;
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
            int result = 0;
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                var gamer = dataBaseContext.gamers.FirstOrDefault(player => player.gamertag == gamertag);
                if (gamer != null)
                {
                    gamer.imageCode = titleIcon;
                    dataBaseContext.SaveChanges();
                    result = Constants.SUCCESS_IN_OPERATION; ;
                }
                else
                {
                    result = Constants.ERROR_IN_OPERATION;
                }
            }
            return result;
        }

        public string GetIcon(string gamertag)
        {
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                var imagecode = dataBaseContext.gamers
                    .Where(player => player.gamertag == gamertag)
                    .Select(player => player.imageCode)
                    .FirstOrDefault();
                return imagecode;
            }
        }

        public int DeleteGuestPlayer(string gamertag)
        {
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
                            result = Constants.SUCCESS_IN_OPERATION;
                        }
                    }
                    catch (SqlException sqlException)
                    {
                        loggerManager.LogError(sqlException);
                    }
                    catch (Exception exception)
                    {
                        dataBaseContextTransaction.Rollback();
                        loggerManager.LogFatal(exception);
                    }
                }
            }

            return result;
        }

    }
}