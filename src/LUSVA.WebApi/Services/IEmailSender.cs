using System.Threading.Tasks;

namespace LUSVA.WebApi.Services
{
  public interface IEmailSender
  {
    Task SendEmailAsync(string email, string subject, string message);
  }
}