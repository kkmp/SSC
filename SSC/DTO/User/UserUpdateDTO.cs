using SSC.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace SSC.DTO.User
{
    public class UserUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Imię użytkownika nie powinno przekraczać 50 znaków")]
        public string? Name { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Nazwisko użytkownika nie powinno przekraczać 50 znaków")]
        public string? Surname { get; set; }

        [Required]
        [MaxLength(254, ErrorMessage = "Adres email nie powinnien przekraczać 254 znaków")]
        public string? Email { get; set; }

        [Required]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "Numer telefonu powinien zawierać cyfry")]
        [MinLength(9, ErrorMessage = "Numer telefonu powinien mieć 9 cyfr")]
        [MaxLength(9, ErrorMessage = "Numer telefonu powinien mieć 9 cyfr")]
        public string? PhoneNumber { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [RegularExpression(@"^(Lekarz|Laborant|Administrator)$", ErrorMessage = $"Podana rola użytkownika nie istnieje")]
        public string? RoleName { get; set; }
    }
}
