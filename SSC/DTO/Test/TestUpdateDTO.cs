﻿using System.ComponentModel.DataAnnotations;

namespace SSC.DTO.Test
{
    public class TestUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime? TestDate { get; set; }

        public DateTime? ResultDate { get; set; }

        [RegularExpression(@"^(P|N|I)$", ErrorMessage = $"Test result does not exist")]
        public char? Result { get; set; }

        [Required]
        public Guid? TestTypeId { get; set; }

        [Required]
        public Guid? PlaceId { get; set; }
    }
}
