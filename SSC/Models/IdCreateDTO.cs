using System.ComponentModel.DataAnnotations;

namespace SSC.Models
{
    public class IdCreateDTO
    {
        [Required]
        public Guid Id { get; set; }
    }
}
