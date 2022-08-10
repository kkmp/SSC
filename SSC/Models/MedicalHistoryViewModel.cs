using System.ComponentModel.DataAnnotations;

namespace SSC.Models
{
    public class MedicalHistoryViewModel
    {
        [Required]
        public DateTime? Date { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        public Guid? PatientId { get; set; }
    }
}
