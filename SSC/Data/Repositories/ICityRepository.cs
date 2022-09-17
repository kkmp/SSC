using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface ICityRepository
    {
        Task<City> GetCity(Guid cityId);
        Task<List<City>> GetCities();
    }
}
