using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public interface ITreatmentDiseaseCoursesRepository
    {
        Task<List<TreatmentDiseaseCourse>> GetTreatmentDiseaseCourses(Guid provinceId, DateTime dateFrom, DateTime dateTo);
        Task<DbResult<TreatmentDiseaseCourse>> AddTreatmentDiseaseCourse(TreatmentDiseaseCoursesViewModel treatmentDiseaseCourse, Guid id);
        Task<List<TreatmentDiseaseCourse>> ShowTreatmentDiseaseCourses(Guid patientId);
    }
}