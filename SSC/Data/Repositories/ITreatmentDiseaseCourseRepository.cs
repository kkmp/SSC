using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public interface ITreatmentDiseaseCourseRepository
    {
        Task<List<TreatmentDiseaseCourse>> GetTreatmentDiseaseCourses(Guid provinceId, DateTime dateFrom, DateTime dateTo);
        Task<DbResult<TreatmentDiseaseCourse>> AddTreatmentDiseaseCourse(TreatmentDiseaseCourseViewModel treatmentDiseaseCourse, Guid issuerId);
        Task<DbResult<List<TreatmentDiseaseCourse>>> ShowTreatmentDiseaseCourses(Guid patientId);
        Task<DbResult<TreatmentDiseaseCourse>> EditTreatmentDiseaseCourse(TreatmentDiseaseCourseEditViewModel treatmentDiseaseCourse, Guid issuerId);
    }
}
