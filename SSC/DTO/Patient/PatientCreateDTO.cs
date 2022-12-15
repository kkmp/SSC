using System.ComponentModel.DataAnnotations;

namespace SSC.DTO.Patient
{
    public class PatientCreateDTO
    {
        [Required]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "Numer PESEL powinien zawierać cyfry")]
        [MinLength(11, ErrorMessage = "Numer PESEL powinien mieć 11 cyfr")]
        [MaxLength(11, ErrorMessage = "Numer PESEL powinien mieć 11 cyfr")]
        public string? Pesel { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Imię pacjenta nie powinno przekraczać 50 znaków")]
        public string? Name { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Nazwisko pacjenta nie powinno przekraczać 50 znaków")]
        public string? Surname { get; set; }

        [Required]
        public char? Sex { get; set; }

        [Required]
        public DateTime? BirthDate { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Nazwa ulicy nie powinna przekraczać 50 znaków")]
        public string? Street { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Adres pacjenta nie powinien przekraczać 10 znaków")]
        public string? Address { get; set; }

        [RegularExpression("^[0-9]{9}$", ErrorMessage = "Numer telefonu powinien zawierać cyfry")]
        [MinLength(9, ErrorMessage = "Numer telefonu powinien mieć 9 cyfr")]
        [MaxLength(9, ErrorMessage = "Numer telefonu powinien mieć 9 cyfr")]
        public string? PhoneNumber { get; set; }

        [Required]
        public Guid CityId { get; set; }

        [Required]
        public Guid CitizenshipId { get; set; }
    }
}
