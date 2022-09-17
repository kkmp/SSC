using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public class CitizenshipRepository : ICitizenshipRepository
    {
        private readonly DataContext context;

        public CitizenshipRepository(DataContext context)
        {
            this.context = context;
        }
        public async Task<List<Citizenship>> GetCitizenships()
        {
            return await context.Citizenships.ToListAsync();
        }

        public async Task<Citizenship> GetCitizenship(Guid citizenshipId)
        {
            return await context.Citizenships.FirstOrDefaultAsync(x => x.Id == citizenshipId);
        }
    }
}
