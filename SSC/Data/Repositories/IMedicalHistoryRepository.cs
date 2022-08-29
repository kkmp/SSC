using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public interface IMedicalHistoryRepository
    {
        Task<DbResult<MedicalHistory>> AddMedicalHistory(MedicalHistoryViewModel medicalHistory, Guid issuerId);
        Task<DbResult<MedicalHistory>> EditMedicalHistory(MedicalHistoryEditViewModel medicalHistory, Guid issuerId);
        Task<DbResult<List<MedicalHistory>>> ShowMedicalHistories(Guid patientId);
    }
}