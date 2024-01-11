using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestsServer
{
    internal class MatchTetsConfiguration : IDisposable
    {

        public MatchTetsConfiguration() { }
        public void Dispose()
        {
            
        }
    }

    public class MatchTest : IClassFixture<MatchTetsConfiguration>
    {
        [Fact]
        public void CreateMatchTest()
        {
            string gamertag = "Lalonch3ra";
            SpiderClueService.IMatchCreationManager matchCreation = new SpiderClueService.MatchCreationManagerClient();
            string result = matchCreation.CreateMatch(gamertag);
            string resultExpected = string.Empty;
            Assert.NotEqual(result, resultExpected);
        }

        [Fact]
        public void CreateMatchErrorConnectionTest()
        {
            string gamertag = "Lalonch3ra";
            SpiderClueService.IMatchCreationManager matchCreation = new SpiderClueService.MatchCreationManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => matchCreation.CreateMatch(gamertag));
        }

        [Fact]
        public void CreateMatchNotEqualsTest()
        {
            string gamertag = "Lalonch3ra";
            string secondGamertag = "mich";
            SpiderClueService.IMatchCreationManager matchCreation = new SpiderClueService.MatchCreationManagerClient();
            string result = matchCreation.CreateMatch(gamertag);
            string secondResult = matchCreation.CreateMatch(secondGamertag);
            Assert.NotEqual(result, secondResult);
        }

    }
}
