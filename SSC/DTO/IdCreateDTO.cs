using System.ComponentModel.DataAnnotations;

namespace SSC.DTO
{
    public class IdCreateDTO
    {
        [Required]
        public Guid Id { get; set; }
    }
}
