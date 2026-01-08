using System.Net;
using System.Net.Mail;
using API.Core.Dtos;
using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Helpers;
using KashmiriZamindar.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using KashmiriZamindar.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KashmiriZamindar.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly bool _enableSsl;

        public EmailService(
            IConfiguration configuration,
            IAdminRepository adminRepository,
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _adminRepository = adminRepository;
            _logger = logger;

            // Load SMTP settings from appsettings.json
            _smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "";
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@kashmirizamindar.com";
            _fromName = _configuration["EmailSettings:FromName"] ?? "Kashmiri Zamindar";
            _enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(_smtpHost, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = _enableSsl
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {toEmail}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendTemplateEmailAsync(
            string toEmail, 
            string templateName, 
            Dictionary<string, string> templateData)
        {
            try
            {
                var template = await _adminRepository.GetEmailTemplateByNameAsync(templateName);
                if (template == null)
                {
                    _logger.LogError($"Email template '{templateName}' not found");
                    return false;
                }

                var subject = RenderTemplate(template.Subject, templateData);
                var body = RenderTemplate(template.BodyTemplate, templateData);

                return await SendEmailAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send template email: {ex.Message}");
                return false;
            }
        }

        public async Task ProcessPendingEmailsAsync()
        {
            try
            {
                var pendingEmails = await _adminRepository.GetPendingEmailNotificationsAsync(50);

                foreach (var email in pendingEmails)
                {
                    var success = await SendEmailAsync(
                        email.RecipientEmail,
                        email.Subject,
                        email.Body
                    );

                    var status = success ? "Sent" : "Failed";
                    var failureReason = success ? null : "SMTP send failed";

                    await _adminRepository.UpdateEmailNotificationStatusAsync(
                        email.NotificationGuid,
                        status,
                        failureReason
                    );

                    // Add delay to avoid rate limiting
                    await Task.Delay(1000);
                }

                _logger.LogInformation($"Processed {pendingEmails.Count} pending emails");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing pending emails: {ex.Message}");
            }
        }

        public Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> data)
        {
            // This is a simple template rendering
            // For production, consider using a proper templating engine like RazorLight or Scriban
            var result = templateName;
            foreach (var kvp in data)
            {
                result = result.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
            }
            return Task.FromResult(result);
        }

        private string RenderTemplate(string template, Dictionary<string, string> data)
        {
            var result = template;
            foreach (var kvp in data)
            {
                result = result.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
            }
            return result;
        }
    }
}


