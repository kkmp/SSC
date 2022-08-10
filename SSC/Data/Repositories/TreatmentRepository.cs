using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public class TreatmentRepository : ITreatmentRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public TreatmentRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<DbResult<Treatment>> AddTreatment(TreatmentViewModel treatment, Guid id)
        {
            if (await context.Treatments.AnyAsync(x => x.PatientId == treatment.PatientId && x.EndDate == null))
            {
                return DbResult<Treatment>.CreateFail("Last treatment is not ended");
            }

            if(await context.Treatments.AnyAsync(x => treatment.StartDate > x.StartDate && treatment.StartDate < x.EndDate))
            {
                return DbResult<Treatment>.CreateFail("Incorrect treatment start date");
            }

            var newTreatment = mapper.Map<Treatment>(treatment);
            newTreatment.UserId = id;
            newTreatment.TreatmentStatus = await context.TreatmentStatuses.FirstOrDefaultAsync(x => x.Name == treatment.TreatmentStatusName);
            await context.Treatments.AddAsync(newTreatment);
            await context.SaveChangesAsync();
            return DbResult<Treatment>.CreateSuccess("Treatment added", newTreatment);
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

        public async Task<List<Treatment>> ShowTreatments(Guid patientId)
        {
            return await context.Treatments
                .Include(x => x.User.Role)
                .Include(x => x.TreatmentStatus)
                .Where(x => x.PatientId == patientId).ToListAsync();
        }

        public async Task<Treatment> TreatmentLasts(Guid patientId)
        {
            return await context.Treatments
                .Include(x => x.TreatmentStatus)
               .Include(x => x.Patient)
               .ThenInclude(x => x.City)
               .FirstOrDefaultAsync(x => x.PatientId == patientId && x.EndDate == null);
        }
    }
}
