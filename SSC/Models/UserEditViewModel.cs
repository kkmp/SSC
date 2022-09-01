using SSC.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace SSC.Models
{
    public class UserEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Surname { get; set; }

        [Required]
        [MaxLength(254)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(9)]
        public string? PhoneNumber { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [RegularExpression(@"^(Lekarz|Laborant|Administrator)$", ErrorMessage = $"Role does not exist")]
        public string? RoleName { get; set; }
    }
}
