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
    public class EmailTest
    {
        [Fact]
        public void AreNotFriendsTestSuccess()
        {
            bool result = false;
            string emailValid = "lalocel09@gmail.com";
            string code = "CODE12";
            string gamertag = "michito";

            SpiderClueService.IInvitationManager invitationManager = new InvitationManagerClient();
            result = invitationManager.SendInvitation(emailValid, code, gamertag);
            Assert.True(result);
        }

        [Fact]
        public void AreNotFriendsTestErrorConnection()
        {
            string emailValid = "lalocel09@gmail.com";
            string code = "CODE12";
            string gamertag = "michito";
            SpiderClueService.IInvitationManager invitationManager = new InvitationManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => invitationManager.SendInvitation(emailValid, code, gamertag));
        }

    }
}
