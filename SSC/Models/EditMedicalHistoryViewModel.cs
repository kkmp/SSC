using System.ComponentModel.DataAnnotations;

namespace SSC.Models
{
    public class EditMedicalHistoryViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime? Date { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
