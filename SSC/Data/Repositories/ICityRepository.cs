using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface ICityRepository
    {
        Task<bool> AnyCity(string cityName);
        Task<City> GetCityByName(string cityName);
    }
}
