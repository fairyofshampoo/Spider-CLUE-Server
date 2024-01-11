using DataBaseManager;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.ServiceModel;
using TestsServer.SpiderClueService;
using Xunit;




namespace TestsServer
{
    public class UsetTestConfiguration : IDisposable
    {
        public UsetTestConfiguration()
        {
            
        }

        public void Dispose()
        {
            
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
            int resultExcepted = ConstantsTests.Success;

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
        public void InsertGamerConectionErrorTest()
        {
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
            Assert.Throws<EndpointNotFoundException>(() => userManager.AddUserTransaction(gamer));
        }

        [Fact]
        public void InsertGamerFailTest()
        {

            int resultExcepted = ConstantsTests.Failure;
            Gamer gamer = new Gamer()
            {
                FirstName = "Eduardo",
                LastName = "Carrera",
                Gamertag = "Lalonch3ra",
                GamesWon = 0,
                ImageCode = "Icon1.jpg",
                Password = "P@ssword0910",
                Email = "eduardo@gmail.com"
            };

            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            int result = userManager.AddUserTransaction(gamer);
            Assert.Equal(resultExcepted, result);

        }

        [Fact]
        public void ModifyGamerDataTest()
        {
            int resultExcepted = ConstantsTests.Failure;
            string gamertag = "Lalonch3ra";
            string firstName = "Mac";
            string lastName = "Miller";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            int result = userManager.ModifyAccount(gamertag, firstName, lastName);
            Assert.Equal(resultExcepted, result);
        }

        [Fact]
        public void ModifyGamerDataConnectionErrorTest()
        {
            string gamertag = "Lalonch3ra";
            string firstName = "Mac";
            string lastName = "Miller";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => userManager.ModifyAccount(gamertag, firstName, lastName));
        }

        [Fact]
        public void ModifyGamerDataFailTest()
        {
            int resultExcepted = ConstantsTests.Failure;
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
            string gamertag = "Lalonch3ra";
            string password = "164cdbd8614682a2cf2f7e944badcf5aa95d41a9";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            bool result = userManager.AuthenticateAccount(gamertag, password);
            Assert.True(result);
        }

        [Fact]
        public void AuthenticateAccountErrorConnectionTest()
        {
            string gamertag = "Lalonch3ra";
            string password = "164cdbd8614682a2cf2f7e944badcf5aa95d41a9";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();

            Assert.Throws<EndpointNotFoundException>(() => userManager.AuthenticateAccount(gamertag, password));
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
            string email = "eduardo@gmail.com";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            bool result = userManager.IsEmailExisting(email);
            Assert.True(result);
        }

        [Fact]
        public void IsEmailExistingErrorConnectionTest()
        {
            string email = "eduardo@gmail.com";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => userManager.IsEmailExisting(email));
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
            string gamertag = "aldoJr";
            string newPassword = "855d96d4b2b7768cc3191eb4de158ab540090f0a";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            int result = userManager.UpdatePassword(gamertag, newPassword);
            int resultExpected = ConstantsTests.Success;
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void UpdatePasswordErrorConnectionTest()
        {
            string gamertag = "aldoJr";
            string newPassword = "855d96d4b2b7768cc3191eb4de158ab540090f0a";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => userManager.UpdatePassword(gamertag, newPassword));
        }

        [Fact]
        public void UpdatePasswordFailTest()
        {
            string gamertag = "Yanpol";
            string newPassword = "RTX4090ti";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            int result = userManager.UpdatePassword(gamertag, newPassword);
            int resultExpected = ConstantsTests.Failure;
            Assert.Equal(result, resultExpected);
        }

        [Fact]
        public void IsGamertagExistingTest()
        {
            string gamertag = "Lalonch3ra";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            bool result = userManager.IsGamertagExisting(gamertag);
            Assert.True(result);
        }

        [Fact]
        public void IsGamertagExistingErrorConnectionTest()
        {
            string gamertag = "Lalonch3ra";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => userManager.IsGamertagExisting(gamertag));
        }

        [Fact]
        public void IsGamertagExistingFailTest()
        {
            string gamertag = "dakara";
            SpiderClueService.IUserManager userManager = new SpiderClueService.UserManagerClient();
            bool result = userManager.IsGamertagExisting(gamertag);
            Assert.False(result);
        }

        [Fact]
        public void GetGamerByGamertagTest()
        {
            string gamertag = "Lalonch3ra";

            Gamer gamer = new Gamer
            {
                FirstName = "Eduardo",
                LastName = "Carrera",
                Gamertag = "Lalonch3ra",
                GamesWon = 0,
                ImageCode = "Icon1.jpg",
                Password = "164cdbd8614682a2cf2f7e944badcf5aa95d41a9",
                Email = "eduardo@gmail.com"
            };

            SpiderClueService.IUserManager userManager = new UserManagerClient();
            Gamer secondGamer = userManager.GetGamerByGamertag(gamertag);

            Assert.Equal(gamer, secondGamer);
        }

        [Fact]
        public void GetGamerByGamertagErrorConnectionTest()
        {
            string gamertag = "Lalonch3ra";
            Gamer gamer = new Gamer
            {
                FirstName = "Eduardo",
                LastName = "Carrera",
                Gamertag = "Lalonch3ra",
                GamesWon = 0,
                ImageCode = "Icon1.jpg",
                Password = "164cdbd8614682a2cf2f7e944badcf5aa95d41a9",
                Email = "eduardo@gmail.com"
            };
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => userManager.GetGamerByGamertag(gamertag));
        }

        [Fact]
        public void GetGamerByGamertagFailTest()
        {
            string gamertag = "dakara";

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
            string gamertag = "eduardo@gmail.com";
            Gamer gamer = new Gamer
            {
                FirstName = "Eduardo",
                LastName = "Carrera",
                Gamertag = "Lalonch3ra",
                GamesWon = 0,
                ImageCode = "Icon1.jpg",
                Password = "164cdbd8614682a2cf2f7e944badcf5aa95d41a9",
                Email = "eduardo@gmail.com"
            };
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            Gamer secondGamer = userManager.GetGamerByEmail(gamertag);
            Assert.Equal(gamer, secondGamer);
        }

        [Fact]
        public void GetGamerByEmailErrorConnectionTest()
        {
            string gamertag = "eduardo@gmail.com";
            Gamer gamer = new Gamer
            {
                FirstName = "Eduardo",
                LastName = "Carrera",
                Gamertag = "Lalonch3ra",
                GamesWon = 0,
                ImageCode = "Icon1.jpg",
                Password = "164cdbd8614682a2cf2f7e944badcf5aa95d41a9",
                Email = "eduardo@gmail.com"
            };
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => userManager.GetGamerByEmail(gamertag));
        }

        [Fact]
        public void GetGamerByEmailFailTest()
        {
            string gamertag = "aldoJr@gmail.com";
            Gamer gamer = new Gamer
            {
                FirstName = "Eduardo",
                LastName = "Carrera",
                Gamertag = "Lalonch3ra",
                GamesWon = 0,
                ImageCode = "Icon1.jpg",
                Password = "164cdbd8614682a2cf2f7e944badcf5aa95d41a9",
                Email = "eduardo@gmail.com"
            };
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            Gamer secondGamer = userManager.GetGamerByEmail(gamertag);
            Assert.False(gamer.Equals(secondGamer));
        }

        [Fact]
        public void ChangeIconTest()
        {
            string gamertag = "aldoJr";
            string icon = "Icon4.jpg";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            int result = userManager.ChangeIcon(gamertag, icon);
            int resultExcepted = ConstantsTests.Success;
            Assert.Equal(result, resultExcepted);
        }

        [Fact]
        public void ChangeIconErrorConnectionTest()
        {
            string gamertag = "aldoJr";
            string icon = "Icon4.jpg";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => userManager.ChangeIcon(gamertag, icon));
        }

        [Fact]
        public void ChangeIconFailTest()
        {
            string gamertag = "kayloRen";
            string icon = "Icon1.jpg";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            int result = userManager.ChangeIcon(gamertag, icon);
            int resultExcepted = ConstantsTests.Failure;
            Assert.Equal(result, resultExcepted);
        }

        [Fact]
        public void GetIconTest()
        {
            string gamertag = "Lalonch3ra";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            string result = userManager.GetIcon(gamertag);
            string resultExcepted = "Icon1.jpg";
            Assert.Equal(result, resultExcepted);
        }

        [Fact]
        public void GetIconErrorConnectionTest()
        {
            string gamertag = "Lalonch3ra";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => userManager.GetIcon(gamertag));
        }

        [Fact]
        public void GetIconFailTest()
        {
            string gamertag = "YoungMiko";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            string result = userManager.GetIcon(gamertag);
            string resultExcepted = "Icon3.jpg";
            Assert.False(result.Equals(resultExcepted));
        }

        [Fact]
        public void DeleteGuestPlayerTest()
        {
            string gamertag = "BWdS3tzN";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            int result = userManager.DeleteGuestPlayer(gamertag);
            int resultExcepted = ConstantsTests.Success;
            Assert.Equal(result, resultExcepted);
        }

        [Fact]
        public void DeleteGuestPlayerErrorTestTest()
        {
            string gamertag = "BWdS3tzN";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => userManager.DeleteGuestPlayer(gamertag));
        }

        [Fact]
        public void DeleteGuestPlayerFailTest()
        {
            string gamertag = "KLSD1z";
            SpiderClueService.IUserManager userManager = new UserManagerClient();
            int result = userManager.DeleteGuestPlayer(gamertag);
            int resultExcepted = ConstantsTests.Failure;
            Assert.Equal(result, resultExcepted);
        }


    }

}