using System.ComponentModel.DataAnnotations;

namespace SSC.DTO.TreatmentDiseaseCourse
{
    public class TreatmentDiseaseCourseUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }

        public DateTime? Date { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        public Guid DiseaseCourseId { get; set; }
    }
}
