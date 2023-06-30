using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Test.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var EmailSender = "dsawo2018@gmail.com";
            var PasswordSender = "akdlxsswsgsqgwnz";
            var Message = new MailMessage();
            Message.From = new MailAddress(EmailSender);
            Message.Subject = subject;
            Message.To.Add(email);
            Message.Body = $"<html><body>{htmlMessage}</body></html>";
            Message.IsBodyHtml = true;
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(EmailSender, PasswordSender),
                EnableSsl = true
            };
            smtpClient.Send(Message);
        }
    }
}
