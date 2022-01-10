using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using SwishIdentity.Tools.DependencyService;

namespace SwishIdentity.Data.EmailService
{
    public interface ISwishEmailService
    {
        Task<Response> VerifySwishEmail(string callbackUrl, string email);
        Task<Response> ResetSwishPassword(string callbackUrl, string email);
    }
    [Service]
    public class SwishEmailService : ISwishEmailService
    {
        public async Task<Response> VerifySwishEmail(string callbackUrl, string email)
        {
            //var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var from = new EmailAddress("alex@swishid.com", "Swish Team");   
            var to = new EmailAddress(email, "Swish User");
            var client = new SendGridClient("sendgrid_secret_here");
            string templateId = "sengrid-template-id";
            object templateDynamicData = new
            {
                callbackUrl=callbackUrl
            };
            
            
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, templateDynamicData);
                //.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return response;
        }
        
        public async Task<Response> ResetSwishPassword(string callbackUrl, string email)
        {
            //var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var from = new EmailAddress("alex@swishid.com", "Swish Team");   
            var to = new EmailAddress(email, "Swish User");
            var client = new SendGridClient("sendgrid_client");
            string templateId = "sendgrid-template-id";
            object templateDynamicData = new
            {
                callbackUrl=callbackUrl
            };
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, templateDynamicData);
            var response = await client.SendEmailAsync(msg);
            return response;
        }
    }
}