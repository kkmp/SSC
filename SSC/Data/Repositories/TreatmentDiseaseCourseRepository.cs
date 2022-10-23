using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.DTO.Treatment;
using SSC.DTO.TreatmentDiseaseCourse;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public class TreatmentDiseaseCourseRepository : BaseRepository<TreatmentDiseaseCourse>, ITreatmentDiseaseCourseRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;
        private readonly IPatientRepository patientRepository;
        private readonly ITreatmentRepository treatmentRepository;
        private readonly ITreatmentStatusRepository treatmentStatusRepository;
        private readonly IDiseaseCourseRepository diseaseCourseRepository;

        public TreatmentDiseaseCourseRepository(DataContext context, IMapper mapper, IPatientRepository patientRepository, ITreatmentRepository treatmentRepository, ITreatmentStatusRepository treatmentStatusRepository, IDiseaseCourseRepository diseaseCourseRepository)
        {
            this.context = context;
            this.mapper = mapper;
            this.patientRepository = patientRepository;
            this.treatmentRepository = treatmentRepository;
            this.treatmentStatusRepository = treatmentStatusRepository;
            this.diseaseCourseRepository = diseaseCourseRepository;
        }

        public async Task<List<TreatmentDiseaseCourse>> GetTreatmentDiseaseCourses(Guid provinceId, DateTime dateFrom, DateTime dateTo)
        {
            return await context.TreatmentDiseaseCourses
            .Include(x => x.DiseaseCourse)
            .Include(x => x.Treatment)
            .ThenInclude(x => x.Patient.City)
            .Where(x => x.Treatment.Patient.City.ProvinceId == provinceId && x.Date >= dateFrom && x.Date <= dateTo)
           .ToListAsync();
        }

        public async Task<DbResult<TreatmentDiseaseCourse>> AddTreatmentDiseaseCourse(TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourse, Guid issuerId)
        {
            var diseaseCourse = await diseaseCourseRepository.GetDiseaseCourse(treatmentDiseaseCourse.DiseaseCourseId.Value);

            var conditions = new Dictionary<Func<bool>, string>
            {
                { () => patientRepository.GetPatient(treatmentDiseaseCourse.PatientId.Value).Result == null, "Pacjent nie istnieje" },
                { () => diseaseCourse == null, "Powikłanie nie istnieje" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var treatment = await treatmentRepository.TreatmentLasts(treatmentDiseaseCourse.PatientId.Value);
            if (treatment == null)
            {
                var newTreatment = new TreatmentCreateDTO
                {
                    StartDate = treatmentDiseaseCourse.Date,
                    PatientId = treatmentDiseaseCourse.PatientId.Value,
                    TreatmentStatusId = treatmentStatusRepository.GetTreatmentStatusByName(TreatmentStatusOptions.Started).Result.Id //status "Rozpoczęto"
                };
                var info = await treatmentRepository.AddTreatment(newTreatment, issuerId);
                treatment = info.Data;
            }

            conditions.Clear();
            conditions.Add(() => treatmentDiseaseCourse.Date < treatment.StartDate, "Nie można dodać wpisu przed datą rozpoczęcia leczenia");
            conditions.Add(() => context.TreatmentDiseaseCourses.AnyAsync(x => treatmentDiseaseCourse.Date < x.Date && x.TreatmentId == treatment.Id).Result, "Nie można dodać wpisu przed innym powikłaniem");

            result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var newTreatmentDiseaseCourse = mapper.Map<TreatmentDiseaseCourse>(treatmentDiseaseCourse);

            newTreatmentDiseaseCourse.TreatmentId = treatment.Id;
            newTreatmentDiseaseCourse.UserId = issuerId;

            await context.TreatmentDiseaseCourses.AddAsync(newTreatmentDiseaseCourse);
            await context.SaveChangesAsync();

            return DbResult<TreatmentDiseaseCourse>.CreateSuccess("Powikłanie zostało dodane", newTreatmentDiseaseCourse);
        }

        public async Task<DbResult<List<TreatmentDiseaseCourse>>> ShowTreatmentDiseaseCourses(Guid patientId)
        {
            if (await patientRepository.GetPatient(patientId) == null)
            {
                return DbResult<List<TreatmentDiseaseCourse>>.CreateFail("Pacjent nie istnieje");
            }

            var data =  await context.TreatmentDiseaseCourses
                .Include(x => x.DiseaseCourse)
                .Where(x => x.Treatment.PatientId == patientId)
                .ToListAsync();

            return DbResult<List<TreatmentDiseaseCourse>>.CreateSuccess("Powodzenie", data);
        }

        public async Task<DbResult<TreatmentDiseaseCourse>> EditTreatmentDiseaseCourse(TreatmentDiseaseCourseUpdateDTO treatmentDiseaseCourse, Guid issuerId)
        {
            var checkTreatmentDiseaseCourse = await GetTreatmentDiseaseCourse(treatmentDiseaseCourse.Id);

            if (checkTreatmentDiseaseCourse == null)
            {
                return DbResult<TreatmentDiseaseCourse>.CreateFail("Powikłanie nie istnieje");
            }

            var treatment = await treatmentRepository.GetTreatment(checkTreatmentDiseaseCourse.TreatmentId.Value);
            var diseaseCourse = await diseaseCourseRepository.GetDiseaseCourse(treatmentDiseaseCourse.DiseaseCourseId);
            var newest = await context.TreatmentDiseaseCourses.OrderByDescending(x => x.Date).FirstOrDefaultAsync(x => x.TreatmentId == checkTreatmentDiseaseCourse.TreatmentId);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => checkTreatmentDiseaseCourse.UserId != issuerId, "Tylko użytkownik, który dodał powikłanie może je edytować"},
                { () => treatment.EndDate != null, "Tego powikłania nie można już edytować - leczenie zostało zakończone"},
                { () => checkTreatmentDiseaseCourse.Id != newest.Id,  "Nie można edytować wpisu przed innym powikłaniem" },
                { () => treatmentDiseaseCourse.Date < treatment.StartDate, "Nie można dodać wpisu przed datą rozpoczęcia leczenia"},
                { () => context.TreatmentDiseaseCourses.AnyAsync(x => x.Id != treatmentDiseaseCourse.Id && treatmentDiseaseCourse.Date < x.Date && x.TreatmentId == checkTreatmentDiseaseCourse.TreatmentId).Result, "Nie można dodać wpisu przed innym powikłaniem" },
                { () => diseaseCourse == null, "Powikłanie nie znalezione" },
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            mapper.Map(treatmentDiseaseCourse, checkTreatmentDiseaseCourse);

            checkTreatmentDiseaseCourse.DiseaseCourse = diseaseCourse;

            context.Update(checkTreatmentDiseaseCourse);
            await context.SaveChangesAsync();

            return DbResult<TreatmentDiseaseCourse>.CreateSuccess("Powikłanie zostało zedytowane", checkTreatmentDiseaseCourse);
        }

        private async Task<TreatmentDiseaseCourse> GetTreatmentDiseaseCourse(Guid treatmentDiseaseCourseId) => await context.TreatmentDiseaseCourses.FirstOrDefaultAsync(x => x.Id == treatmentDiseaseCourseId);

        public async Task<DbResult<TreatmentDiseaseCourse>> ShowTreatmentDiseaseCourseDetails(Guid treatmentDiseaseCourseId)
        {
            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => GetTreatmentDiseaseCourse(treatmentDiseaseCourseId).Result == null, "Powikłanie nie istnieje" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var data = await context.TreatmentDiseaseCourses
                .Include(x => x.DiseaseCourse)
                .Include(x => x.Treatment)
                .Include(x => x.User.Role)
                .FirstOrDefaultAsync(x => x.Id == treatmentDiseaseCourseId);

            return DbResult<TreatmentDiseaseCourse>.CreateSuccess("Powodzenie", data);
        }
    }
}
