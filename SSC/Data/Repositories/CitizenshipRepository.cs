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

        public async Task<bool> AnyCitizenship(string citizenshipName)
        {
            return await context.Citizenships.AnyAsync(x => x.Name == citizenshipName);
        }

        public async Task<Citizenship> GetCitizenshipByName(string citizenshipName)
        {
            return await context.Citizenships.FirstOrDefaultAsync(x => x.Name == citizenshipName);
        }
    }
}
