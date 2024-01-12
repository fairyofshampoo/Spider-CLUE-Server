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
            string emailValid = "lalocel09@gmail.com";
            string code = "CODE12";
            string gamertag = "michito";
            SpiderClueService.IInvitationManager invitationManager = new InvitationManagerClient();
            bool result = invitationManager.SendInvitation(emailValid, code, gamertag);
            Assert.True(result);
        }

        [Fact]
        public void SendInvitationTestFail()
        {
            string emailInvalid = "lalocel09gmail.com";
            string code = "CODE12";
            string gamertag = "michito";
            SpiderClueService.IInvitationManager invitationManager = new InvitationManagerClient();
            bool result = invitationManager.SendInvitation(emailInvalid, code, gamertag);
            Assert.False(result);
        }

        [Fact]
        public void GenerateVerificationCodeTestSuccess()
        {
            string emailValid = "lalocel09@gmail.com";
            SpiderClueService.IEmailVerificationManager emailVerificationManager = new EmailVerificationManagerClient();
            bool result = emailVerificationManager.GenerateVerificationCode(emailValid);
            Assert.True(result);
        }

        [Fact]
        public void GenerateVerificationCodeTestFail()
        {
            string emailInvalid = "lalocel09gmail.com";
            SpiderClueService.IEmailVerificationManager emailVerificationManager = new EmailVerificationManagerClient();
            bool result = emailVerificationManager.GenerateVerificationCode(emailInvalid);
            Assert.False(result);
        }

        [Fact]
        public void VerifyCodeTestSuccess()
        {
            string emailValid = "lalocel09@gmail.com";
            string code = "418034";
            SpiderClueService.IEmailVerificationManager emailVerificationManager = new EmailVerificationManagerClient();
            bool result = emailVerificationManager.VerifyCode(emailValid, code);
            Assert.True(result);
        }

        [Fact]
        public void VerifyCodeTestFail()
        {
            string emailValid = "lalocel09@gmail.com";
            string wrongCode = "123";
            SpiderClueService.IEmailVerificationManager emailVerificationManager = new EmailVerificationManagerClient();
            bool result = emailVerificationManager.VerifyCode(emailValid, wrongCode);
            Assert.False(result);
        }

        [Fact]
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
