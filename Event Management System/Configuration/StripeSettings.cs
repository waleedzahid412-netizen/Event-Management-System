namespace Event_Management_System.Configuration
{
    public class StripeSettings
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
        public string Currency { get; set; } = "usd";
    }
}
