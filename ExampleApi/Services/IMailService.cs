using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace ExampleApi.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string content);
    }
    public class SendGridMailService : IMailService
    {
        IConfiguration _configuration;
        public SendGridMailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var apiKey = _configuration["EmailSettings:ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configuration["EmailSettings:FromEmailAddress"], _configuration["EmailSettings:FromEmailAddressDisplayName"]);
            var to = new EmailAddress(toEmail);
            var plainTextContent = content;
            var htmlContent = $"<p>{content}.</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            //Console.WriteLine(response.StatusCode);
        }
    }
}
