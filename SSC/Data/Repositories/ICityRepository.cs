using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface ICityRepository
    {
        Task<City> GetCityByName(string cityName);
    }
}
