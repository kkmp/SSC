using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SSC.Data.Models
{
    [Index(propertyNames: nameof(Name), IsUnique = true)]
    public class Province
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string? Name { get; set; }
    }
}
