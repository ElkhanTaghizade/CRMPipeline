namespace CRM_Pipeline.Models
{
    public class LeadCreateDto
    {
        //public string FullName { get; set; }
        //public string Product { get; set; } 
        public int CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public double ExpectedRevenue { get; set; } 
        public double Probability { get; set; } 
        public DateTime ExpectedClosingDate { get; set; } 
        public Guid StageId { get; set; } 
        public int UserId { get; set; } 
    }
}
