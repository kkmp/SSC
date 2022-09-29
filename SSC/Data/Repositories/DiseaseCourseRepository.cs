using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public class DiseaseCourseRepository : IDiseaseCourseRepository
    {
        private readonly DataContext context;

        public DiseaseCourseRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<DiseaseCourse> GetDiseaseCourse(Guid diseaseCourseId)
        {
            return await context.DiseaseCourses.FirstOrDefaultAsync(x => x.Id == diseaseCourseId);
        }

        public async Task<DiseaseCourse> GetDiseaseCourseByName(string diseaseCourseName)
        {
            return await context.DiseaseCourses.FirstOrDefaultAsync(x => x.Name == diseaseCourseName);
        }

        public async Task<List<DiseaseCourse>> GetDiseaseCourses()
        {
            return await context.DiseaseCourses.ToListAsync();
        }
    }
}
