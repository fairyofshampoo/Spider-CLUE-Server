using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using GameService.Contracts;
using System.IO;

namespace GameService.Services
{
    public partial class GameService : IEmailVerificationManager
    {
        private Dictionary<string, VerificationData> verificationDictionary = new Dictionary<string, VerificationData>();
        private TimeSpan verificationCodeValidity = TimeSpan.FromMinutes(8);

        public bool GenerateVerificationCode(string email)
        {
            string verificationCode = GenerateRandomCode();
            verificationDictionary[email] = new VerificationData(verificationCode, DateTime.Now);

            bool codeProcessResult = SendEmailWithCode(email, verificationCode);

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

        private bool SendEmailWithCode(string toEmail, string code)
        {
            bool emailProcessResult = false;
            try
            {
                string fromMail = "soobluving@gmail.com";
                string fromPassword = "flmcnbzxpwcnxudz";

                string emailTemplate = File.ReadAllText("Utilities/EmailTemplate-ES-MX.html");

                string emailBody = emailTemplate.Replace("{code}", code);

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = "Code Verification from Spider Clue";
                message.To.Add(new MailAddress(toEmail));
                message.Body = emailBody;
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true
                };

                smtpClient.Send(message);

                emailProcessResult = true;
            }
            catch (SmtpException smtpException)
            {
                // Aquí va un log jejeje
                Console.WriteLine("Error SMTP: " + smtpException.Message);
            }

            return emailProcessResult;
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
