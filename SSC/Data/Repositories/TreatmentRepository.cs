using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public class TreatmentRepository : BaseRepository<Treatment>, ITreatmentRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public TreatmentRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<DbResult<Treatment>> AddTreatment(TreatmentViewModel treatment, Guid issuerId)
        {
            if (await context.Treatments.AnyAsync(x => x.PatientId == treatment.PatientId && x.EndDate == null))
            {
                return DbResult<Treatment>.CreateFail("Last treatment is not ended");
            }

            if(await context.Treatments.AnyAsync(x => treatment.StartDate > x.StartDate && treatment.StartDate < x.EndDate && x.PatientId == treatment.PatientId))
            {
                return DbResult<Treatment>.CreateFail("Incorrect treatment start date");
            }

            var newTreatment = mapper.Map<Treatment>(treatment);
            newTreatment.UserId = issuerId;
            newTreatment.TreatmentStatus = await context.TreatmentStatuses.FirstOrDefaultAsync(x => x.Name == treatment.TreatmentStatusName);
            await context.Treatments.AddAsync(newTreatment);
            await context.SaveChangesAsync();
            return DbResult<Treatment>.CreateSuccess("Treatment added", newTreatment);
        }

        public async Task<DbResult<Treatment>> EditTreatment(TreatmentEditViewModel treatment, Guid issuerId)
        {
            var treatmentToCheck = await context.Treatments.FirstOrDefaultAsync(x => x.Id == treatment.Id);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () =>  treatmentToCheck == null, "Treatment does not exist"},
                { () =>  treatmentToCheck.UserId != issuerId, "Only the user who added the treatment can edit"},
                { () =>  treatmentToCheck.EndDate != null, "The treatment cannot be edited anymore - the treatment has been ended"},
               // { () =>  context.Treatments.AnyAsync(x => treatment.StartDate < x.EndDate && treatment.Id != x.Id && x.PatientId == treatmentToCheck.PatientId).Result, "The tratment date cannot be older than another tratment start date."},
                { () =>  context.Tests.AnyAsync(x => x.TreatmentId == treatment.Id && treatment.StartDate > x.TestDate).Result, "Date of treatment cannot be after existing test" },
                { () =>  context.TreatmentDiseaseCourses.AnyAsync(x => x.TreatmentId == treatment.Id && treatment.StartDate > x.Date).Result, "Date of treatment cannot be after existing treatment disease course entry" },
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            mapper.Map(treatment, treatmentToCheck);
            treatmentToCheck.TreatmentStatus = await context.TreatmentStatuses.FirstOrDefaultAsync(x => x.Name == treatment.TreatmentStatusName);

            context.Update(treatmentToCheck);
            await context.SaveChangesAsync();
            return DbResult<Treatment>.CreateSuccess("Treatment has been edited", treatmentToCheck);
        }

        public async Task<Treatment> GetTreatment(Guid treatmentId)
        {
            return await context.Treatments.FirstOrDefaultAsync(x => x.Id == treatmentId);
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
