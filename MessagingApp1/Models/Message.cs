namespace MessagingApp1.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public DateTime SentDate { get; set; }
        public string Status { get; set; } // "new" or "read"
    }
}
