namespace InfrastructureRequestApp.Data.Services.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email. When SMTP is not configured the message is written to
        /// the application log instead so the flow remains testable in development.
        /// </summary>
        Task SendAsync(string toAddress, string subject, string body, bool isHtml = false);
    }
}
