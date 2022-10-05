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

        public async Task<TreatmentStatus> GetTreatmentStatus(Guid treatmentStatusId)
        {
            return await context.TreatmentStatuses.FirstOrDefaultAsync(x => x.Id == treatmentStatusId);
        }

        public async Task<TreatmentStatus> GetTreatmentStatusByName(string treatmentStatusName)
        {
            return await context.TreatmentStatuses.FirstOrDefaultAsync(x => x.Name == treatmentStatusName);
        }

        public async Task<List<TreatmentStatus>> GetTreatmentStatuses()
        {
            return await context.TreatmentStatuses.ToListAsync();
        }
    }
}
