using System.ComponentModel.DataAnnotations;

namespace SSC.Models
{
    public class TreatmentCreateDTO
    {
        [Required]
        public DateTime? StartDate { get; set; }

        public bool? IsCovid { get; set; }

        [Required]
        public Guid? PatientId { get; set; }

        [Required]
        public string? TreatmentStatusName { get; set; }
    }
}
