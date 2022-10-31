using System.ComponentModel.DataAnnotations;

namespace SSC.DTO.Test
{
    public class TestCreateDTO
    {
        [Required]
        public DateTime? TestDate { get; set; }

        [Required]
        [MinLength(12, ErrorMessage = "Numer testu składa się z 12 znaków")]
        [MaxLength(12, ErrorMessage = "Numer testu składa się z 12 znaków")]
        public string? OrderNumber { get; set; }

        public DateTime? ResultDate { get; set; }

        [RegularExpression(@"^(P|N|I)$", ErrorMessage = $"Test result does not exist")]
        public char? Result { get; set; }

        [Required]
        public Guid? TestTypeId { get; set; }

        [Required]
        public Guid? PlaceId { get; set; }

        [Required]
        public Guid? PatientId { get; set; }
    }
}
