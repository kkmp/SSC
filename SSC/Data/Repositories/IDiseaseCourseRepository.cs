using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface IDiseaseCourseRepository
    {
        Task<DiseaseCourse> GetDiseaseCourseByName(string diseaseCourseName);
    }
}
