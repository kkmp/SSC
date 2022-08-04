namespace SSC.DTO
{
    public class PatientDTO
    {
        public Guid Id { get; set; }
        public string? Pesel { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public char? Sex { get; set; }
        public string? BirthDate { get; set; }
    }
}
