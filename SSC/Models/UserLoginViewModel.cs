using System.ComponentModel.DataAnnotations;

namespace SSC.Models
{
    public class UserLoginViewModel
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
