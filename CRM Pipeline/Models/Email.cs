namespace CRM_Pipeline.Models
{
    public class Email
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsResponse { get; set; }

        // Foreign Keys
        public int SentByUserId { get; set; }
        public User SentByUser { get; set; }

        public int SentToCustomerId { get; set; }
        public Customers SentToCustomer { get; set; }
    }
}
