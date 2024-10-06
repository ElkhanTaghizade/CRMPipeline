using System.ComponentModel.DataAnnotations;

namespace CRM_Pipeline.Models
{
    public class Products
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
    }
}
