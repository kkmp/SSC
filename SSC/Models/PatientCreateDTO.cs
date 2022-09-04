using SSC.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace SSC.Models
{
    public class PatientCreateDTO
    {
        [Required]
        [MinLength(11)]
        [MaxLength(11)]
        public string? Pesel { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Surname { get; set; }

        [Required]
        public char? Sex { get; set; }

        [Required]
        public DateTime? BirthDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Street { get; set; }

        [Required]
        [MaxLength(10)]
        public string? Address { get; set; }

        [MaxLength(9)]
        public string? PhoneNumber { get; set; }

        [Required]
        public string? CityName { get; set; }

        [Required]
        public string? CitizenshipName { get; set; }
    }
}
