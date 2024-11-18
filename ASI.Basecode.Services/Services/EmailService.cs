using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class EmailService : IEmailService
    {
        public EmailService()
        {

        }
        public void SendEmail(string to, string subject, string body)
        {
            var from = "bookr2117@gmail.com";
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("bookr2117@gmail.com", "fnlg vxkn ltkc hyhw"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(to);
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email send failed: {ex.Message}");
            }
        }
        public void SendPasswordResetEmail(string email, string resetToken)
        {
            var resetLink = $"https://localhost:2317/Account/ResetPassword?token={resetToken}";
            var subject = "Password Reset Request";
            var body = $"Click the link below to reset your password:\n\n{resetLink}";

            SendEmail(email, subject, body);
        }

    }
}
