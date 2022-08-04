using System.ComponentModel.DataAnnotations;

namespace SSC.Models
{
    public class UserEmailViewModel
    {
        [Required]
        public string? Email { get; set; }
    }
}
