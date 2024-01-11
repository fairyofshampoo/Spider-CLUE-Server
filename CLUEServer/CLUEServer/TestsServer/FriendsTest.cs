using DataBaseManager;
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
            
        }

        public void Dispose()
        {

        }
    }
    public class FriendsTest : IClassFixture<FriendTestConfiguration>
    {
        FriendTestConfiguration Configuration;

        [Fact]
        public void AreNotFriendsTestSuccess()
        {
            string gamertag = "soobin";
            string secondGamertag = "Star3oy";
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            bool result = friendshipManager.AreNotFriends(gamertag, secondGamertag);
            Assert.True(result);
        }

        [Fact]
        public void AreNotFriendsTestErrorConnection()
        {
            string gamertag = "soobin";
            string secondGamertag = "Star3oy";
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendshipManager.AreNotFriends(gamertag, secondGamertag));
        }

        [Fact]
        public void AreNotFriendsTestFalse()
        {
            string gamertag = "soobin";
            string secondGamertag = "michito";
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            bool result = friendshipManager.AreNotFriends(gamertag, secondGamertag);
            Assert.False(result);
        }

        [Fact]
        public void AddFriendTestSuccess()
        {
            string gamertag = "Star3oy";
            string secondGamertag = "mich";
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            int result = friendshipManager.AddFriend(gamertag, secondGamertag);
            int resultExpected = ConstantsTests.Success;
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void AddFriendTestErrorConnection()
        {
            string gamertag = "Star3oy";
            string secondGamertag = "mich";
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendshipManager.AddFriend(gamertag, secondGamertag));
        }

        [Fact]
        public void GetFriendListSuccess()
        {
            string[] resultExpected = new string[]
            {
                "michito"
            };
            string gamertag = "soobin";
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            string[] result = friendshipManager.GetFriendList(gamertag);
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void GetFriendListTestConnectionError()
        {
            string[] resultExpected = new string[]
            {
                "michito"
            };
            string gamertag = "soobin";
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendshipManager.GetFriendList(gamertag));
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
            int resultExpected = ConstantsTests.Success;
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            string gamertag = "Star3oy";
            string secondGamertag = "michito";
            int result = friendshipManager.DeleteFriend(gamertag, secondGamertag);
            Assert.False(resultExpected.Equals(result));
        }

        [Fact]
        public void DeleteFriendErrorConnection()
        {
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            string gamertag = "Logan";
            string secondGamertag = "Charles";
            Assert.Throws<EndpointNotFoundException>(() => friendshipManager.DeleteFriend(gamertag, secondGamertag));
        }

        [Fact]
        public void DeleteFriendSuccess()
        {
            int resultExpected = ConstantsTests.Success;
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            string gamertag = "Logan";
            string secondGamertag = "Charles";
            int result = friendshipManager.DeleteFriend(gamertag, secondGamertag);
            Assert.True(resultExpected.Equals(result));
        }

        [Fact]
        public void CreateFriendRequestSuccess()
        {
            int resultExpected = ConstantsTests.Success;
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            string gamertag = "Star3oy";
            string secondGamertag = "mich";
            int result = friendRequestManager.CreateFriendRequest(gamertag, secondGamertag);
            Assert.True(resultExpected.Equals(result));
        }

        [Fact]
        public void CreateFriendRequestErrorConnection()
        {
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            string gamertag = "Star3oy";
            string secondGamertag = "mich";
            Assert.Throws<EndpointNotFoundException>(() => friendRequestManager.CreateFriendRequest(gamertag, secondGamertag));
        }

        [Fact]
        public void GetFriendsRequestSuccess()
        {
            string[] resultExpected = new string[]
            {
                "TheWeeknd"
            };

            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            string gamertag = "PostMalone";
            string[] result = friendRequestManager.GetFriendsRequest(gamertag);
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void GetFriendsRequestErrorConnection()
        {
            string[] resultExpected = new string[]
            {
                "TheWeeknd"
            };
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            string gamertag = "PostMalone";
            Assert.Throws<EndpointNotFoundException>(() => friendRequestManager.GetFriendsRequest(gamertag));
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
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            string gamertag = "mich";
            string secondGamertag = "Star3oy";
            string friendRequestResponse = "Accepted";
            int result = friendRequestManager.ResponseFriendRequest(gamertag, secondGamertag, friendRequestResponse);
            int resultExpected = ConstantsTests.Success;    
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void ResponseFriendRequestErrorConnection()
        {
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            string gamertag = "mich";
            string secondGamertag = "Star3oy";
            string friendRequestResponse = "Accepted";
            Assert.Throws<EndpointNotFoundException>(() => friendRequestManager.ResponseFriendRequest(gamertag, secondGamertag, friendRequestResponse));
        }

        [Fact]
        public void ResponseFriendRequestFail()
        {
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            string gamertag = "mich";
            string friendResquestResponse = "Accepted";
            int result = friendRequestManager.ResponseFriendRequest(gamertag, gamertag, friendResquestResponse);
            int resultExpected = ConstantsTests.Failure;
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void DeleteFriendRequestSuccess()
        {
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            string gamertag = "Maneskin";
            string secondGamertag = "Cuco";
            int result = friendRequestManager.DeleteFriendRequest(gamertag, secondGamertag);
            int resultExpected = ConstantsTests.Success;
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void DeleteFriendRequestErrorConnection()
        {
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            string gamertag = "Maneskin";
            string secondGamertag = "Cuco";
            Assert.Throws<EndpointNotFoundException>(() => friendRequestManager.DeleteFriendRequest(gamertag, secondGamertag));
        }

        [Fact]
        public void DeleteFriendRequestFail()
        {
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            string gamertag = "mich";
            int result = friendRequestManager.DeleteFriendRequest(gamertag, gamertag);
            int resultExpected = ConstantsTests.Failure;
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void ThereIsNoFriendRequestFail()
        {
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            string gamertag = "CharlesLeclerc";
            string secondGamertag = "CarlosSainz";
            bool result = friendshipManager.ThereIsNoFriendRequest(gamertag, secondGamertag);
            Assert.True(result);
        }

        [Fact]
        public void ThereIsNoFriendRequestErrorConnection()
        {
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            string gamertag = "CharlesLeclerc";
            string secondGamertag = "CarlosSainz";
            Assert.Throws<EndpointNotFoundException>(() => friendshipManager.ThereIsNoFriendRequest(gamertag, secondGamertag));
        }

        [Fact]
        public void ThereIsNoFriendRequestSuccess()
        {
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            string gamertag = "CarlosSainz";
            string secondGamertag = "CharlesLeclerc";
            bool result = friendshipManager.ThereIsNoFriendRequest(gamertag, secondGamertag);
            Assert.False(result);
        }

    }
}
