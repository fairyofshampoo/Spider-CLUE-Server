using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GameService.Utilities;

namespace GameService
{
    public class EmailService
    {
        public EmailService(){}

        public bool SendEmailWithCode(string toEmail, string code)
        {
            string templatePath = GetTemplatePath("EmailTemplate-ES-MX.html");
            string emailTemplate = File.ReadAllText(templatePath);
            string emailBody = emailTemplate.Replace("{code}", code);

            return SendEmail(toEmail, "Code Verification from Spider Clue", emailBody);
        }
        public bool SendEmailWithInvitation(string email, string matchCode, string gamertag)
        {

            string templatePath = GetTemplatePath("EmailInvitationTemplate-ES-MX.html");
            string emailTemplate = File.ReadAllText(templatePath);
            string emailBody = string.Format(emailTemplate, gamertag, matchCode);

            return SendEmail(email, "Invitation from Spider Clue", emailBody);
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
            catch (SmtpException smtpException)
            {
                logger.LogError(smtpException);
                emailProcessResult = false;
            }
            catch (Exception exception)
            {
                logger.LogFatal(exception);
            }

            return emailProcessResult;
        }

        private string GetTemplatePath(string templateName)
        {
            string PathDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string PathServerDirectory = Path.GetFullPath(Path.Combine(PathDirectory, "../../../"));
            return Path.Combine(PathServerDirectory, "GameService/Utilities", templateName);
        }
    }
}
