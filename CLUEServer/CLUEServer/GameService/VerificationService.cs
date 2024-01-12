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
        private static readonly Dictionary<string, VerificationData> verificationDictionary = new Dictionary<string, VerificationData>();

        /// <summary>
        /// Generates a verification code, associates it with the provided email, sends an email with the code,
        /// and returns the result of the code generation and email sending process.
        /// </summary>
        /// <param name="email">The email address to which the verification code will be sent.</param>
        /// <returns>True if the code generation and email sending process is successful; otherwise, false.</returns>
        public bool GenerateVerificationCode(string email)
        {
            EmailService emailService = new EmailService();
            string verificationCode = GenerateRandomCode();
            Stopwatch localStopwatch = Stopwatch.StartNew();
            verificationDictionary[email] = new VerificationData(verificationCode, localStopwatch);
            bool codeProcessResult = emailService.SendEmailWithCode(email, verificationCode);

            return codeProcessResult;
        }

        /// <summary>
        /// Verifies the provided code for the given email. If the code is valid within the specified validity period,
        /// removes the verification data associated with the email and returns true; otherwise, returns false.
        /// </summary>
        /// <param name="email">The email address to verify the code for.</param>
        /// <param name="code">The verification code to be checked.</param>
        /// <returns>True if the code is valid and the verification is successful; otherwise, false.</returns>
        public bool VerifyCode(string email, string code)
        {
            bool verificationStatus = false;
            TimeSpan verificationCodeValidity = TimeSpan.FromMinutes(8);

            if (verificationDictionary.ContainsKey(email))
            {
                VerificationData data = verificationDictionary[email];
                if (data.IsValid(code, verificationCodeValidity))
                {
                    verificationDictionary.Remove(email);
                    verificationStatus = true;
                }
            }0

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

    /// <summary>
    /// Represents data for verification, including a code and a local stopwatch.
    /// </summary>
    public class VerificationData
    {
        /// <summary>
        /// Gets the verification code.
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Gets the local stopwatch associated with the verification.
        /// </summary>
        public Stopwatch LocalStopwatch { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerificationData"/> class with the specified code and local stopwatch.
        /// </summary>
        /// <param name="code">The verification code.</param>
        /// <param name="localStopwatch">The local stopwatch associated with the verification.</param>
        public VerificationData(string code, Stopwatch localStopwatch)
        {
            Code = code;
            LocalStopwatch = localStopwatch;
        }

        /// <summary>
        /// Checks if the provided code matches the stored code and if the elapsed time on the local stopwatch is within the specified validity period.
        /// </summary>
        /// <param name="code">The code to be verified.</param>
        /// <param name="validityPeriod">The maximum allowed time span for the verification to be considered valid.</param>
        /// <returns>True if the verification is valid; otherwise, false.</returns>
        public bool IsValid(string code, TimeSpan validityPeriod)
        {
            bool result = Code == code && LocalStopwatch.Elapsed <= validityPeriod;
            return result;
        }
    }

}
