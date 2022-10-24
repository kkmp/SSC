using SSC.Data.Models;
using SSC.DTO.MedicalHistory;

namespace SSC.Data.Repositories
{
    public interface IMedicalHistoryRepository
    {
        Task<DbResult<MedicalHistory>> AddMedicalHistory(MedicalHistoryCreateDTO medicalHistory, Guid issuerId);
        Task<DbResult<MedicalHistory>> EditMedicalHistory(MedicalHistoryUpdateDTO medicalHistory, Guid issuerId);
        Task<DbResult<List<MedicalHistory>>> ShowMedicalHistories(Guid patientId);
        Task<DbResult<MedicalHistory>> ShowMedicalHistoryDetails(Guid medicalHistoryId);
    }
}