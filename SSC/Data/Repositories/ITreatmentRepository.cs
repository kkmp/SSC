using SSC.Data.Models;
using SSC.DTO.Treatment;

namespace SSC.Data.Repositories
{
    public interface ITreatmentRepository
    {
        Task<List<Treatment>> GetTreatments(Guid provinceId, DateTime dateFrom, DateTime dateTo);
        Task<DbResult<Treatment>> AddTreatment(TreatmentCreateDTO treatment, Guid issuerId);
        Task<Treatment> TreatmentLasts(Guid patientId);
        Task<DbResult<List<Treatment>>> ShowTreatments(Guid patientId);
        Task<DbResult<Treatment>> ShowTreatmentDetails(Guid treatmentId);
        Task<DbResult<Treatment>>EditTreatment(TreatmentUpdateDTO treatment, Guid issuerId);
        Task<Treatment> GetTreatment(Guid treatmentId);

    }
}
