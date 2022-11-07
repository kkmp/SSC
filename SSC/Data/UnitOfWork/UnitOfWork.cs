using AutoMapper;
using SSC.Services;
using SSC.Data.Repositories;

namespace SSC.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public IUserRepository UserRepository  { get; set; }
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

        public UnitOfWork(DataContext context, IMailService mailService, IMapper mapper)
        {
            Mapper = mapper;
            MailService = mailService;
            UserRepository = new UserRepository(context, this);
            PatientRepository = new PatientRepository(context, this);
            ChangePasswordRepository = new ChangePasswordRepository(context, this);
            CitizenshipRepository = new CitizenshipRepository(context);
            CityRepository = new CityRepository(context);
            DiseaseCourseRepository = new DiseaseCourseRepository(context);
            MedicalHistoryRepository = new MedicalHistoryRepository(context, this);
            PlaceRepository = new PlaceRepository(context, this);
            RoleRepository = new RoleRepository(context);
            TestRepository = new TestRepository(context, this);
            TestTypeRepository = new TestTypeRepository(context);
            TreatmentDiseaseCourseRepository = new TreatmentDiseaseCourseRepository(context, this);
            TreatmentRepository = new TreatmentRepository(context, this);
            TreatmentStatusRepository = new TreatmentStatusRepository(context);
            ProvinceRepository = new ProvinceRepository(context);
        }
    }
}
