using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public class TreatmentStatusRepository : ITreatmentStatusRepository
    {
        private readonly DataContext context;

        public TreatmentStatusRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<TreatmentStatus> GetTreatmentStatusByName(string treatmentStatusName)
        {
            return await context.TreatmentStatuses.FirstOrDefaultAsync(x => x.Name == treatmentStatusName);
        }
    }
}
