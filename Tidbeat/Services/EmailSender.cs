﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using SpotifyAPI.Web.Http;
using System.Security.Policy;
using System.Text.Encodings.Web;
using System.Text;
using Tidbeat.Models;

namespace Tidbeat.Services {
    public class EmailSender : IEmailSender {
        private readonly ILogger _logger;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSender> logger) {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public AuthMessageSenderOptions Options { get; }

        public async Task SendEmailAsync(string toEmail, string subject, string message) {
            if (string.IsNullOrEmpty(Options.SendGridKey)) {
                throw new Exception("Null SendGridKey");
            }
            await Execute(Options.SendGridKey, subject, message, toEmail);
        }

        public async Task Execute(string apikey, string subject, string message, string toEmail) {
            var client = new SendGridClient(apikey);
            var msg = new SendGridMessage() {
                From = new EmailAddress("tidbeatnoreply@gmail.com", "Tidbeat NO-REPLY"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);
            _logger.LogInformation(response.IsSuccessStatusCode ? $"Email to {toEmail} queued sucessfully!" : $"Failure email to {toEmail}");
        }
    }
}
