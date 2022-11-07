using AutoMapper;
using SSC.Services;
using SSC.Data.Repositories;

namespace SSC.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepository { get; set; }
        public IPatientRepository PatientRepository { get; set; }
        public IRoleRepository RoleRepository { get; set; }
        public ITestRepository TestRepository { get; set; }
        public ITreatmentDiseaseCourseRepository TreatmentDiseaseCourseRepository { get; set; }
        public ITreatmentRepository TreatmentRepository { get; set; }
        public IMedicalHistoryRepository MedicalHistoryRepository { get; set; }
        public IChangePasswordRepository ChangePasswordRepository { get; set; }
        public ICitizenshipRepository CitizenshipRepository { get; set; }
        public ICityRepository CityRepository { get; set; }
        public IPlaceRepository PlaceRepository { get; set; }
        public ITestTypeRepository TestTypeRepository { get; set; }
        public IDiseaseCourseRepository DiseaseCourseRepository { get; set; }
        public ITreatmentStatusRepository TreatmentStatusRepository { get; set; }
        public IProvinceRepository ProvinceRepository { get; set; }
        public IMailService MailService { get; set; }
        public IMapper Mapper { get; set; }
    }
}
