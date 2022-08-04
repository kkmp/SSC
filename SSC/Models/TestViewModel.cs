using System.ComponentModel.DataAnnotations;

namespace SSC.Models
{
    public class TestViewModel
    {
        [Required]
        public DateTime? TestDate { get; set; }

        [Required]
        [MinLength(12)]
        [MaxLength(12)]
        public string? OrderNumber { get; set; }

        public DateTime? ResultDate { get; set; }

        [RegularExpression(@"^(P|N|I)$", ErrorMessage = $"Test result does not exist")]
        public char? Result { get; set; }

        [Required]
        public string? TestType { get; set; }

        [Required]
        public Guid? Place { get; set; }

        [Required]
        public Guid? PatientId { get; set; }
    }
}
