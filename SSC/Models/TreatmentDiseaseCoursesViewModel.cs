﻿using System.ComponentModel.DataAnnotations;

namespace SSC.Models
{
    public class TreatmentDiseaseCoursesViewModel
    {
        [Required]
        public DateTime? Date { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        public Guid? PatientId { get; set; }

        [Required]
        public string? DiseaseCourseName { get; set; }
    }
}