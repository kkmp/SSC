using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSC.Data.Models
{
    public class MedicalHistory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string? Date { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        public Patient? Patient { get; set; }

        [ForeignKey("Patient")]
        public Guid? PatientId { get; set; }

        [Required]
        public User? User { get; set; }

        [ForeignKey("User")]
        public Guid? UserId { get; set; }
    }
}
