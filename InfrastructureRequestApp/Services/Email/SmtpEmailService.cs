using System.Net;
using System.Net.Mail;
using InfrastructureRequestApp.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace InfrastructureRequestApp.Services.Email
{
    /// <summary>
    /// Sends email over SMTP (e.g. Gmail). If no SMTP host/credentials are
    /// configured it logs the message instead, so the password-recovery flow
    /// works end-to-end in development without real credentials.
    /// </summary>
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IOptions<EmailSettings> settings, ILogger<SmtpEmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendAsync(string toAddress, string subject, string body, bool isHtml = false)
        {
            if (string.IsNullOrWhiteSpace(toAddress))
            {
                _logger.LogWarning("Email not sent: no recipient address. Subject was '{Subject}'.", subject);
                return;
            }

            if (!_settings.IsConfigured)
            {
                // Dev fallback: no SMTP configured, so log the email instead of sending.
                _logger.LogWarning(
                    "SMTP not configured. Email would have been sent to {To}.\nSubject: {Subject}\nBody:\n{Body}",
                    toAddress, subject, body);
                return;
            }

            var fromAddress = string.IsNullOrWhiteSpace(_settings.FromAddress)
                ? _settings.UserName
                : _settings.FromAddress;

            using var message = new MailMessage
            {
                From = new MailAddress(fromAddress, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            message.To.Add(toAddress);

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.UserName, _settings.Password)
            };

            await client.SendMailAsync(message);
            _logger.LogInformation("Recovery email sent to {To}.", toAddress);
        }
    }
}
