using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Wedding.Services
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await configSendGridasync(message);
        }

        // Use NuGet to install SendGrid (Basic C# client lib) 
        private async Task configSendGridasync(IdentityMessage message)
        {
            var apiKey = ConfigurationManager.AppSettings["sendGridApiKey"];
            dynamic sg = new SendGridAPIClient(apiKey);

            Mail mail = new Mail();

            Content content = new Content();
            content.Type = "text/plain";
            content.Value = message.Body;
            mail.AddContent(content);
            content = new Content();
            content.Type = "text/html";
            content.Value = message.Body;
            mail.AddContent(content);

            mail.From = new Email("zenzoy@zenzoy.com", "ZenZoy");
            mail.Subject = message.Subject;

            Personalization personalization = new Personalization();
            var emailTo = new Email();
            emailTo.Name = message.Destination;
            emailTo.Address = message.Destination;
            personalization.AddTo(emailTo);
            mail.AddPersonalization(personalization);

            dynamic response = sg.client.mail.send.post(requestBody: mail.Get());

            var status = response.StatusCode;
            var responseBody = await response.Body.ReadAsStringAsync();
        }

        //// Use NuGet to install SendGrid (Basic C# client lib) 
        //private async Task configSendGridasync(IdentityMessage message)
        //{
        //    var myMessage = new SendGridMessage();
        //    myMessage.AddTo(message.Destination);
        //    myMessage.From = new System.Net.Mail.MailAddress("zenzoy@gmail.com", "ZenZoy");
        //    myMessage.Subject = message.Subject;
        //    myMessage.Text = message.Body;
        //    myMessage.Html = message.Body;

        //    var credentials = new NetworkCredential(
        //               ConfigurationManager.AppSettings["mailAccount"],
        //               ConfigurationManager.AppSettings["mailPassword"]
        //               );

        //    var apiKey = ConfigurationManager.AppSettings["sendGridApiKey"];

        //    // Create a Web transport for sending email.
        //    var transportWeb = new Web(apiKey);

        //    // Send the email.
        //    if (transportWeb != null)
        //    {
        //        await transportWeb.DeliverAsync(myMessage);
        //    }
        //    else
        //    {
        //        Trace.TraceError("Failed to create Web transport.");
        //        await Task.FromResult(0);
        //    }
        //}
    }
}