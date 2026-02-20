using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace API.Services
{
    public class EmailService
    {
        private readonly string _smtpHost = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "rami13195@gmail.com"; // Replace with your email
        private readonly string _smtpPass = "your-app-password"; // Replace with your app password

        public async Task SendOrderEmailAsync(string toEmail, string subject, string body)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress(_smtpUser);
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = false;

            using (var smtp = new SmtpClient(_smtpHost, _smtpPort))
            {
                smtp.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);
            }
        }
    }
}
