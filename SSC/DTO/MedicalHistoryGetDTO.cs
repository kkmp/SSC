namespace SSC.DTO
{
    public class MedicalHistoryGetDTO
    {
        public Guid Id { get; set; }
        public string? Date { get; set; }
        public string? Description { get; set; }
        public string? UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserRole { get; set; }
    }
}
