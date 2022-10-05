using System.ComponentModel.DataAnnotations;

namespace SSC.DTO.Treatment
{
    public class TreatmentUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsCovid { get; set; }

        [Required]
        public Guid? TreatmentStatusId { get; set; }
    }
}
