using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public class ProvinceRepository : IProvinceRepository
    {
        private readonly DataContext context;

        public ProvinceRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<List<Province>> GetProvinces()
        {
            return await context.Provinces.ToListAsync();
        }
    }
}
