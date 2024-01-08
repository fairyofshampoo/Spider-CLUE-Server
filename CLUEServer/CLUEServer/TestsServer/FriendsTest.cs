using DataBaseManager;
using GameService.Contracts;
using GameService.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
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

            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    string gamertag = "Star3oy";
                    string friendGamertag = "mich";
                    var existingFriendship = databaseContext.friendLists
                            .FirstOrDefault(f => f.gamertag == gamertag && f.friend == friendGamertag);

                    if (existingFriendship == null)
                    {
                        var newFriends = new DataBaseManager.friendList
                        {
                            gamertag = gamertag,
                            friend = friendGamertag
                        };
                        databaseContext.friendLists.Add(newFriends);
                        databaseContext.SaveChanges();
                    }
                }
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"{sqlException.Message}");
            }
            catch (EntityException entityException)
            {
                Console.WriteLine(entityException.Message);
            }

            try
            {
                using (var databaseContext = new SpiderClueDbEntities())
                {
                    string gamertag = "mich";
                    string friendGamertag = "Star3oy";
                    var existingFriendship = databaseContext.friendLists
                            .FirstOrDefault(f => f.gamertag == gamertag && f.friend == friendGamertag);

                    if (existingFriendship == null)
                    {
                        var newFriends = new DataBaseManager.friendList
                        {
                            gamertag = gamertag,
                            friend = friendGamertag
                        };
                        databaseContext.friendLists.Add(newFriends);
                        databaseContext.SaveChanges();
                    }
                }
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"{sqlException.Message}");
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
                        .FirstOrDefault(player => player.gamertag == "soobin");
                    if (gamerInDB != null)
                    {
                        context.gamers.Remove(gamerInDB);
                        context.SaveChanges();
                        Console.WriteLine("Se ha eliminado");
                    }

                    var secondGamerInDB = context.gamers
                        .FirstOrDefault(player => player.gamertag == "michito");
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
        FriendTestConfiguration Configuration;

        public FriendsTest(FriendTestConfiguration configuration)
        {
            Configuration = configuration;
        }

        [Fact]
        public void AreNotFriendsTestSuccess()
        {
            bool result = false;

            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            result = friendshipManager.AreNotFriends("soobin", "Star3oy");
            Assert.True(result);
        }

        [Fact]
        public void AreNotFriendsTestErrorConnection()
        {

            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendshipManager.AreNotFriends("soobin", "Star3oy"));
        }

        [Fact]
        public void AreNotFriendsTestFalse()
        {
            bool result = false;

            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            result = friendshipManager.AreNotFriends("soobin", "michito");
            Assert.False(result);
        }

        [Fact]
        public void AddFriendTestSuccess()
        {
            int resultExpected = Constants.SUCCESS_IN_OPERATION;

            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            int result = friendshipManager.AddFriend("soobin", "michito");
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void AddFriendTestErrorConnection()
        {
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendshipManager.AddFriend("soobin", "michito"));
        }

        [Fact]
        public void GetFriendListSuccess()
        {
            string[] resultExpected = new string[]
            {
                "michito"
            };
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            string[] result = friendshipManager.GetFriendList("soobin");
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void GetFriendListTestConnectionError()
        {
            string[] resultExpected = new string[]
            {
                "michito"
            };
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendshipManager.GetFriendList("soobin"));
        }

        [Fact]
        public void GetFriendListFail()
        {
            string[] resultExpected = new string[]
            {
                "michito", "Star3oy"
            };

            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            string[] result = friendshipManager.GetFriendList("soobin");
            Assert.False(resultExpected.Equals(result));
        }

        [Fact]
        public void DeleteFriendFail()
        {
            int resultExpected = Constants.SUCCESS_IN_OPERATION;

            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            int result = friendshipManager.DeleteFriend("Star3oy", "michito");
            Assert.False(resultExpected.Equals(result));
        }

        [Fact]
        public void DeleteFriendErrorConnection()
        {
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendshipManager.DeleteFriend("Star3oy", "michito"));
        }

        [Fact]
        public void DeleteFriendSuccess()
        {
            int resultExpected = Constants.SUCCESS_IN_OPERATION;

            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            int result = friendshipManager.DeleteFriend("soobin", "michito");
            Assert.True(resultExpected.Equals(result));
        }

        [Fact]
        public void CreateFriendRequestSuccess()
        {
            int resultExpected = Constants.SUCCESS_IN_OPERATION;

            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            int result = friendRequestManager.CreateFriendRequest("Star3oy", "michi");
            Assert.True(resultExpected.Equals(result));
        }

        [Fact]
        public void CreateFriendRequestErrorConnection()
        {
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendRequestManager.CreateFriendRequest("Star3oy", "michi"));
        }

        [Fact]
        public void GetFriendsRequestSuccess()
        {
            string[] resultExpected = new string[]
            {
                "Star3oy"
            };

            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            string[] result = friendRequestManager.GetFriendsRequest("michi");
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void GetFriendsRequestErrorConnection()
        {
            string[] resultExpected = new string[]
            {
                "Star3oy"
            };
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendRequestManager.GetFriendsRequest("michi"));
        }

        [Fact]
        public void GetFriendsRequestFail()
        {
            string[] resultExpected = new string[]
            {
                "Star3oy"
            };

            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            string[] result = friendRequestManager.GetFriendsRequest("soobin");
            Assert.False(resultExpected.Equals(result));
        }

        [Fact]
        public void ResponseFriendRequestSuccess()
        {
            int resultExpected = Constants.SUCCESS_IN_OPERATION;

            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            int result = friendRequestManager.ResponseFriendRequest("michi", "Star3oy", "Accepted");
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void ResponseFriendRequestErrorConnection()
        {
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendRequestManager.ResponseFriendRequest("michi", "Star3oy", "Accepted"));
        }

        [Fact]
        public void ResponseFriendRequestFail()
        {
            int resultExpected = Constants.ERROR_IN_OPERATION;

            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            int result = friendRequestManager.ResponseFriendRequest("michi", "michi", "Accepted");
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void DeleteFriendRequestSuccess()
        {
            int resultExpected = Constants.SUCCESS_IN_OPERATION;

            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            int result = friendRequestManager.DeleteFriendRequest("michi", "Star3oy");
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void DeleteFriendRequestErrorConnection()
        {
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendRequestManager.DeleteFriendRequest("michi", "Star3oy"));
        }

        [Fact]
        public void DeleteFriendRequestFail()
        {
            int resultExpected = Constants.ERROR_IN_OPERATION;

            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            int result = friendRequestManager.DeleteFriendRequest("michi", "michi");
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void ThereIsNoFriendRequestFail()
        {
            bool result = false;

            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            result = friendshipManager.ThereIsNoFriendRequest("michi", "Star3oy");
            Assert.True(result);
        }

        [Fact]
        public void ThereIsNoFriendRequestErrorConnection()
        {
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendshipManager.ThereIsNoFriendRequest("michi", "Star3oy"));
        }

        [Fact]
        public void ThereIsNoFriendRequestSuccess()
        {
            bool result = false;

            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            result = friendshipManager.ThereIsNoFriendRequest("Star3oy", "michi");
            Assert.False(result);
        }

    }
}
