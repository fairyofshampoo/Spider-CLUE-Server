using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestsServer
{
    public class SessionTest
    {
        [Fact]
        public void ConnectTest()
        {
            string gamertag = "MacMiller";
            SpiderClueService.ISessionManager sessionManager = new SpiderClueService.SessionManagerClient();
            int result = sessionManager.Connect(gamertag);
            int resultExpceted = ConstantsTests.Success;
            Assert.Equal(result, resultExpceted);   
        }

        [Fact]
        public void ConnectErrorConnectionTest()
        {
            string gamertag = "MacMiller";
            SpiderClueService.ISessionManager sessionManager = new SpiderClueService.SessionManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => sessionManager.Connect(gamertag));
        }

        [Fact]
        public void ConnectFailTest()
        {
            string gamertag = "MacMiller";
            SpiderClueService.ISessionManager sessionManager = new SpiderClueService.SessionManagerClient();
            int result = sessionManager.Connect(gamertag);
            int resultExpceted = ConstantsTests.Failure;
            Assert.Equal(result, resultExpceted);
        }

        [Fact]
        public void IsGamerAlreadyOnlineTest()
        {
            string gamertag = "MacMiller";
            SpiderClueService.ISessionManager sessionManager = new SpiderClueService.SessionManagerClient();
            bool result = sessionManager.IsGamerAlreadyOnline(gamertag);
            Assert.True(result);
        }

        [Fact]
        public void IsGamerAlreadyOnlineErrorConnectionTest()
        {
            string gamertag = "MacMiller";
            SpiderClueService.ISessionManager sessionManager = new SpiderClueService.SessionManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => sessionManager.IsGamerAlreadyOnline(gamertag));
        }

        [Fact]
        public void IsGamerAlreadyOnlineFailTest()
        {
            string gamertag = "Pardo";
            SpiderClueService.ISessionManager sessionManager = new SpiderClueService.SessionManagerClient();
            bool result = sessionManager.IsGamerAlreadyOnline(gamertag);
            Assert.False(result);
        }

    }
}
