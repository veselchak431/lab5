using System.Collections.Generic;

namespace MessagingApp1.Models
{
    public class UserMessagesViewModel
    {
        public User User { get; set; }
        public List<Message> Messages { get; set; }
        public string Sender { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Status { get; set; }
        public string SortOrder { get; set; }
    }
}