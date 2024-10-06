using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_Pipeline.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Surname { get; set; }
        [Required] public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Customers> Customers { get; set; } 
		public ICollection<Email> SentEmails { get; set; }   
    }
}
