namespace SSC.DTO.Treatment
{
    public class TreatmentOverallGetDTO
    {
        public Guid Id { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public bool? IsCovid { get; set; }
        public string? TreatmentStatus { get; set; }
    }
}
