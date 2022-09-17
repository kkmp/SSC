namespace SSC.DTO.Patient
{
    public class PatientGetDTO
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Pesel { get; set; }
        public string? BirthDate { get; set; }
        public char? Sex { get; set; }
        public string? Street { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? CityId { get; set; }
        public string? Province { get; set; }
        public string? Citizenship { get; set; }
        public string? CitizenshipId { get; set; }
    }
}
