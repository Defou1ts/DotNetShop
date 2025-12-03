namespace SimpleShop.Services
{
    /// <summary>
    /// Настройки SMTP для отправки почтовых уведомлений.
    /// </summary>
    public class EmailSettings
    {
        public const string SectionName = "Email";

        public string Sender { get; set; } = "noreply@simpleshop.local";

        public string SenderDisplayName { get; set; } = "SimpleShop";

        public string RecipientOverride { get; set; } = string.Empty;

        public string SmtpHost { get; set; } = string.Empty;

        public int SmtpPort { get; set; } = 587;

        public bool UseSsl { get; set; } = true;

        public string? Username { get; set; }

        public string? Password { get; set; }
    }
}


