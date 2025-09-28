using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Helpers
{
    public static class NotificationTemplates
    {
        public static string GetTemplate(string templateName, Dictionary<string, string> placeholders)
        {
            string template = "";
            switch (templateName)
            {
                case "WelcomeEmail":
                    template = GenerateWelcomeEmail(placeholders);
                    break;
                case "AdminCreatedAccountWelcome":
                    template = GenerateAdminCreatedAccountWelcome(placeholders);
                    break;
                case "OrderConfirmation":
                    template = GenerateOrderConfirmation(placeholders);
                    break;
                case "OrderStatusUpdate":
                    template = GenerateOrderStatusUpdate(placeholders);
                    break;
                case "PasswordReset":
                    template = GeneratePasswordReset(placeholders);
                    break;
                case "PasswordResetConfirmation":
                    template = GeneratePasswordResetConfirmation(placeholders);
                    break;
                case "EmailConfirmation":
                    template = GenerateEmailConfirmation(placeholders);
                    break;
                case "LowStockAlert":
                    template = GenerateLowStockAlert(placeholders);
                    break;
                case "PaymentReceipt":
                    template = GeneratePaymentReceipt(placeholders);
                    break;
                case "PaymentConfirmation":
                    template = GeneratePaymentConfirmation(placeholders);
                    break;
                case "PrintOrderConfirmation":
                    template = GeneratePrintOrderConfirmation(placeholders);
                    break;
                case "PrintingStatusUpdate":
                    template = GeneratePrintingStatusUpdate(placeholders);
                    break;
                case "PaymentStatusUpdate":
                    template = GeneratePaymentStatusUpdate(placeholders);
                    break;
                case "AdminAccountDetails":
                    template = GetAdminAccountDetailsBody(placeholders);
                    break;
            }

            foreach (var placeholder in placeholders)
            {
                template = template.Replace("{" + placeholder.Key + "}", placeholder.Value);
            }
            return template;
        }

        private static string RenderViewToString(string viewPath, Dictionary<string, string> model)
        {
            using (var sw = new StringWriter())
            {
                var httpContext = HttpContext.Current;
                if (httpContext == null)
                {
                    throw new InvalidOperationException("HttpContext.Current is null. This method can only be called in the context of an HTTP request.");
                }

                var httpContextBase = new HttpContextWrapper(httpContext);
                var routeData = new System.Web.Routing.RouteData();
                routeData.Values.Add("controller", "EmailTemplates"); // Dummy controller name

                var controllerContext = new ControllerContext(httpContextBase, routeData, new EmptyController());

                var razorViewEngine = new RazorViewEngine();
                var viewResult = razorViewEngine.FindView(controllerContext, viewPath, null, false);

                if (viewResult.View == null)
                {
                    throw new InvalidOperationException($"Could not find view: {viewPath}");
                }

                viewResult.View.Render(new ViewContext(controllerContext, viewResult.View, new ViewDataDictionary(model), new TempDataDictionary(), sw), sw);
                return sw.ToString();
            }
        }

        public static string GenerateWelcomeEmail(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/WelcomeEmail.cshtml", placeholders);
        }

        public static string GenerateAdminCreatedAccountWelcome(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/AdminCreatedAccountWelcome.cshtml", placeholders);
        }

        public static string GenerateOrderConfirmation(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/OrderConfirmation.cshtml", placeholders);
        }

        public static string GenerateOrderStatusUpdate(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/OrderStatusUpdate.cshtml", placeholders);
        }

        public static string GeneratePaymentReset(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/PaymentReset.cshtml", placeholders);
        }

        public static string GeneratePaymentResetConfirmation(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/PaymentResetConfirmation.cshtml", placeholders);
        }

        public static string GeneratePasswordResetEmail(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/PasswordResetEmail.cshtml", placeholders);
        }

        public static string GenerateEmailConfirmation(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/EmailConfirmation.cshtml", placeholders);
        }

        public static string GenerateLowStockAlert(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/LowStockAlert.cshtml", placeholders);
        }

        public static string GeneratePaymentReceipt(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/PaymentReceipt.cshtml", placeholders);
        }

        public static string GeneratePaymentConfirmation(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/PaymentConfirmation.cshtml", placeholders);
        }

        public static string GeneratePrintOrderConfirmation(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/PrintOrderConfirmation.cshtml", placeholders);
        }

        public static string GeneratePrintingStatusUpdate(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/PrintingStatusUpdate.cshtml", placeholders);
        }

        public static string GeneratePaymentStatusUpdate(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/PaymentStatusUpdate.cshtml", placeholders);
        }

        public static string GetAdminAccountDetailsBody(Dictionary<string, string> placeholders)
        {
            return RenderViewToString("~/Views/EmailTemplates/AdminAccountDetails.cshtml", placeholders);
        }
    }
}