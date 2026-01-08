using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KashmiriZamindar.Core.Interfaces;

namespace KashmiriZamindar.Core.Services
{
    public class EmailProcessingBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailProcessingBackgroundService> _logger;

        public EmailProcessingBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<EmailProcessingBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("✅ Email Processing Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    await emailService.ProcessPendingEmailsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error processing emails: {ex.Message}");
                }

                // Check every 30 seconds (emails sent within 30 seconds max)
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }

            _logger.LogInformation("Email Processing Background Service stopped");
        }
    }
}