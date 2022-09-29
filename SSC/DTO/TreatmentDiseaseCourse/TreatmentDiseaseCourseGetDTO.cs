namespace SSC.DTO.TreatmentDiseaseCourse
{
    public class TreatmentDiseaseCourseGetDTO
    {
        public string? Date { get; set; }
        public string? Description { get; set; }
        public string? DiseaseCourse { get; set; }
        public Guid DiseaseCourseId { get; set; }
        public string? DiseaseCourseDescription { get; set; }
        public Guid TreatmentId { get; set; }
        public string? UserName { get; set; }
        public string? UserSurname { get; set; }
        public string? UserEmail { get; set; }
        public string? UserRole { get; set; }
    }
}
