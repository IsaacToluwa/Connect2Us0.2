using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail; // Added for MailMessage and SmtpClient
using WebApplication1.Models;
using WebApplication1.Helpers;
using System.Configuration;

namespace WebApplication1.Services
{
    public interface INotificationService
    {
        void CreateNotificationForUser(int userId, string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Normal, string relatedLink = null);
        void CreateNotificationForAllUsers(string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Normal, string relatedLink = null);
        void CreateNotificationForRole(string role, string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Normal, string relatedLink = null);
        Task SendTemplatedNotificationAsync(int userId, string templateName, Dictionary<string, string> placeholders, string link = null);
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

    public class NotificationService : INotificationService
    {
        private readonly BookstoreDbContext _context;

        public NotificationService(BookstoreDbContext context)
        {
            _context = context;
        }

        // Synchronous wrapper methods for easier use in controllers
        public void CreateNotificationForUser(int userId, string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Normal, string relatedLink = null)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    NotificationType = type.ToString(),
                    Priority = priority.ToString(),
                    RelatedLink = relatedLink,
                    CreatedDate = DateTime.Now,
                    IsRead = false
                };

                _context.Notifications.Add(notification);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid breaking the main flow
                System.Diagnostics.Debug.WriteLine($"Error creating notification: {ex.Message}");
            }
        }

        public void CreateNotificationForAllUsers(string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Normal, string relatedLink = null)
        {
            try
            {
                var users = _context.Users.ToList();
                foreach (var user in users)
                {
                    var notification = new Notification
                    {
                        UserId = user.UserId,
                        Title = title,
                        Message = message,
                        NotificationType = type.ToString(),
                        Priority = priority.ToString(),
                        RelatedLink = relatedLink,
                        CreatedDate = DateTime.Now,
                        IsRead = false
                    };

                    _context.Notifications.Add(notification);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid breaking the main flow
                System.Diagnostics.Debug.WriteLine($"Error creating notifications for all users: {ex.Message}");
            }
        }

        public void CreateNotificationForRole(string role, string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Normal, string relatedLink = null)
        {
            try
            {
                var users = _context.Users.Where(u => u.Role == role).ToList();
                foreach (var user in users)
                {
                    var notification = new Notification
                    {
                        UserId = user.UserId,
                        Title = title,
                        Message = message,
                        NotificationType = type.ToString(),
                        Priority = priority.ToString(),
                        RelatedLink = relatedLink,
                        CreatedDate = DateTime.Now,
                        IsRead = false
                    };

                    _context.Notifications.Add(notification);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid breaking the main flow
                System.Diagnostics.Debug.WriteLine($"Error creating notifications for role {role}: {ex.Message}");
            }
        }

        public async Task SendTemplatedNotificationAsync(int userId, string templateName, Dictionary<string, string> placeholders, string link = null)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine($"User with ID {userId} not found for templated notification.");
                    return;
                }

                // Assuming NotificationTemplates.GetTemplate returns the full message content
                string message = Helpers.NotificationTemplates.GetTemplate(templateName, placeholders);

                var notification = new Notification
                {
                    UserId = userId,
                    Title = templateName, // Or a more descriptive title based on templateName
                    Message = message,
                    NotificationType = NotificationType.Info.ToString(), // Default to Info, can be made dynamic
                    Priority = NotificationPriority.Normal.ToString(), // Default to Normal, can be made dynamic
                    RelatedLink = link,
                    CreatedDate = DateTime.Now,
                    IsRead = false
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending templated notification: {ex.Message}");
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
                int smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                string smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
                string smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
                string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
                string fromName = ConfigurationManager.AppSettings["FromName"];

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail, fromName);
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(smtpHost, smtpPort))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending email: {ex.Message}");
                // Optionally re-throw or log more severely
            }
        }
    }
}