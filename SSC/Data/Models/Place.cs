using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSC.Data.Models
{
    [Index(propertyNames: nameof(Name), IsUnique = true)]
    public class Place
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Street { get; set; }

        [Required]
        public City? City { get; set; }

        [ForeignKey("City")]
        public Guid? CityId { get; set; }
    }
}
