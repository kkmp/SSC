﻿using System.ComponentModel.DataAnnotations;

namespace SSC.DTO.MedicalHistory
{
    public class MedicalHistoryUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime? Date { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Długość opisu musi być dłuższa niż 3 znaki")]
        [MaxLength(255, ErrorMessage = "Długość opisu nie może przekraczać 255 znaków")]
        public string? Description { get; set; }
    }
}
