using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSC.Data.Models
{
    public class City
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string? Name { get; set; }

        [Required]
        public Province? Province { get; set; }

        [ForeignKey("Province")]
        public Guid? ProvinceId { get; set; }
    }
}
