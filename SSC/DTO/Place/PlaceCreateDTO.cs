
using System.ComponentModel.DataAnnotations;

namespace SSC.DTO.Place
{
    public class PlaceCreateDTO
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Długość nazwy placówki musi być krótsza niż 50 znaków")]
        public string? Name { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Długość nazwy ulicy musi być krótsza niż 100 znaków")]
        public string? Street { get; set; }

        [Required]
        public Guid? CityId { get; set; }
    }
}
