using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.DTO.Treatment;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public class TreatmentRepository : BaseRepository<Treatment>, ITreatmentRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;
        private readonly IPatientRepository patientRepository;
        private readonly ITreatmentStatusRepository treatmentStatusRepository;

        public TreatmentRepository(DataContext context, IMapper mapper, IPatientRepository patientRepository, ITreatmentStatusRepository treatmentStatusRepository)
        {
            this.context = context;
            this.mapper = mapper;
            this.patientRepository = patientRepository;
            this.treatmentStatusRepository = treatmentStatusRepository;
        }

        public async Task<DbResult<Treatment>> AddTreatment(TreatmentCreateDTO treatment, Guid issuerId)
        {
            var treatmentStatus = await treatmentStatusRepository.GetTreatmentStatusByName(treatment.TreatmentStatusName);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => patientRepository.GetPatient(treatment.PatientId.Value).Result == null, "Patient does not exist" },
                { () => treatmentStatus == null, "Treatment status does not exist" },
                { () => context.Treatments.AnyAsync(x => x.PatientId == treatment.PatientId && x.EndDate == null).Result, "Last treatment is not ended"},
                { () => context.Treatments.AnyAsync(x => treatment.StartDate > x.StartDate && treatment.StartDate < x.EndDate && x.PatientId == treatment.PatientId).Result, "Incorrect treatment start date" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var newTreatment = mapper.Map<Treatment>(treatment);

            newTreatment.UserId = issuerId;
            newTreatment.TreatmentStatus = treatmentStatus;

            await context.Treatments.AddAsync(newTreatment);
            await context.SaveChangesAsync();

            return DbResult<Treatment>.CreateSuccess("Treatment added", newTreatment);
        }

        public async Task<DbResult<Treatment>> EditTreatment(TreatmentUpdateDTO treatment, Guid issuerId)
        {
            var treatmentToCheck = await context.Treatments.FirstOrDefaultAsync(x => x.Id == treatment.Id);
            var treatmentStatus = await treatmentStatusRepository.GetTreatmentStatusByName(treatment.TreatmentStatusName);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () =>  treatmentToCheck == null, "Treatment does not exist"},
                { () =>  treatmentToCheck.UserId != issuerId, "Only the user who added the treatment can edit"},
                { () =>  treatmentStatus == null, "Treatment status does not exist" },
                { () =>  treatmentToCheck.EndDate != null, "The treatment cannot be edited anymore - the treatment has been ended"},
                { () =>  treatment.StartDate > treatment.EndDate, "Treatment start date cannot be earlier than the end date"},
                { () =>  context.Treatments.AnyAsync(x => treatment.StartDate < x.EndDate && treatment.Id != x.Id && x.PatientId == treatmentToCheck.PatientId).Result, "The tratment start date cannot be older than another tratment end date."},
                { () =>  context.Tests.AnyAsync(x => x.TreatmentId == treatment.Id && treatment.StartDate > x.TestDate).Result, "Date of treatment cannot be after existing test" },
                { () =>  context.TreatmentDiseaseCourses.AnyAsync(x => x.TreatmentId == treatment.Id && treatment.StartDate > x.Date).Result, "Date of treatment cannot be after existing treatment disease course entry" },
                { () =>  context.Tests.AnyAsync(x => x.TreatmentId == treatment.Id && x.ResultDate == null).Result, "Some tests in treatment have no result date"}
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            mapper.Map(treatment, treatmentToCheck);

            treatmentToCheck.TreatmentStatus = treatmentStatus;

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

        public async Task<DbResult<List<Treatment>>> ShowTreatments(Guid patientId)
        {
            if (await patientRepository.GetPatient(patientId) == null)
            {
                return DbResult<List<Treatment>>.CreateFail("Patient does not exist");
            }

            var result = await context.Treatments
                .Include(x => x.User.Role)
                .Include(x => x.TreatmentStatus)
                .Where(x => x.PatientId == patientId).ToListAsync();

            return DbResult<List<Treatment>>.CreateSuccess("Success", result);
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
