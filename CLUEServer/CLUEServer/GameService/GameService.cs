using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseManager;
using BCrypt;
using GameService;
using System.Security.Cryptography;
using System.ServiceModel.Configuration;
using System.Data.SqlClient;

namespace GameService
{
    public class GameService : IUserManager
    {
        const int Error = -1;
        const int Success = 1;

        public int AddUserTransaction(Gamer gamer)
        {
            using (var dataBaseContext = new SpiderClueEntities())
            {
                using (var dataBaseContextTransaction = dataBaseContext.Database.BeginTransaction())
                {
                    try
                    {
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(gamer.Password, BCrypt.Net.BCrypt.GenerateSalt());
                        var newAccessAccount = new DataBaseManager.accessAccount
                        {
                            password = gamer.Password,
                            gamertag = gamer.Gamertag,
                            email = gamer.Email,
                            isbanned = gamer.IsBanned
                        };

                        var newGamer = new DataBaseManager.gamer
                        {
                            firstName = gamer.FirstName,
                            lastName = gamer.LastName,
                            level = gamer.Level
                        };

                        dataBaseContext.AccessAccounts.Add(newAccessAccount);
                        dataBaseContext.Gamers.Add(newGamer);

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
            using (var context = new SpiderClueEntities())
            {
                var existingAccount = context.AccessAccounts.FirstOrDefault(accessAccount => accessAccount.gamertag == gamertag);
                return existingAccount != null && BCrypt.Net.BCrypt.Verify(password, existingAccount.password);
            }
        }

        public bool IsAccountExisting(string email)
        {
            using (var dataBaseContext = new SpiderClueEntities())
            {
                var existingAccount = dataBaseContext.AccessAccounts.FirstOrDefault(accessAccount => accessAccount.email == email);
                return existingAccount != null;
            }
        }
    }
}