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
    public class EmailTest
    {
        [Fact]
        public void SendInvitationTestSuccess()
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
        public void SendInvitationTestFail()
        {
            bool result = false;
            string emailInvalid = "lalocel09gmail.com";
            string code = "CODE12";
            string gamertag = "michito";

            SpiderClueService.IInvitationManager invitationManager = new InvitationManagerClient();
            result = invitationManager.SendInvitation(emailInvalid, code, gamertag);
            Assert.False(result);
        }

        [Fact]
        public void GenerateVerificationCodeTestSuccess()
        {
            bool result = false;
            string emailValid = "lalocel09@gmail.com";

            SpiderClueService.IEmailVerificationManager emailVerificationManager = new EmailVerificationManagerClient();
            result = emailVerificationManager.GenerateVerificationCode(emailValid);
            Assert.True(result);
        }

        [Fact]
        public void GenerateVerificationCodeTestFail()
        {
            bool result = false;
            string emailInvalid = "lalocel09gmail.com";

            SpiderClueService.IEmailVerificationManager emailVerificationManager = new EmailVerificationManagerClient();
            result = emailVerificationManager.GenerateVerificationCode(emailInvalid);
            Assert.False(result);
        }

        [Fact]
        public void VerifyCodeTestSuccess()
        {
            bool result = false;
            string emailValid = "lalocel09@gmail.com";
            string code = "418034";

            SpiderClueService.IEmailVerificationManager emailVerificationManager = new EmailVerificationManagerClient();
            result = emailVerificationManager.VerifyCode(emailValid, code);
            Assert.False(result);
        }

        [Fact]
        public void VerifyCodeTestFail()
        {
            bool result = false;
            string emailValid = "lalocel09@gmail.com";
            string wrongCode = "123";

            SpiderClueService.IEmailVerificationManager emailVerificationManager = new EmailVerificationManagerClient();
            result = emailVerificationManager.VerifyCode(emailValid, wrongCode);
            Assert.False(result);
        }
        public void SendInvitationTestErrorConnection()
        {
            string emailValid = "lalocel09@gmail.com";
            string code = "CODE12";
            string gamertag = "michito";
            SpiderClueService.IInvitationManager invitationManager = new InvitationManagerClient();
            Assert.Throws<EndpointNotFoundException>(() => invitationManager.SendInvitation(emailValid, code, gamertag));
        }

    }
}
