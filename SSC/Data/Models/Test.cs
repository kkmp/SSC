using Microsoft.EntityFrameworkCore;
using SSC.Tools;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSC.Data.Models
{
    [Index(propertyNames: nameof(OrderNumber), IsUnique = true)]
    public class Test : ICSV
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime? TestDate { get; set; }

        [Required]
        [MinLength(12)]
        [MaxLength(12)]
        public string? OrderNumber { get; set; }

        public DateTime? ResultDate { get; set; }

        public char? Result { get; set; }

        [Required]
        public TestType? TestType { get; set; }

        [ForeignKey("TestType")]
        public Guid? TestTypeId { get; set; }

        [Required]
        public Treatment? Treatment { get; set; }

        [ForeignKey("Treatment")]
        public Guid? TreatmentId { get; set; }

        [Required]
        public User? User { get; set; }

        [ForeignKey("User")]
        public Guid? UserId { get; set; }

        [Required]
        public Place? Place { get; set; }

        [ForeignKey("Place")]
        public Guid? PlaceId { get; set; }

        public string Header()
        {
            return "OrderNumber;TestDate;TestType;Result;Place";
        }

        public string ToCSV()
        {
            return $"{OrderNumber};{TestDate};{TestType?.Name};{Result};{Place?.Name}";
        }
    }
}
