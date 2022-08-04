using System.ComponentModel.DataAnnotations;

namespace SSC.Models
{
    public class IdViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}
