using System.Threading.Tasks;

namespace LUSVA.WebApi.Services
{
  public class MessageService : IEmailSender
  {
    public Task SendEmailAsync(string email, string subject, string message)
    {
      throw new System.NotImplementedException();
    }
  }
}