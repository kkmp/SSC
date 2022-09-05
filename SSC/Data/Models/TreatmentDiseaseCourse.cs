using SSC.Tools;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSC.Data.Models
{
    public class TreatmentDiseaseCourse : ICSV
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime? Date { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        public Treatment? Treatment { get; set; }

        [ForeignKey("Treatment")]
        public Guid? TreatmentId { get; set; }

        [Required]
        public DiseaseCourse? DiseaseCourse { get; set; }

        [ForeignKey("DiseaseCourse")]
        public Guid? DiseaseCourseId { get; set; }

        [Required]
        public User? User { get; set; }

        [ForeignKey("User")]
        public Guid? UserId { get; set; }

        public string Header()
        {
            return "Date;Description;DiseaseCourse;DiseaseCourseDescription";
        }

        public string ToCSV()
        {
            return $"{Date};{Description};{DiseaseCourse?.Name};{DiseaseCourse?.Description}";
        }
    }
}
