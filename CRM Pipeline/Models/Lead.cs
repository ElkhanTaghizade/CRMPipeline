using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_Pipeline.Models
{
    public class Lead
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "Please enter customer")]
        public int CustomerId { get; set; }
        public Boolean IsActive { get; set; } = true;
        //[Required(ErrorMessage = "Please enter product name")]
        //public string Product { get; set; }
        [Required(ErrorMessage = "Please enter a Expected Revenue")]
        public double ExpectedRevenue { get; set; }
        public int UserId { get; set; }
        public Guid Stage_Id { get; set; }
        [Required(ErrorMessage = "Please enter product")]
        public Guid Product_Id { get; set; }

        //public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
