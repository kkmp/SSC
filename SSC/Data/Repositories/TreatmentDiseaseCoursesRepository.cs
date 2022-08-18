using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public class TreatmentDiseaseCoursesRepository : BaseRepository<TreatmentDiseaseCourse>, ITreatmentDiseaseCoursesRepository
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
            /*return await context.TreatmentDiseaseCourses
                .Include(x => x.DiseaseCourse)
                //.Include(x => x.Patient)
                //.ThenInclude(x => x.City)
                .Where(x => x.Patient.City.ProvinceId == provinceId && x.Date >= dateFrom && x.Date <= dateTo)
               .ToListAsync();      
            */
            return null;
        }

        public async Task<DbResult<TreatmentDiseaseCourse>> AddTreatmentDiseaseCourse(TreatmentDiseaseCoursesViewModel treatmentDiseaseCourse, Guid id)
        {
            var patient = await patientRepository.GetPatient(treatmentDiseaseCourse.PatientId.Value);
            var diseaseCourse = await context.DiseaseCourses.FirstOrDefaultAsync(x => x.Name == treatmentDiseaseCourse.DiseaseCourseName);
            var conditions = new Dictionary<Func<bool>, string>
            {
                { () => patient == null, "Patient not found" },
                { () => diseaseCourse == null, "Disease Course not found" }
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
                    TreatmentStatusName = "Rozpoczęto"
                };
                var info = await treatmentRepository.AddTreatment(newTreatment, id);
                treatment = info.Data;
            }

            if (treatmentDiseaseCourse.Date < treatment.StartDate)
            {
                return DbResult<TreatmentDiseaseCourse>.CreateFail("Test date cannot be older than treatment start date");
            }

            var newTreatmentDiseaseCourse = mapper.Map<TreatmentDiseaseCourse>(treatmentDiseaseCourse);
            newTreatmentDiseaseCourse.TreatmentId = treatment.Id;
            newTreatmentDiseaseCourse.DiseaseCourse = diseaseCourse;
            newTreatmentDiseaseCourse.UserId = id;
            await context.TreatmentDiseaseCourses.AddAsync(newTreatmentDiseaseCourse);
            await context.SaveChangesAsync();

            return DbResult<TreatmentDiseaseCourse>.CreateSuccess("Treatment disease course added", newTreatmentDiseaseCourse);
        }

        public async Task<List<TreatmentDiseaseCourse>> ShowTreatmentDiseaseCourses(Guid patientId)
        {
            return await context.TreatmentDiseaseCourses
                .Include(x => x.DiseaseCourse)
                .Where(x => x.Treatment.PatientId == patientId)
                .ToListAsync();
            return null;
        }

        public async Task<DbResult<TreatmentDiseaseCourse>> EditTreatmentDiseaseCourses(TreatmentDiseaseCourseEditViewModel treatmentDiseaseCourse, Guid id)
        {
            var chekTreatmentDiseaseCourse = await context.TreatmentDiseaseCourses.FirstOrDefaultAsync(x => treatmentDiseaseCourse.Id == x.Id);
            var treatment = await context.Treatments.FirstOrDefaultAsync(x => x.Id == chekTreatmentDiseaseCourse.TreatmentId);
            var diseaseCourse = await context.DiseaseCourses.FirstOrDefaultAsync(x => x.Name == treatmentDiseaseCourse.DiseaseCourseName);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () =>  treatmentDiseaseCourse == null, "Treatment disease course entry does not exist"},
                { () =>  chekTreatmentDiseaseCourse.UserId != id, "Only the user who added the treatment disease course can edit"},
                { () =>  treatment.EndDate != null, "The treatment disease course cannot be edited anymore - the treatment has been ended"},
                { () =>  treatmentDiseaseCourse.Date < treatment.StartDate, "The test date and result date cannot be earlier than the treatment start date"},
                { () =>  context.TreatmentDiseaseCourses.OrderByDescending(x => x.Date).FirstOrDefaultAsync().Result.Date > treatmentDiseaseCourse.Date, "Cannot add entry before another treatment disease course" },
                { () => diseaseCourse == null, "Disease course not found" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }


            mapper.Map(treatmentDiseaseCourse, chekTreatmentDiseaseCourse);
            chekTreatmentDiseaseCourse.DiseaseCourse = diseaseCourse;

            context.Update(chekTreatmentDiseaseCourse);
            await context.SaveChangesAsync();
            return DbResult<TreatmentDiseaseCourse>.CreateSuccess("Test has been edited", chekTreatmentDiseaseCourse);
        }
    }
}
