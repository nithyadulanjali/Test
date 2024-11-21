using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace MSLSystem.Models
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SendEmail(string toEmail, string otp)
        {
            try
            {
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromPassword = _configuration["EmailSettings:Password"];
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

                var fromAddress = new MailAddress(fromEmail);
                var toAddress = new MailAddress(toEmail);
                const string subject = "Your OTP Code";
                string body = $"Your OTP code is {otp}";

                using (var smtp = new SmtpClient(smtpHost, smtpPort))
                {
                    smtp.EnableSsl = true; 
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(fromEmail, fromPassword); 

                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        smtp.Send(message);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
