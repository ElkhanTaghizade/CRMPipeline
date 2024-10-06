using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_Pipeline.Models
{
    public class Stages
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "Please enter stage name")] public string Name { get; set; }
        public double Total_Revenue { get; set; }
        public Boolean Is_Active { get; set; } = true;
        public int UserId { get; set; }
        public string User { get; set; } 
    }
}
