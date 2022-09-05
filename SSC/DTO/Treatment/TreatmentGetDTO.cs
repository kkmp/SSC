namespace SSC.DTO.Treatment
{
    public class TreatmentGetDTO
    {
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public bool? IsCovid { get; set; }
        public string? UserName { get; set; }
        public string? UserSurname { get; set; }
        public string? UserRole { get; set; }
        public string? TreatmentStatus { get; set; }
    }
}
