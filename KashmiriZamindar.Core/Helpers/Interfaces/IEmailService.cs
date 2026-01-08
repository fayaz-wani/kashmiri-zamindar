using API.Core.Dtos;

namespace KashmiriZamindar.Core.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
        Task<bool> SendTemplateEmailAsync(string toEmail, string templateName, Dictionary<string, string> templateData);
        Task ProcessPendingEmailsAsync();
        Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> data);
    }
}