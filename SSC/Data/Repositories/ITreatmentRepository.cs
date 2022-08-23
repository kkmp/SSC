using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public interface ITreatmentRepository
    {
        Task<List<Treatment>> GetTreatments(Guid provinceId, DateTime dateFrom, DateTime dateTo);
        Task<DbResult<Treatment>> AddTreatment(TreatmentViewModel treatment, Guid issuerId);
        Task<Treatment> TreatmentLasts(Guid patientId);
        Task<List<Treatment>> ShowTreatments(Guid patientId);
        Task<DbResult<Treatment>>EditTreatment(TreatmentEditViewModel treatment, Guid issuerId);
        Task<Treatment> GetTreatment(Guid treatmentId);

    }
}
