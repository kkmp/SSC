using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public interface IMedicalHistoryRepository
    {
        Task<DbResult<MedicalHistory>> AddMedicalHistory(MedicalHistoryViewModel medicalHistory, Guid userId);
        Task<DbResult<MedicalHistory>> EditMedicalHistory(EditMedicalHistoryViewModel medicalHistory, Guid userId);
    }
}
