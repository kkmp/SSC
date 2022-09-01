using AutoMapper;
using SSC.Data.Models;
using SSC.DTO;
using SSC.DTO.Patient;
using SSC.Models;

namespace SSC.Services
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Patient, PatientOverallDTO>();
            CreateMap<MedicalHistory, MedicalHistoryDTO>()
                .ForMember(x => x.UserName, x => x.MapFrom(y => y.User.Name))
                .ForMember(x => x.UserSurname, x => x.MapFrom(y => y.User.Surname))
                .ForMember(x => x.UserRole, x => x.MapFrom(y => y.User.Role.Name));
            CreateMap<Patient, PatientOverallDTO>();
            CreateMap<Patient, PatientDTO>()
                .ForMember(x => x.City, x => x.MapFrom(y => y.City.Name))
                .ForMember(x => x.Province, x => x.MapFrom(y => y.City.Province.Name))
                .ForMember(x => x.Citizenship, x => x.MapFrom(y => y.Citizenship.Name));
            CreateMap<Test, TestOverallDTO>()
                .ForMember(x => x.TestType, x => x.MapFrom(y => y.TestType.Name))
                .ForMember(x => x.Place, x => x.MapFrom(y => y.Place.Name));
            CreateMap<Test, TestDTO>()
                .ForMember(x => x.TestType, x => x.MapFrom(y => y.TestType.Name))
                .ForMember(x => x.UserName, x => x.MapFrom(y => y.User.Name))
                .ForMember(x => x.UserSurname, x => x.MapFrom(y => y.User.Surname))
                .ForMember(x => x.UserRole, x => x.MapFrom(y => y.User.Role.Name))
                .ForMember(x => x.Place, x => x.MapFrom(y => y.Place.Name));
            CreateMap<Treatment, TreatmentDTO>()
                .ForMember(x => x.UserName, x => x.MapFrom(y => y.User.Name))
                .ForMember(x => x.UserSurname, x => x.MapFrom(y => y.User.Surname))
                .ForMember(x => x.UserRole, x => x.MapFrom(y => y.User.Role.Name))
                .ForMember(x => x.TreatmentStatus, x => x.MapFrom(y => y.TreatmentStatus.Name));
            CreateMap<TreatmentDiseaseCourse, TreatmentDiseaseCourseDTO>()
                .ForMember(x => x.DiseaseCourse, x => x.MapFrom(y => y.DiseaseCourse.Name));
            CreateMap<User, UserDTO>()
                .ForMember(x => x.Role, x => x.MapFrom(y => y.Role.Name));
            CreateMap<User, UserOverallDTO>()
                .ForMember(x => x.Role, x => x.MapFrom(y => y.Role.Name));

            CreateMap<TreatmentViewModel, Treatment>();
            CreateMap<TestEditViewModel, Test>();
            CreateMap<MedicalHistoryViewModel, MedicalHistory>();
            CreateMap<TreatmentDiseaseCourseViewModel, TreatmentDiseaseCourse>();
            CreateMap<TreatmentDiseaseCourseEditViewModel, TreatmentDiseaseCourse>();
            CreateMap<TreatmentEditViewModel, Treatment>();
            CreateMap<TestViewModel, Test>();
            CreateMap<PatientViewModel, Patient>();
            CreateMap<PatientUpdateDTO, Patient>();
            CreateMap<UserViewModel, User>();
            CreateMap<UserEditViewModel, User>();
        }
    }
}
