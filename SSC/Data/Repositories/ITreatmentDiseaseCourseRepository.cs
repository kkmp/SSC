using SSC.Data.Models;
using SSC.DTO.TreatmentDiseaseCourse;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public interface ITreatmentDiseaseCourseRepository
    {
        Task<List<TreatmentDiseaseCourse>> GetTreatmentDiseaseCourses(Guid provinceId, DateTime dateFrom, DateTime dateTo);
        Task<DbResult<TreatmentDiseaseCourse>> AddTreatmentDiseaseCourse(TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourse, Guid issuerId);
        Task<DbResult<List<TreatmentDiseaseCourse>>> ShowTreatmentDiseaseCourses(Guid patientId);
        Task<DbResult<TreatmentDiseaseCourse>> EditTreatmentDiseaseCourse(TreatmentDiseaseCourseUpdateDTO treatmentDiseaseCourse, Guid issuerId);
    }
}
