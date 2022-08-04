using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public interface ITreatmentRepository
    {

        Task<List<Treatment>> GetTreatments(Guid provinceId, DateTime dateFrom, DateTime dateTo);
        Task<DbResult<Treatment>> AddTreatment(TreatmentViewModel treatment, Guid id);
        Task<Treatment> TreatmentLasts(); //zwraca treatment nieskończony (z nullem)
                                          //gdy nie zwróci znaczy, że wszystkie skończone i można dodać nowy
    }
}
