using System;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using Store.Helpers.Sender.Enums;

namespace Store.Helpers.Sender
{
    public class EmailSender
    {
        /// <summary>
        /// Method generates email letter that informs that order status has changed.
        /// </summary>
        /// <param name="order">Order that was changed.</param>
        public async Task StatusChangedSend(Order order)
        {
            const string EmailTemplateSection = "EmailSendTemplates";

            ConfigurationManager configurationManger = new ConfigurationManager();
            var section = configurationManger.Configuration.GetSection($"{EmailTemplateSection}");

            var message = string.Format(section[EmailSendTemplate.StatusChangedMessage.ToString()],
                order.Customer.FirstName + " " + order.Customer.SecondName,
                order.Id, order.OrderStatus, DateTime.Now);
            var subject = section[EmailSendTemplate.StatusChangedSubject.ToString()];

            await SendEmailAsync(order.Customer.Email, subject, message, section);
        }

        /// <summary>
        /// Method generates email letter that informs that end point of order has changed.
        /// </summary>
        /// <param name="order">Order that was changed.</param>
        /// <param name="oldEndPoint">Old end point of order.</param>
        public async Task EndPointChangeSend(Order order, string oldEndPoint)
        {
            const string EmailTemplateSection = "EmailSendTemplates";

            ConfigurationManager configurationManger = new ConfigurationManager();
            var section = configurationManger.Configuration.GetSection($"{EmailTemplateSection}");

            var message = string.Format(section[EmailSendTemplate.EndPointChangedMessage.ToString()],
                order.Customer.FirstName + " " + order.Customer.SecondName,
                order.Id, oldEndPoint, order.EndPointCity + ", " + order.EndPointStreet);
            var subject = section[EmailSendTemplate.EndPointChangedSubject.ToString()];

            await SendEmailAsync(order.Customer.Email, subject, message, section);
        }

        /// <summary>
        /// Method generates email letter where is code to change password.
        /// </summary>
        /// <param name="order">Order that was changed.</param>
        /// <param name="oldEndPoint">Old end point of order.</param>
        public async Task PasswordChangeCodeSend(Customer customer, string url)
        {
            const string EmailTemplateSection = "EmailSendTemplates";

            ConfigurationManager configurationManger = new ConfigurationManager();
            var section = configurationManger.Configuration.GetSection($"{EmailTemplateSection}");

            var message = string.Format(section[EmailSendTemplate.PasswordChangeMessage.ToString()], url);
            var subject = section[EmailSendTemplate.PasswordChangeSubject.ToString()];

            await SendEmailAsync(customer.Email, subject, message, section);
        }

        /// <summary>
        /// Common method that sends letter to customer's email.
        /// </summary>
        /// <param name="email">Customer'email.</param>
        /// <param name="subject">Subject of letter.</param>
        /// <param name="message">Message.</param>
        /// <param name="section">Section in appsettings.json file.</param>
        public async Task SendEmailAsync(string email, string subject, string message,
            IConfigurationSection section)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(section[EmailSendTemplate.FromWhoName.ToString()],
                section[EmailSendTemplate.FromWhoEmail.ToString()]));
            emailMessage.To.Add(new MailboxAddress(email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(section[EmailSendTemplate.GmailHost.ToString()], 587, false);
                await client.AuthenticateAsync(section[EmailSendTemplate.FromWhoEmail.ToString()],
                    section[EmailSendTemplate.Password.ToString()]);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
