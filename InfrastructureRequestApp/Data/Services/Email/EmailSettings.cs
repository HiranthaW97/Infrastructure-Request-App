namespace InfrastructureRequestApp.Data.Services.Email
{
    /// <summary>
    /// SMTP configuration bound from the "Email" section of appsettings.json.
    /// When <see cref="Host"/> or <see cref="UserName"/> are empty the app falls
    /// back to logging the email to the console instead of sending it.
    /// </summary>
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;

        // Credentials used to authenticate with the SMTP server.
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // The address shown in the "From" field. Defaults to UserName when empty.
        public string FromAddress { get; set; } = string.Empty;
        public string FromName { get; set; } = "InfraRequest";

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(UserName);
    }
}
