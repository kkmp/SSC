using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface ITreatmentDiseaseCoursesRepository
    {
        Task<List<TreatmentDiseaseCourse>> GetTreatmentDiseaseCourses(Guid provinceId, DateTime dateFrom, DateTime dateTo);
    }
}
