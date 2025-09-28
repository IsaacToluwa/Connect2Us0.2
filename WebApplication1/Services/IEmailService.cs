using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(User user, string token);
        Task SendAdminAccountDetailsAsync(User user, string username, string password);
        Task SendPaymentConfirmationAsync(int printRequestId, PaymentResult paymentResult);
        Task SendPasswordResetAsync(User user, string resetToken);
    }
}