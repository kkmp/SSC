using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSC.Data.Models
{
    public class ChangePasswordCode
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public User? User { get; set; }

        [ForeignKey("User")]
        public Guid? UserId { get; set; }

        [Required]
        [MaxLength(100)]
        [MinLength(100)]
        public string? Code { get; set; }
    }
}
