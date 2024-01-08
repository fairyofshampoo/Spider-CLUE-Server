using System;
using System.Collections.Generic;
using TestsServer.SpiderClueService;
using Xunit;

namespace TestsServer
{
    public class WinnersTest
    {
        [Fact]
        public void GetTopGlobalWinnersTestSuccess()
        {
            SpiderClueService.IWinnersManager winnersManager = new WinnersManagerClient();

            Winner[] winners = winnersManager.GetTopGlobalWinners();
            List<String> topGlobal = new List<String>();
            foreach (var winner in winners)
            {
                topGlobal.Add(winner.Gamertag);
            }

            List<String> actualWinners = new List<String>()
            {
                "Star3oy",
                "mich",
            };

            Assert.Equal(topGlobal, actualWinners);
        }

        [Fact]
        public void GetTopGlobalWinnersTestFail()
        {
            SpiderClueService.IWinnersManager winnersManager = new WinnersManagerClient();

            Winner[] winners = winnersManager.GetTopGlobalWinners();
            List<String> topGlobal = new List<String>();
            foreach (var winner in winners)
            {
                topGlobal.Add(winner.Gamertag);
            }

            List<String> actualWinners = new List<String>()
            {
                "soobin",
                "mich"
            };

            Assert.NotEqual(topGlobal, actualWinners);
        }
    }
}
