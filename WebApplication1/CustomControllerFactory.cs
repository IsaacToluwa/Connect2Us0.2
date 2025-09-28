using System;
using System.Web.Mvc;
using System.Web.Routing;
using WebApplication1.Services;
using WebApplication1.Models; // Add this using directive

namespace WebApplication1
{
    public class CustomControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return base.GetControllerInstance(requestContext, controllerType);
            }

            // Manually resolve dependencies for AccountController
            if (controllerType == typeof(Controllers.AccountController))
            {
                var context = new BookstoreDbContext(); // Create a new DbContext instance
                var notificationService = new NotificationService(context); // Instantiate NotificationService with context
                var emailService = new EmailService(context, notificationService); // Instantiate EmailService with context and notificationService
                return new Controllers.AccountController(emailService, notificationService);
            }

            // For other controllers, use the default factory behavior
            if (controllerType == typeof(Controllers.PrintingController))
            {
                var context = new BookstoreDbContext();
                var notificationService = new NotificationService(context);
                var emailService = new EmailService(context, notificationService);
                return new Controllers.PrintingController(context, emailService, notificationService);
            }
            return base.GetControllerInstance(requestContext, controllerType);
        }
    }
}