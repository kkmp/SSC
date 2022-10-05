using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface ITreatmentStatusRepository
    {
        Task<TreatmentStatus> GetTreatmentStatusByName(string treatmentStatusName);
        Task<TreatmentStatus> GetTreatmentStatus(Guid treatmentStatusId);
        Task<List<TreatmentStatus>> GetTreatmentStatuses();
    }
}
