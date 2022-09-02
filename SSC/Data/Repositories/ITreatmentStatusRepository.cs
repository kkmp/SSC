using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface ITreatmentStatusRepository
    {
        Task<TreatmentStatus> GetTreatmentStatusByName(string treatmentStatusName);
    }
}
