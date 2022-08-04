using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public class TreatmentDiseaseCoursesRepository : ITreatmentDiseaseCoursesRepository
    {
        private readonly DataContext context;

        public TreatmentDiseaseCoursesRepository(DataContext context)
        {
            this.context = context;
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
    }
}
