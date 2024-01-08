using System;
using System.IO;
using System.Net.Mail;
using System.Net;
using GameService.Utilities;

namespace GameService
{
    public class EmailService
    {
        public EmailService(){}

        public bool SendEmailWithCode(string toEmail, string code)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            bool result = false;
            try
            {
                string templatePath = GetTemplatePath("EmailTemplate-ES-MX.html");
                string emailTemplate = File.ReadAllText(templatePath);
                string emailBody = emailTemplate.Replace("{code}", code);

                result = SendEmail(toEmail, "Code Verification from Spider Clue", emailBody);
            } 
            catch (FileNotFoundException fileNotFoundException)
            {
                logger.LogError(fileNotFoundException);
                result = false;
            }
            catch (IOException ioException)
            {
                logger.LogError(ioException);
                result = false;
            }
            return result;
        }

        public bool SendEmailWithInvitation(string email, string matchCode, string gamertag)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            bool result = false;
            try
            {
                string templatePath = GetTemplatePath("EmailInvitationTemplate-ES-MX.html");
                string emailTemplate = File.ReadAllText(templatePath);
                string emailBody = emailTemplate.Replace("{inviter}", gamertag).Replace("{code}", matchCode);

                result = SendEmail(email, "Invitation from Spider Clue", emailBody);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                logger.LogError(fileNotFoundException);
                result = false;
            }
            catch (IOException ioException)
            {
                logger.LogError(ioException);
                result = false;
            }
            return result;
        }

        private string GetTemplatePath(string templateName)
        {
            string PathDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string PathServerDirectory = Path.GetFullPath(Path.Combine(PathDirectory, "../../../"));
            return Path.Combine(PathServerDirectory, "GameService/Utilities", templateName);
        }

        private bool SendEmail(string toEmail, string subject, string emailBody)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            bool emailProcessResult = false;
            try
            {
                string fromMail = "soobluving@gmail.com";
                string fromPassword = Environment.GetEnvironmentVariable("EMAIL_CLUE_PASSWORD");

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = subject;
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

            catch (FormatException formatException)
            {
                logger.LogError(formatException);
                emailProcessResult = false;
            }

            catch (SmtpFailedRecipientException failedRecipientException)
            {
                logger.LogError(failedRecipientException);
                emailProcessResult = false;
            }
            catch (SmtpException smtpException)
            {
                logger.LogError(smtpException);
                emailProcessResult = false;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                logger.LogError(invalidOperationException);
                emailProcessResult = false;
            }

            return emailProcessResult;
        }
    }
}
