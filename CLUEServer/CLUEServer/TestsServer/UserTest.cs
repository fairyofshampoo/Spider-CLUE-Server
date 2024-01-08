using DataBaseManager;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using TestsServer.SpiderClueService;
using Xunit;




namespace TestsServer
{
    public class UsetTestConfiguration : IDisposable
    {
        public UsetTestConfiguration()
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
                                password = "xP@ssword0910x",
                                gamertag = "Lalonch3ra",
                                email = "correo@gmail.com",
                            };

                            var newGamer = new DataBaseManager.gamer
                            {
                                firstName = "Eduardo",
                                lastName = "Carrera",
                                gamertag = "Lalonch3ra",
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
                                password = "xK@nye20",
                                gamertag = "Aligtl",
                                email = "Gorila@gmail.com",
                            };

                            var newGamer = new DataBaseManager.gamer
                            {
                                firstName = "Yael",
                                lastName = "Dominguez",
                                gamertag = "Aligtl",
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

    public class UsetTest : IClassFixture<UsetTestConfiguration>
    {
        UsetTestConfiguration Configuration;

        public UsetTest(UsetTestConfiguration configuration)
        {
            Configuration = configuration;
        }

        [Fact]
        public void InsertGamerTest()
        {
            int resultExcepted = 1;

            Gamer gamer = new Gamer()
            {
                FirstName = "Karla",
                LastName = "Vazquez",
                Gamertag = "Karlatv",
                GamesWon = 0,
                ImageCode = "Icon0.jpg",
                Password = "Qfb#2307",
                Email = "Karla@gmail.com"
            };

            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            int result = userManager.AddUserTransaction(gamer);
            Assert.Equal(resultExcepted, result);
        }

        [Fact]
        public void InsertGamerFailTest()
        {

            int resultExcepted = -1;
            Gamer gamer = new Gamer()
            {
                FirstName = "Eduardo",
                LastName = "Carrera",
                Gamertag = "Star3oy",
                GamesWon = 0,
                ImageCode = "Icon0.jpg",
                Password = "xP@ssword0910x",
                Email = "eduardo@gmail.com"
            };

            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            int result = userManager.AddUserTransaction(gamer);
            Assert.Equal(resultExcepted, result);

        }

        [Fact]
        public void ModifyGamerDataTest()
        {
            int resultExcepted = -1;
            string gamertag = "Star3oy";
            string firstName = "Eduardo";
            string lastName = "Carrera";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            int result = userManager.ModifyAccount(gamertag, firstName, lastName);
            Assert.Equal(resultExcepted, result);
        }

        [Fact]
        public void ModifyGamerDataFailTest()
        {
            int resultExcepted = -1;
            string gamertag = "Swift";
            string firstName = "Taylor";
            string lastName = "Swift";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            int result = userManager.ModifyAccount(gamertag, firstName, lastName);
            Assert.Equal(resultExcepted, result);
        }

        [Fact]
        public void AuthenticateAccountTest()
        {
            string gamertag = "Star3oy";
            string password = "164cdbd8614682a2cf2f7e944badcf5aa95d41a9";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            bool result = userManager.AuthenticateAccount(gamertag, password);
            Assert.True(result);
        }

        [Fact]
        public void AuthenticateAccountFailTest()
        {
            string gamertag = "NoobMaster";
            string password = "xPasswordx";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            bool result = userManager.AuthenticateAccount(gamertag, password);
            Assert.False(result);
        }

        [Fact]
        public void IsEmailExistingTest()
        {
            string email = "eduarcaco@hotmail.com";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            bool result = userManager.IsEmailExisting(email);
            Assert.True(result);
        }

        [Fact]
        public void IsEmailExistingFailTest()
        {
            string email = "correoinexistente@gmail.com";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            bool result = userManager.IsEmailExisting(email);
            Assert.False(result);
        }

        [Fact]
        public void UpdatePasswordTest()
        {
            string gamertag = "Lalonch3ra";
            string newPassword = "KRRERA135625x";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            int result = userManager.UpdatePassword(gamertag, newPassword);
            int resultExpected = -1;
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void UpdatePasswordFailTest()
        {
            string gamertag = "Yanpol";
            string newPassword = "RTX4090ti";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            int result = userManager.UpdatePassword(gamertag, newPassword);
            int resultExpected = -1;
            Assert.Equal(result, resultExpected);
        }

        [Fact]
        public void IsGamertagExistingTest()
        {
            string gamertag = "Star3oy";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            bool result = userManager.IsGamertagExisting(gamertag);
            Assert.True(result);
        }

        [Fact]
        public void IsGamertagExistingFailTest()
        {
            string gamertag = "noobMaster";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            bool result = userManager.IsGamertagExisting(gamertag);
            Assert.False(result);
        }

        [Fact]
        public void GetGamerByGamertagTest()
        {
            string gamertag = "Star3oy";

            Gamer gamer = new Gamer
            {
                FirstName = "Eduardo",
                LastName = "Carrera",
                Gamertag = "Star3oy",
                GamesWon = 0,
                ImageCode = "Icon0.jpg",
                Password = "164cdbd8614682a2cf2f7e944badcf5aa95d41a9",
                Email = "eduarcaco@hotmail.com"
            };

            SpiderClueService.IUserManager userManager = new UserManagerClient();
            Gamer secondGamer = userManager.GetGamerByGamertag(gamertag);

            Assert.Equal(gamer, secondGamer);
        }

        [Fact]
        public void GetGamerByGamertagFailTest()
        {
            string gamertag = "Star3oy";

            Gamer gamer = new Gamer
            {
                FirstName = "Mac",
                LastName = "Miller",
                Gamertag = "mac",
                GamesWon = 0,
                ImageCode = "Icon2.jpg",
                Password = "164cdbd8614682a2cf2f7e944badcf5aa95d41a9",
                Email = "MacMiller@hotmail.com"
            };

            SpiderClueService.IUserManager userManager = new UserManagerClient();
            Gamer secondGamer = userManager.GetGamerByGamertag(gamertag);
            Assert.False(gamer.Equals(secondGamer));
        }

        [Fact]
        public void GetGamerByEmailTest()
        {
            string gamertag = "eduarcaco@hotmail.com";

            Gamer gamer = new Gamer
            {
                FirstName = "Eduardo",
                LastName = "Carrera",
                Gamertag = "Star3oy",
                GamesWon = 0,
                ImageCode = "Icon0.jpg",
                Password = "164cdbd8614682a2cf2f7e944badcf5aa95d41a9",
                Email = "eduarcaco@hotmail.com"
            };

            SpiderClueService.IUserManager userManager = new UserManagerClient();
            Gamer secondGamer = userManager.GetGamerByEmail(gamertag);

            Assert.Equal(gamer, secondGamer);
        }

        [Fact]
        public void GetGamerByEmailFailTest()
        {
            string gamertag = "eduarcaco@hotmail.com";

            Gamer gamer = new Gamer
            {
                FirstName = "Aldo",
                LastName = "Carrera",
                Gamertag = "AldoJr",
                GamesWon = 0,
                ImageCode = "Icon2.jpg",
                Password = "164cdbd8614682a2cf2f7e944badcf5aa95d41a9",
                Email = "eduarcaco@hotmail.com"
            };

            SpiderClueService.IUserManager userManager = new UserManagerClient();
            Gamer secondGamer = userManager.GetGamerByEmail(gamertag);
            Assert.False(gamer.Equals(secondGamer));
        }

        [Fact]
        public void ChangeIconTest()
        {
            string gamertag = "Star3oy";
            string icon = "Icon1";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            int result = userManager.ChangeIcon(gamertag, icon);
            int resultExcepted = 1;
            Assert.Equal(result, resultExcepted);
        }

        [Fact]
        public void ChangeIconFailTest()
        {
            string gamertag = "Aldojr";
            string icon = "Icon1";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            int result = userManager.ChangeIcon(gamertag, icon);
            int resultExcepted = -1;
            Assert.Equal(result, resultExcepted);
        }

        [Fact]
        public void GetIconTest()
        {
            string gamertag = "Star3oy";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            string result = userManager.GetIcon(gamertag);
            string resultExcepted = "Icon1";
            Assert.Equal(result, resultExcepted);
        }

        [Fact]
        public void GetIconFailTest()
        {
            string gamertag = "Star3oy";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            string result = userManager.GetIcon(gamertag);
            string resultExcepted = "Icon3";
            Assert.False(result.Equals(resultExcepted));
        }

        [Fact]
        public void DeleteGuestPlayerTest()
        {
            string gamertag = "BWdS3tzN";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            int result = userManager.DeleteGuestPlayer(gamertag);
            int resultExcepted = 1;
            Assert.Equal(result, resultExcepted);
        }

        [Fact]
        public void DeleteGuestPlayerFailTest()
        {
            string gamertag = "GuessPlayer";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            int result = userManager.DeleteGuestPlayer(gamertag);
            int resultExcepted = -1;
            Assert.Equal(result, resultExcepted);
        }


    }

}