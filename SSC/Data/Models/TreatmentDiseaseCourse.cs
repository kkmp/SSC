using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSC.Data.Models
{
    public class TreatmentDiseaseCourse
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime? Date { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        public Patient? Patient { get; set; }

        [ForeignKey("Patient")]
        public Guid? PatientId { get; set; }

        [Required]
        public DiseaseCourse? DiseaseCourse { get; set; }

        [ForeignKey("DiseaseCourse")]
        public Guid? DiseaseCourseId { get; set; }
    }
}
