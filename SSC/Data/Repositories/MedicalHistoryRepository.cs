using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public class MedicalHistoryRepository : BaseRepository<MedicalHistory>, IMedicalHistoryRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;
        private readonly IPatientRepository patientRepository;

        public MedicalHistoryRepository(DataContext context, IMapper mapper, IPatientRepository patientRepository)
        {
            this.context = context;
            this.mapper = mapper;
            this.patientRepository = patientRepository;
        }

        public async Task<DbResult<MedicalHistory>> AddMedicalHistory(MedicalHistoryViewModel medicalHistory, Guid issuerId)
        {
            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => patientRepository.GetPatient(medicalHistory.PatientId.Value).Result == null, "Patient does not exist" },
                { () => context.MedicalHistories.Any(x => x.PatientId == medicalHistory.PatientId && x.Date >= medicalHistory.Date), "Cannot add medical history. There is already an entry before the given date" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var newMedicalHistory = mapper.Map<MedicalHistory>(medicalHistory);
            newMedicalHistory.UserId = issuerId;

            await context.AddAsync(newMedicalHistory);
            await context.SaveChangesAsync();

            return DbResult<MedicalHistory>.CreateSuccess("Medical history added", newMedicalHistory);
        }

        public async Task<DbResult<MedicalHistory>> EditMedicalHistory(MedicalHistoryEditViewModel medicalHistory, Guid issuerId)
        {
            var medicalHistoryUpdate = await GetMedicalHistory(medicalHistory.Id);

            if (medicalHistoryUpdate == null)
            {
                return DbResult<MedicalHistory>.CreateFail("There is no such entry in the medical history");
            }

            var newest = await context.MedicalHistories.Where(x => x.PatientId == medicalHistoryUpdate.PatientId).OrderByDescending(x => x.Date).FirstOrDefaultAsync();

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => medicalHistoryUpdate.UserId != issuerId, "Only the user who added the medical history entry can edit" },
                { () => newest.Id != medicalHistoryUpdate.Id, "This medical history cannot be edited anymore" },
                { () => context.MedicalHistories.Any(x => x.PatientId == medicalHistoryUpdate.PatientId && x.Date >= medicalHistory.Date && x.Id != medicalHistory.Id), "Cannot edit medical history. There is already an entry before the given date" },
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            medicalHistoryUpdate.Date = medicalHistory.Date;
            medicalHistoryUpdate.Description = medicalHistory.Description;

            context.Update(medicalHistoryUpdate);
            await context.SaveChangesAsync();

            return DbResult<MedicalHistory>.CreateSuccess("Medical history has been edited", medicalHistoryUpdate);
        }

        public async Task<DbResult<List<MedicalHistory>>> ShowMedicalHistories(Guid patientId)
        {
            if (await patientRepository.GetPatient(patientId) == null)
            {
                return DbResult<List<MedicalHistory>>.CreateFail("Patient does not exist");
            }

            var result = await context.MedicalHistories
                .Include(x => x.User.Role)
                .Where(x => x.PatientId == patientId).ToListAsync();

            return DbResult<List<MedicalHistory>>.CreateSuccess("Success", result);
        }

        private async Task<MedicalHistory> GetMedicalHistory(Guid medicalHistoryId) => await context.MedicalHistories.FirstOrDefaultAsync(x => x.Id == medicalHistoryId);
    }
}
