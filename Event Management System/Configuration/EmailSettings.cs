namespace Event_Management_System.Configuration
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = null!;
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; } = null!;
        public string SmtpPass { get; set; } = null!;
        public string FromName { get; set; } = null!;
    }
}
