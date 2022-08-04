using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSC.Data.Models
{
    [Index(propertyNames:nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
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
        public byte[]? PasswordHash { get; set; }

        [Required]
        public byte[]? PasswordSalt { get; set; }

        [Required]
        public DateTime? Date { get; set; } = DateTime.Now;

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public Role? Role { get; set; }

        [ForeignKey("Role")]
        public Guid? RoleId { get; set; }
    }
}
