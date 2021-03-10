using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using System.Threading.Tasks;
using ToDoList.Models;

namespace ToDoList.Infrastructure
{
    public interface ISendEmail
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class SendEmail : ISendEmail
    {
        private readonly EmailSettings _emailSettings;
        private readonly IHostingEnvironment _env;

        public SendEmail(EmailSettings emailSettings, IHostingEnvironment env)
        {
            _emailSettings = emailSettings;
            _env = env;
        }
        public async Task SendEmailAsync(string email, string subject, string body)
        {

            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress(_emailSettings.SmtpUsername));
            message.To.Add(new MailboxAddress(email));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = body
            };

            using (SmtpClient client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                if (_env.IsDevelopment())
                {
                    await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, true);
                }
                else
                {
                    await client.ConnectAsync(_emailSettings.SmtpServer);
                }

                await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);

            }
        }
    }
}
