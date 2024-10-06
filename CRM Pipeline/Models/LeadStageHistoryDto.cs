using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CRM_Pipeline.Models
{
    public class LeadStageHistoryDto
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "Add the name please")]
        public string Name { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "Add the surname please")]
        public string Surname { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter Email ID")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }
        [Required]
        public string? Phone_Number { get; set; }
        [Required(ErrorMessage = "Please enter company name")]
        public string? Company { get; set; }
        [ValidateNever]
        public string? Department { get; set; }
        [ValidateNever]
        public string? Position { get; set; }
        [ValidateNever]
        public double Probability { get; set; }
        public DateTime ExpectedClosingDate { get; set; }
        public Guid ProductId { get; set; }
        public double? ExpectedRevenue { get; set; }
        public int User_Id { get; set; }
        public string Extra_Information { get; set; }
        public string Internal_Notes { get; set; }
        public DateTime Changed_At { get; set; }
    }
}
 