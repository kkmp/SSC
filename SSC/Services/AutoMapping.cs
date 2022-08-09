using AutoMapper;
using SSC.Data.Models;
using SSC.DTO;
using SSC.Models;

namespace SSC.Services
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Patient, PatientDTO>();
            CreateMap<TreatmentViewModel, Treatment>();
            //.ForMember(x => x.BirthDate, x => x.MapFrom(y => y.City.Name));
            CreateMap<TestEditViewModel, Test>();
        }
    }
}
