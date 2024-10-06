using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_Pipeline.Models
{
    public class Customers
    {
        //[Key] public int Id { get; set; }
        //[Required] public string Name { get; set; }
        //[Required] public string Surname { get; set; }
        //public string? Email { get; set; }
        //public string? PhoneNumber { get; set; }
        //[ValidateNever]
        ////public string? Company { get; set; }
        //[ValidateNever]
        ////public string? Department { get; set; }
        //[ValidateNever]
        ////public string? Position { get; set; }
        //[ValidateNever] 

        //public int User_Id { get; set; }
        //public DateTime Created_At { get; set; } = DateTime.Now;



        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        [Required]
        [StringLength(30, ErrorMessage = "Add the name please")]
        public string Name { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "Add the surname please")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Please enter a phone number")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Phone number must be exactly 9 digits.")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter Email ID")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter company name")]
        public string? Company { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign Key
        public int? CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        public ICollection<Lead>? Leads { get; set; } = new List<Lead>();
        public ICollection<Email>? ReceivedEmails { get; set; } = new List<Email>();
    }
}
