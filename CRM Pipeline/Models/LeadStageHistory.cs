using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_Pipeline.Models
{
    public class LeadStageHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid Lead_Id { get; set; }

        [Required(ErrorMessage = "Please enter a probablity")]
        public double Probability { get; set; }
        public DateTime? ExpectedClosingDate { get; set; }
        [ValidateNever]
        public int User_Id { get; set; }
        [ValidateNever]
        public string? Extra_Information { get; set; }
        [ValidateNever]
        public string? Internal_Notes { get; set; }
        public DateTime Changed_At { get; set; } = DateTime.Now;

    }
}
