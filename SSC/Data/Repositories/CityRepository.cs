using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public class CityRepository : ICityRepository
    {

        private readonly DataContext context;

        public CityRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<City> GetCityByName(string cityName)
        {
            return await context.Cities.FirstOrDefaultAsync(x => x.Name == cityName);
        }
    }
}
