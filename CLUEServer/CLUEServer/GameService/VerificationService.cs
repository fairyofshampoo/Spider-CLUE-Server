using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using GameService.Contracts;
using GameService.Utilities;

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
            Stopwatch localStopwatch = Stopwatch.StartNew();
            verificationDictionary[email] = new VerificationData(verificationCode, localStopwatch);

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
            LoggerManager logger = new LoggerManager(this.GetType());
            string code = string.Empty;
            int lowerBound = 100000;
            int upperBound = 1000000;

            try
            {
                using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                {
                    byte[] randomNumber = new byte[4];
                    rng.GetBytes(randomNumber);

                    int codeNumber = BitConverter.ToInt32(randomNumber, 0) & Int32.MaxValue;
                    int range = upperBound - lowerBound;
                    int scaledNumber = lowerBound + (codeNumber % range);

                    code = scaledNumber.ToString();
                }
            }
            catch (CryptographicException cryptographicException)
            {
                logger.LogError(cryptographicException);
            }

            return code;
        }

    }

    public class VerificationData
    {
        public string Code { get; private set; }
        public Stopwatch LocalStopwatch { get; private set; }

        public VerificationData(string code, Stopwatch localStopwatch)
        {
            Code = code;
            LocalStopwatch = localStopwatch;
        }

        public bool IsValid(string code, TimeSpan validityPeriod)
        {
            return Code == code && LocalStopwatch.Elapsed <= validityPeriod;
        }
    }
}
