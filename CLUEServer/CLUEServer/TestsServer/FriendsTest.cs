using DataBaseManager;
using GameService.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using TestsServer.SpiderClueService;
using Xunit;

namespace TestsServer
{
    public class FriendTestConfiguration : IDisposable
    {
        public FriendTestConfiguration()
        {
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
                                password = "@byFairy0fShampoo",
                                gamertag = "michito",
                                email = "michito@gmail.com",
                            };

                            var newGamer = new DataBaseManager.gamer
                            {
                                firstName = "Michelle",
                                lastName = "Moreno",
                                gamertag = "michito",
                                gamesWon = 0,
                                imageCode = "Icon0.jpg"
                            };

                            dataBaseContext.accessAccounts.Add(newAccessAccount);
                            dataBaseContext.gamers.Add(newGamer);
                            dataBaseContext.SaveChanges();
                            dataBaseContextTransaction.Commit();
                        }
                        catch (SqlException sQLException)
                        {
                            Console.WriteLine(sQLException.Message);
                            dataBaseContextTransaction.Rollback();
                        }
                    }
                }

                using (var dataBaseContext = new SpiderClueDbEntities())
                {
                    using (var dataBaseContextTransaction = dataBaseContext.Database.BeginTransaction())
                    {
                        try
                        {
                            var newAccessAccount = new DataBaseManager.accessAccount
                            {
                                password = "@byFairy0fShampoo",
                                gamertag = "soobin",
                                email = "soobin@gmail.com",
                            };

                            var newGamer = new DataBaseManager.gamer
                            {
                                firstName = "Miriam",
                                lastName = "Ramirez",
                                gamertag = "soobin",
                                gamesWon = 0,
                                imageCode = "Icon0.jpg"
                            };

                            dataBaseContext.accessAccounts.Add(newAccessAccount);
                            dataBaseContext.gamers.Add(newGamer);
                            dataBaseContext.SaveChanges();
                            dataBaseContextTransaction.Commit();
                        }
                        catch (SqlException sQLException)
                        {
                            Console.WriteLine(sQLException.Message);
                            dataBaseContextTransaction.Rollback();
                        }
                    }
                }
            }
            catch (EntityException entityException)
            {
                Console.WriteLine(entityException.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                using (var context = new SpiderClueDbEntities())
                {
                    var gamerInDB = context.gamers
                        .FirstOrDefault(player => player.gamertag == "Lalonch3ra");
                    if (gamerInDB != null)
                    {
                        context.gamers.Remove(gamerInDB);
                        context.SaveChanges();
                        Console.WriteLine("Se ha eliminado");
                    }

                    var secondGamerInDB = context.gamers
                        .FirstOrDefault(player => player.gamertag == "Karlatv");
                    if (gamerInDB != null)
                    {
                        context.gamers.Remove(secondGamerInDB);
                        context.SaveChanges();
                        Console.WriteLine("Se ha eliminado");
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
    public class FriendsTest : IClassFixture<FriendTestConfiguration>
    {
    }
}
