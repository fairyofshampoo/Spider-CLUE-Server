using System;
using System.Collections.Generic;
using System.ServiceModel;
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
                "Logan",
                "Charles",
                "MutanX23"
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
                "mich",
                "Star3oy"
            };

            Assert.NotEqual(topGlobal, actualWinners);
        }


        [Fact]
        public void GetTopGlobalWinnersTestErrprConnection()
        {
            SpiderClueService.IWinnersManager winnersManager = new WinnersManagerClient();

            Assert.Throws<EndpointNotFoundException>(() => winnersManager.GetTopGlobalWinners());
            List<String> topGlobal = new List<String>();

            List<String> actualWinners = new List<String>()
            {
                "Logan",
                "Charles",
                "MutanX23"
            };
        }


    }
}
