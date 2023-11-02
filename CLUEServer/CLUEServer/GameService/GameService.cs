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

namespace GameService.Services
{
    public partial class GameService : IUserManager
    {
        const int Error = -1;
        const int Success = 1;

        public int AddUserTransaction(Gamer gamer)
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
                            isbanned = 0
                        };

                        var newGamer = new DataBaseManager.gamer
                        {
                            firstName = gamer.FirstName,
                            lastName = gamer.LastName,
                            gamertag = gamer.Gamertag,
                            level = gamer.Level
                        };

                        dataBaseContext.accessAccounts.Add(newAccessAccount);
                        dataBaseContext.gamers.Add(newGamer);

                        dataBaseContext.SaveChanges();
                        dataBaseContextTransaction.Commit();

                        return Success;
                    }
                    catch (SqlException sQLException)
                    {
                        dataBaseContextTransaction.Rollback();
                        return Error;
                        throw sQLException;
                    }
                }
            }
        }

        public bool AuthenticateAccount(string gamertag, string password)
        {
            using (var context = new SpiderClueDbEntities())
            {
                var existingAccount = context.accessAccounts.FirstOrDefault(accessAccount => accessAccount.gamertag == gamertag);
                return existingAccount != null && existingAccount.password == password;
            }
        }

        public string RequestGuessPlayer()
        {
            return CreateRandomUserName();
        }

        private string CreateRandomUserName()
        {
            int length = 8;
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();

            string randomUsername = new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return randomUsername;
        }


        public bool IsAccountExisting(string email)
        {
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                var existingAccount = dataBaseContext.accessAccounts.FirstOrDefault(accessAccount => accessAccount.email == email);
                return existingAccount != null;
            }
        }

        public int AuthenticateGamertag(string soughtGamertag)
        {
            using(var dataBaseContext = new SpiderClueDbEntities())
            {
                int coincidences = dataBaseContext.gamers.Count(gamer =>  gamer.gamertag == soughtGamertag);
                return coincidences;
            }
        }

        public int AuthenticateEmail(string soughtEmail)
        {
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                int coincidences = dataBaseContext.gamers.Count(gamer => gamer.gamertag == soughtEmail);
                return coincidences;
            }
        }

        public Boolean IsAccessAccountExisting (String user, String password)
        {
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                Boolean exist = false;
                int coincidences = dataBaseContext.accessAccounts.Count(accessAccount => accessAccount.gamertag == user && accessAccount.password == password);
                if (coincidences == 2)
                {
                    exist = true;
                }
                return exist;
            }
        }

        public bool IsEmailExisting(string email)
        {
            using(var dataBaseContext = new SpiderClueDbEntities())
            {
                Boolean exist = false;
                int coincidences = dataBaseContext.accessAccounts.Count(accessAccount => accessAccount.email == email);
                if (coincidences > 0)
                {
                    exist = true;   
                }
                return exist;
            }
        }

        public bool IsGamertagExisting(string gamertag)
        {
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                Boolean exists = false;
                int coincidences = dataBaseContext.accessAccounts.Count(accessAccount => accessAccount.gamertag ==  gamertag);
                if (coincidences > 0)
                {
                    exists = true;
                }
                return exists;
            }
           
        }

        public Gamer GetGamer(string gamertag)
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

                }
                return gamer;
            }
        }

        public int GetBannedStatus(string gamertag)
        {
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                int isBanned = dataBaseContext.accessAccounts
                    .Where(accessAccount => accessAccount.gamertag == gamertag)
                    .Select(accessAccount => accessAccount.isbanned)
                    .FirstOrDefault();

                return isBanned;
            }
        }

        public int ModifyAccount(string gamertag, string firstName, string lastName)
        {
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                    var gamer = dataBaseContext.gamers.FirstOrDefault(player => player.gamertag == gamertag);
                    if (gamer != null)
                    {
                        gamer.firstName = firstName;
                        gamer.lastName = lastName;
                        dataBaseContext.SaveChanges();
                        return Success;
                    }else 
                    { 
                    return Error; 
                    }
            }
        }
    }
}