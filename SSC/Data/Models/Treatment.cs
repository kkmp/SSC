using SSC.Tools;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSC.Data.Models
{
    public class Treatment : ICSV
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsCovid { get; set; }

        [Required]
        public Patient? Patient { get; set; }

        [ForeignKey("Patient")]
        public Guid? PatientId { get; set; }

        [Required]
        public User? User { get; set; }

        [ForeignKey("User")]
        public Guid? UserId { get; set; }

        [Required]
        public TreatmentStatus? TreatmentStatus { get; set; }

        [ForeignKey("TreatmentStatus")]
        public Guid? TreatmentStatusId { get; set; }

        public string Header()
        {
            return "StartDate;EndDate;IsCovid;TreatmentStatus";
        }

        public string ToCSV()
        {
            return $"{StartDate};{EndDate?.ToString()};{IsCovid};{TreatmentStatus.Name}";
        }
    }
}
