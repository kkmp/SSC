using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSC.Data.Models
{
    [Index(propertyNames: nameof(Pesel), IsUnique = true)]
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
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
        public DateTime? AddDate { get; set; } = DateTime.Now;

        [Required]
        public City? City { get; set; }

        [ForeignKey("City")]
        public Guid? CityId { get; set; }

        [Required]
        public Citizenship? Citizenship { get; set; }

        [ForeignKey("Citizenship")]
        public Guid? CitizenshipId { get; set; }

        [Required]
        public User? User { get; set; }

        [ForeignKey("User")]
        public Guid? UserId { get; set; }
    }
}
