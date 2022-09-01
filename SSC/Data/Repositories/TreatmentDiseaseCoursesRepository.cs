using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public class TreatmentDiseaseCoursesRepository : BaseRepository<TreatmentDiseaseCourse>, ITreatmentDiseaseCourseRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;
        private readonly IPatientRepository patientRepository;
        private readonly ITreatmentRepository treatmentRepository;

        public TreatmentDiseaseCoursesRepository(DataContext context, IMapper mapper, IPatientRepository patientRepository, ITreatmentRepository treatmentRepository)
        {
            this.context = context;
            this.mapper = mapper;
            this.patientRepository = patientRepository;
            this.treatmentRepository = treatmentRepository;
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

        public async Task<DbResult<TreatmentDiseaseCourse>> AddTreatmentDiseaseCourse(TreatmentDiseaseCourseViewModel treatmentDiseaseCourse, Guid issuerId)
        {
            var diseaseCourse = await context.DiseaseCourses.FirstOrDefaultAsync(x => x.Name == treatmentDiseaseCourse.DiseaseCourseName);

            var conditions = new Dictionary<Func<bool>, string>
            {
                { () => patientRepository.GetPatient(treatmentDiseaseCourse.PatientId.Value).Result == null, "Patient does not exist" },
                { () => diseaseCourse == null, "Disease course does not exist" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var treatment = await treatmentRepository.TreatmentLasts(treatmentDiseaseCourse.PatientId.Value);
            if (treatment == null)
            {
                var newTreatment = new TreatmentViewModel
                {
                    StartDate = DateTime.Now,
                    PatientId = treatmentDiseaseCourse.PatientId.Value,
                    TreatmentStatusName = "Rozpoczęto" //z seedera
                };
                var info = await treatmentRepository.AddTreatment(newTreatment, issuerId);
                treatment = info.Data;
            }

            conditions.Clear();
            conditions.Add(() => treatmentDiseaseCourse.Date < treatment.StartDate, "Cannot add entry before treatment start date");
            conditions.Add(() => context.TreatmentDiseaseCourses.AnyAsync(x => treatmentDiseaseCourse.Date < x.Date && x.TreatmentId == treatment.Id).Result, "Cannot add entry before another treatment disease course");

            result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var newTreatmentDiseaseCourse = mapper.Map<TreatmentDiseaseCourse>(treatmentDiseaseCourse);

            newTreatmentDiseaseCourse.TreatmentId = treatment.Id;
            newTreatmentDiseaseCourse.DiseaseCourse = diseaseCourse;
            newTreatmentDiseaseCourse.UserId = issuerId;

            await context.TreatmentDiseaseCourses.AddAsync(newTreatmentDiseaseCourse);
            await context.SaveChangesAsync();

            return DbResult<TreatmentDiseaseCourse>.CreateSuccess("Treatment disease course added", newTreatmentDiseaseCourse);
        }

        public async Task<DbResult<List<TreatmentDiseaseCourse>>> ShowTreatmentDiseaseCourses(Guid patientId)
        {
            if (await patientRepository.GetPatient(patientId) == null)
            {
                return DbResult<List<TreatmentDiseaseCourse>>.CreateFail("Patient does not exist");
            }

            var data =  await context.TreatmentDiseaseCourses
                .Include(x => x.DiseaseCourse)
                .Where(x => x.Treatment.PatientId == patientId)
                .ToListAsync();

            return DbResult<List<TreatmentDiseaseCourse>>.CreateSuccess("Success", data);
        }

        public async Task<DbResult<TreatmentDiseaseCourse>> EditTreatmentDiseaseCourse(TreatmentDiseaseCourseEditViewModel treatmentDiseaseCourse, Guid issuerId)
        {
            var checkTreatmentDiseaseCourse = await GetTreatmentDiseaseCourse(treatmentDiseaseCourse.Id);

            if (checkTreatmentDiseaseCourse == null)
            {
                return DbResult<TreatmentDiseaseCourse>.CreateFail("Treatment disease course entry does not exist");
            }

            var treatment = await treatmentRepository.GetTreatment(checkTreatmentDiseaseCourse.TreatmentId.Value);
            var diseaseCourse = await context.DiseaseCourses.FirstOrDefaultAsync(x => x.Name == treatmentDiseaseCourse.DiseaseCourseName);
            var newest = await context.TreatmentDiseaseCourses.OrderByDescending(x => x.Date).FirstOrDefaultAsync(x => x.TreatmentId == checkTreatmentDiseaseCourse.TreatmentId);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => checkTreatmentDiseaseCourse.UserId != issuerId, "Only the user who added the treatment disease course entry can edit"},
                { () => treatment.EndDate != null, "The treatment disease course entry cannot be edited anymore - the treatment has been ended"},
                { () => checkTreatmentDiseaseCourse.Id != newest.Id,  "The treatment disease course cannot be edited anymore" },
                { () => treatmentDiseaseCourse.Date < treatment.StartDate, "The treatment disease course entry date cannot be earlier than the treatment start date"},
                { () => context.TreatmentDiseaseCourses.AnyAsync(x => x.Id != treatmentDiseaseCourse.Id && treatmentDiseaseCourse.Date < x.Date && x.TreatmentId == checkTreatmentDiseaseCourse.TreatmentId).Result, "Cannot edit entry before another treatment disease course" },
                { () => diseaseCourse == null, "Disease course not found" },
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

            return DbResult<TreatmentDiseaseCourse>.CreateSuccess("Treatment disease course entry has been edited", checkTreatmentDiseaseCourse);
        }

        private async Task<TreatmentDiseaseCourse> GetTreatmentDiseaseCourse(Guid treatmentDiseaseCourseId) => await context.TreatmentDiseaseCourses.FirstOrDefaultAsync(x => x.Id == treatmentDiseaseCourseId);
    }
}
