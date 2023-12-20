using System;
using System.Collections.Generic;
using GameService.Contracts;

namespace GameService.Services
{
    public partial class GameService : IEmailVerificationManager
    {
        private readonly Dictionary<string, VerificationData> verificationDictionary = new Dictionary<string, VerificationData>();
        private TimeSpan verificationCodeValidity = TimeSpan.FromMinutes(8);

        public bool GenerateVerificationCode(string email)
        {
            EmailService emailService = new EmailService();
            string verificationCode = GenerateRandomCode();
            verificationDictionary[email] = new VerificationData(verificationCode, DateTime.Now);

            bool codeProcessResult = emailService.SendEmailWithCode(email, verificationCode);

            return codeProcessResult;
        }

        public bool VerifyCode(string email, string code)
        {
            bool verificationStatus = false;
            if (verificationDictionary.ContainsKey(email))
            {
                VerificationData data = verificationDictionary[email];
                if (data.IsValid(code, verificationCodeValidity))
                {
                    verificationDictionary.Remove(email);

                    verificationStatus = true;
                }
            }

            return verificationStatus;
        }

        private string GenerateRandomCode()
        {
            int lowerBound = 100000;
            int upperBound = 1000000;

            Random random = new Random();
            int codeNumber = random.Next(lowerBound, upperBound);
            string code = codeNumber.ToString();

            return code;
        }
    }

    public class VerificationData
    {
        public string Code { get; private set; }
        public DateTime CreationTime { get; private set; }

        public VerificationData(string code, DateTime creationTime)
        {
            Code = code;
            CreationTime = creationTime;
        }

        public bool IsValid(string code, TimeSpan validityPeriod)
        {
            return Code == code && (DateTime.Now - CreationTime) <= validityPeriod;
        }
    }
}
