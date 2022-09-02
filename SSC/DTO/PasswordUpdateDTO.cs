using System.ComponentModel.DataAnnotations;

namespace SSC.DTO
{
    public class PasswordUpdateDTO
    {
        [Required]
        public string? Password { get; set; }
    }
}
