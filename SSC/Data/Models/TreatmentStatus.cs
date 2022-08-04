using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SSC.Data.Models
{
    [Index(propertyNames: nameof(Name), IsUnique = true)]
    public class TreatmentStatus
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string? Name { get; set; }
    }
}
