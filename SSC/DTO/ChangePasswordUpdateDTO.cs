using System.ComponentModel.DataAnnotations;

namespace SSC.DTO
{
    public class ChangePasswordUpdateDTO
    {
        [Required]
        public string? Password { get; set; }

        [Required]
        public string? Code { get; set; }
    }
}
