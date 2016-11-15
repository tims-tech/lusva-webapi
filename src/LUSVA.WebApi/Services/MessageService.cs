using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace LUSVA.WebApi.Services
{
  public class MessageService : IEmailSender
  {
    public async Task SendEmailAsync(string email, string subject, string message)
    {
      var mail = new MimeMessage();

      mail.From.Add(new MailboxAddress("LUSVA Support", "no-reply@lusva.org"));
      mail.To.Add(new MailboxAddress("", email));
      mail.Subject = subject;
      mail.Body = new TextPart(TextFormat.Plain) { Text = message };

      using (var client = new SmtpClient())
      {
        client.LocalDomain = "localhost";
        await client.ConnectAsync("smtp.google.com", 25, SecureSocketOptions.StartTls).ConfigureAwait(false);
        await client.SendAsync(mail).ConfigureAwait(false);
        await client.DisconnectAsync(true).ConfigureAwait(false);
      }
    }
  }
}