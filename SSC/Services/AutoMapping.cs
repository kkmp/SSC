using AutoMapper;
using SSC.Data.Models;
using SSC.DTO;

namespace SSC.Services
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Patient, PatientDTO>();
                //.ForMember(x => x.BirthDate, x => x.MapFrom(y => y.City.Name));
        }
    }
}
