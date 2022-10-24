using System.ComponentModel.DataAnnotations;

namespace SSC.DTO.Password
{
    public class PasswordUpdateDTO
    {
        [Required]
        public string? OldPassword { get; set; }

        [Required]
        public string? NewPassword { get; set; }
    }
}
