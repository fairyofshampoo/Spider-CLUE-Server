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
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            bool result = friendshipManager.AreNotFriends("soobin", "Star3oy");
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
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            bool result = friendshipManager.AreNotFriends("soobin", "michito");
            Assert.False(result);
        }

        [Fact]
        public void AddFriendTestSuccess()
        {
            int resultExpected = ConstantsTests.Success;
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            int result = friendshipManager.AddFriend("Star3oy", "mich");
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
            int resultExpected = ConstantsTests.Success;
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
            int resultExpected = ConstantsTests.Success;
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            int result = friendshipManager.DeleteFriend("Logan", "Charles");
            Assert.True(resultExpected.Equals(result));
        }

        [Fact]
        public void CreateFriendRequestSuccess()
        {
            int resultExpected = ConstantsTests.Success;
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            int result = friendRequestManager.CreateFriendRequest("Star3oy", "mich");
            Assert.True(resultExpected.Equals(result));
        }

        [Fact]
        public void CreateFriendRequestErrorConnection()
        {
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendRequestManager.CreateFriendRequest("Star3oy", "mich"));
        }

        [Fact]
        public void GetFriendsRequestSuccess()
        {
            string[] resultExpected = new string[]
            {
                "TheWeeknd"
            };

            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            string[] result = friendRequestManager.GetFriendsRequest("PostMalone");
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
            Assert.Throws<EndpointNotFoundException>(() => friendRequestManager.GetFriendsRequest("PostMalone"));
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
            int resultExpected = ConstantsTests.Success;

            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            int result = friendRequestManager.ResponseFriendRequest("mich", "Star3oy", "Accepted");
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void ResponseFriendRequestErrorConnection()
        {
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendRequestManager.ResponseFriendRequest("mich", "Star3oy", "Accepted"));
        }

        [Fact]
        public void ResponseFriendRequestFail()
        {
            int resultExpected = ConstantsTests.Failure;
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            int result = friendRequestManager.ResponseFriendRequest("mich", "mich", "Accepted");
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void DeleteFriendRequestSuccess()
        {
            int resultExpected = ConstantsTests.Success;
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            int result = friendRequestManager.DeleteFriendRequest("Maneskin", "Cuco");
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void DeleteFriendRequestErrorConnection()
        {
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendRequestManager.DeleteFriendRequest("Maneskin", "Cuco"));
        }

        [Fact]
        public void DeleteFriendRequestFail()
        {
            int resultExpected = ConstantsTests.Failure;
            SpiderClueService.IFriendRequestManager friendRequestManager = new FriendRequestManagerClient();
            int result = friendRequestManager.DeleteFriendRequest("mich", "mich");
            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public void ThereIsNoFriendRequestFail()
        {
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            bool result = friendshipManager.ThereIsNoFriendRequest("CharlesLeclerc", "CarlosSainz");
            Assert.True(result);
        }

        [Fact]
        public void ThereIsNoFriendRequestErrorConnection()
        {
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => friendshipManager.ThereIsNoFriendRequest("mich", "Star3oy"));
        }

        [Fact]
        public void ThereIsNoFriendRequestSuccess()
        {
            SpiderClueService.IFriendshipManager friendshipManager = new FriendshipManagerClient();
            bool result = friendshipManager.ThereIsNoFriendRequest("CarlosSainz", "CharlesLeclerc");
            Assert.False(result);
        }

    }
}
