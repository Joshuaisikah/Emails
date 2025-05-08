namespace EmailService.Frontend.Models
{
    public class EmailRecord
    {
        public Guid EmailId { get; set; }
        public string? Sender { get; set; }
        public string? Recipient { get; set; }
        public string? Subject { get; set; }
        public string? MessageBody { get; set; }
        public string? Status { get; set; }
        public DateTime DateSent { get; set; }
        public int NumberOfAttempts { get; set; }
    }
}