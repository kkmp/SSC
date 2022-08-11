using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public class TreatmentDiseaseCoursesRepository : ITreatmentDiseaseCoursesRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public TreatmentDiseaseCoursesRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<List<TreatmentDiseaseCourse>> GetTreatmentDiseaseCourses(Guid provinceId, DateTime dateFrom, DateTime dateTo)
        {
            return await context.TreatmentDiseaseCourses
                .Include(x => x.DiseaseCourse)
                .Include(x => x.Patient)
                .ThenInclude(x => x.City)
                .Where(x => x.Patient.City.ProvinceId == provinceId && x.Date >= dateFrom && x.Date <= dateTo)
               .ToListAsync();        
        }

        public async Task<DbResult<TreatmentDiseaseCourse>> AddTreatmentDiseaseCourse(TreatmentDiseaseCoursesViewModel treatmentDiseaseCourse, Guid id)
        {
            if (await context.TreatmentDiseaseCourses.AnyAsync(x => x.Date > treatmentDiseaseCourse.Date))
            {
                return DbResult<TreatmentDiseaseCourse>.CreateFail("Cannot add treatment disease course because there is an entry with earlier date");
            }

            var newTreatmentDiseaseCourse = mapper.Map<TreatmentDiseaseCourse>(treatmentDiseaseCourse);

            newTreatmentDiseaseCourse.DiseaseCourse = await context.DiseaseCourses.FirstOrDefaultAsync(x => x.Name == treatmentDiseaseCourse.DiseaseCourseName);

            await context.TreatmentDiseaseCourses.AddAsync(newTreatmentDiseaseCourse);
            await context.SaveChangesAsync();

            return DbResult<TreatmentDiseaseCourse>.CreateSuccess("Treatment disease course added", newTreatmentDiseaseCourse);

        }

        public async Task<List<TreatmentDiseaseCourse>> ShowTreatmentDiseaseCourses(Guid patientId)
        {
            return await context.TreatmentDiseaseCourses
                .Include(x => x.DiseaseCourse)
                .Where(x => x.PatientId == patientId)
                .ToListAsync();
        }
    }
}
