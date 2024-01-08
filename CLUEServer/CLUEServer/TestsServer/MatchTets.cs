using System;
using System.Collections.Generic;
using System.Linq;
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
            string gamertag = "Star3oy";
            SpiderClueService.IMatchCreationManager matchCreation = new SpiderClueService.MatchCreationManagerClient();
            string result = matchCreation.CreateMatch(gamertag);
            string resultExpected = string.Empty;
            Assert.NotEqual(result, resultExpected);
        }

        [Fact]
        public void CreateMatchNotEqualsTest()
        {
            string gamertag = "Star3oy";
            string secondGamertag = "mich";
            SpiderClueService.IMatchCreationManager matchCreation = new SpiderClueService.MatchCreationManagerClient();
            string result = matchCreation.CreateMatch(gamertag);
            string secondResult = matchCreation.CreateMatch(secondGamertag);
            Assert.NotEqual(result, secondResult);
        }

    }
}
