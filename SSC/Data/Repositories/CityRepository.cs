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

        public async Task<List<City>> GetCities()
        {
            return await context.Cities.ToListAsync();
        }

        public async Task<City> GetCity(Guid cityId)
        {
            return await context.Cities.FirstOrDefaultAsync(x => x.Id == cityId);
        }
    }
}
