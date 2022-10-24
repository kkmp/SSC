using AutoMapper;
using SSC.Data.Models;
using SSC.DTO.Citizenship;
using SSC.DTO.City;
using SSC.DTO.DiseaseCourse;
using SSC.DTO.MedicalHistory;
using SSC.DTO.Patient;
using SSC.DTO.Test;
using SSC.DTO.TestType;
using SSC.DTO.Treatment;
using SSC.DTO.TreatmentDiseaseCourse;
using SSC.DTO.TreatmentStatus;
using SSC.DTO.User;

namespace SSC.Services
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Patient, PatientOverallGetDTO>();
            CreateMap<MedicalHistory, MedicalHistoryOverallGetDTO>()
                .ForMember(x => x.UserRole, x => x.MapFrom(y => y.User.Role.Name));
            CreateMap<MedicalHistory, MedicalHistoryGetDTO>()
                .ForMember(x => x.UserRole, x => x.MapFrom(y => y.User.Role.Name));
            CreateMap<Patient, PatientOverallGetDTO>();
            CreateMap<Patient, PatientGetDTO>()
                .ForMember(x => x.City, x => x.MapFrom(y => y.City.Name))
                .ForMember(x => x.Province, x => x.MapFrom(y => y.City.Province.Name))
                .ForMember(x => x.Citizenship, x => x.MapFrom(y => y.Citizenship.Name));
            CreateMap<Test, TestOverallGetDTO>()
                .ForMember(x => x.TestType, x => x.MapFrom(y => y.TestType.Name))
                .ForMember(x => x.Place, x => x.MapFrom(y => y.Place.Name));
            CreateMap<Test, TestGetDTO>()
                .ForMember(x => x.TestType, x => x.MapFrom(y => y.TestType.Name))
                .ForMember(x => x.UserRole, x => x.MapFrom(y => y.User.Role.Name))
                .ForMember(x => x.Place, x => x.MapFrom(y => y.Place.Name));
            CreateMap<Treatment, TreatmentOverallGetDTO>()
                .ForMember(x => x.TreatmentStatus, x => x.MapFrom(y => y.TreatmentStatus.Name));
            CreateMap<Treatment, TreatmentGetDTO>()
               .ForMember(x => x.UserRole, x => x.MapFrom(y => y.User.Role.Name))
               .ForMember(x => x.TreatmentStatus, x => x.MapFrom(y => y.TreatmentStatus.Name));
            CreateMap<TreatmentDiseaseCourse, TreatmentDiseaseCourseOverallGetDTO>()
                .ForMember(x => x.DiseaseCourse, x => x.MapFrom(y => y.DiseaseCourse.Name));
            CreateMap<User, UserDTO>()
                .ForMember(x => x.Role, x => x.MapFrom(y => y.Role.Name));
            CreateMap<User, UserOverallDTO>()
                .ForMember(x => x.Role, x => x.MapFrom(y => y.Role.Name));
            CreateMap<City, CityGetDTO>();
            CreateMap<Citizenship, CitizenshipGetDTO>();
            CreateMap<TestType, TestTypeGetDTO>();
            CreateMap<TreatmentDiseaseCourse, TreatmentDiseaseCourseGetDTO>()
                .ForMember(x => x.DiseaseCourse, x => x.MapFrom(y => y.DiseaseCourse.Name))
                .ForMember(x => x.DiseaseCourseDescription, x => x.MapFrom(y => y.DiseaseCourse.Description))
                .ForMember(x => x.UserRole, x => x.MapFrom(y => y.User.Role.Name));
            CreateMap<DiseaseCourse, DiseaseCourseGetDTO>();
            CreateMap<TreatmentStatus, TreatmentStatusGetDTO>();

            CreateMap<TreatmentCreateDTO, Treatment>();
            CreateMap<TestUpdateDTO, Test>();
            CreateMap<MedicalHistoryCreateDTO, MedicalHistory>();
            CreateMap<TreatmentDiseaseCourseCreateDTO, TreatmentDiseaseCourse>();
            CreateMap<TreatmentDiseaseCourseUpdateDTO, TreatmentDiseaseCourse>();
            CreateMap<TreatmentUpdateDTO, Treatment>();
            CreateMap<TestCreateDTO, Test>();
            CreateMap<PatientCreateDTO, Patient>();
            CreateMap<PatientUpdateDTO, Patient>();
            CreateMap<UserCreateDTO, User>();
            CreateMap<UserUpdateDTO, User>();
        }
    }
}
