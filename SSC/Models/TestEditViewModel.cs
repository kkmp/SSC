using System.ComponentModel.DataAnnotations;

namespace SSC.Models
{
    public class TestEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

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
        public string? TestTypeName { get; set; }

        [Required]
        public Guid? PlaceId { get; set; }
    }
}
