using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public class TreatmentRepository : ITreatmentRepository
    {
        private readonly DataContext context;

        public TreatmentRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<DbResult<Treatment>> AddTreatment(TreatmentViewModel treatment, Guid id)
        {
            //zabezpiecznie przed dodaniem drugiego treatment gdy poprzedni nie zakończony
            //var treatments = await context.Treatments.FirstOrDefaultAsync(x => x.EndDate == null)
            //if ()
            {

            }
            var status = await context.TreatmentStatuses.FirstOrDefaultAsync(x => x.Name == treatment.TreatmentStatus);
            var newTreatment = new Treatment {
                StartDate = treatment.StartDate,
                EndDate = treatment.EndDate,
                IsCovid = treatment.IsCovid,
                Patient = await context.Patients.FirstOrDefaultAsync(x => x.Id == treatment.PatientId),
                User = await context.Users.FirstOrDefaultAsync(x => x.Id == id),
                UserId = id,
                TreatmentStatus = status,
                TreatmentStatusId = status.Id,
            };
        }

        public async Task<List<Treatment>> GetTreatments(Guid provinceId, DateTime dateFrom, DateTime dateTo)
        {
            return await context.Treatments
               .Include(x => x.TreatmentStatus)
               .Include(x => x.Patient)
               .ThenInclude(x => x.City)
               .Where(x => x.Patient.City.ProvinceId == provinceId && ((x.EndDate >= dateFrom && x.EndDate <= dateTo) || (x.EndDate == null && x.StartDate >= dateFrom && x.StartDate <= dateTo)))
              .ToListAsync();
        }
    }
}
