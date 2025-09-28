using System;
using System.Configuration;
using System.Net.Mail;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Helpers;
using System.Collections.Generic;

namespace WebApplication1.Services
{
    public class EmailService : IEmailService
    {
        private readonly INotificationService _notificationService;
        private readonly BookstoreDbContext _db;
        private readonly string _adminEmail;

        public EmailService(BookstoreDbContext context, INotificationService notificationService)
        {
            _notificationService = notificationService;
            _db = context;
            _adminEmail = ConfigurationManager.AppSettings["AdminEmail"];
        }

        public async Task SendEmailConfirmationAsync(User user, string token)
        {
            var subject = "Confirm your email";
            var placeholders = new Dictionary<string, string>
            {
                { "FirstName", user.FirstName },
                { "UserId", user.UserId.ToString() },
                { "Token", token }
            };
            var body = NotificationTemplates.GenerateEmailConfirmation(placeholders);
            await _notificationService.SendEmailAsync(user.Email, subject, body);
        }

        public async Task SendAdminAccountDetailsAsync(User user, string username, string password)
        {
            var subject = "Your New Admin Account Details";
            var placeholders = new Dictionary<string, string>
            {
                { "FirstName", user.FirstName },
                { "Username", username },
                { "Password", password }
            };
            var body = NotificationTemplates.GetAdminAccountDetailsBody(placeholders);
            await _notificationService.SendEmailAsync(user.Email, subject, body);
        }

        public async Task SendPaymentConfirmationAsync(int printRequestId, PaymentResult paymentResult)
        {
            var printRequest = await _db.PrintRequests.FindAsync(printRequestId);
            if (printRequest == null)
            {
                System.Diagnostics.Debug.WriteLine($"Print request with ID {printRequestId} not found.");
                return;
            }

            var order = await _db.Orders.FindAsync(printRequest.OrderId);
            if (order == null)
            {
                System.Diagnostics.Debug.WriteLine($"Order with ID {printRequest.OrderId} not found for print request {printRequestId}.");
                return;
            }

            var user = await _db.Users.FindAsync(order.CustomerId);
            if (user == null)
            {
                System.Diagnostics.Debug.WriteLine($"User with ID {order.CustomerId} not found for order {order.OrderId}.");
                return;
            }

            var subject = "Payment Confirmation for Print Request #" + printRequestId;
            var placeholders = new Dictionary<string, string>
            {
                { "FirstName", user.FirstName },
                { "Amount", paymentResult.Amount.ToString("C") },
                { "OrderId", order.OrderId.ToString() },
                { "PaymentDate", paymentResult.ProcessedAt.ToString("g") }
            };
            var body = NotificationTemplates.GeneratePaymentConfirmation(placeholders);
            await _notificationService.SendEmailAsync(user.Email, subject, body);
        }

        public async Task SendPasswordResetAsync(User user, string resetToken)
        {
            var subject = "Reset Your Password";
            var placeholders = new Dictionary<string, string>
            {
                { "FirstName", user.FirstName },
                { "ResetLink", $"http://localhost:8083/Account/ResetPassword?userId={user.UserId}&token={resetToken}" }
            };
            var body = NotificationTemplates.GeneratePasswordResetEmail(placeholders);
            await _notificationService.SendEmailAsync(user.Email, subject, body);
        }
    }
}