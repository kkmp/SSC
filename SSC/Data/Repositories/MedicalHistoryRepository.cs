using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Data.UnitOfWork;
using SSC.DTO.MedicalHistory;

namespace SSC.Data.Repositories
{
    public class MedicalHistoryRepository : BaseRepository<MedicalHistory>, IMedicalHistoryRepository
    {
        private readonly DataContext context;
        private readonly IUnitOfWork unitOfWork;

        public MedicalHistoryRepository(DataContext context, IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
        }

        public async Task<DbResult<MedicalHistory>> AddMedicalHistory(MedicalHistoryCreateDTO medicalHistory, Guid issuerId)
        {
            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => unitOfWork.PatientRepository.GetPatient(medicalHistory.PatientId.Value).Result == null, "Pacjent nie istnieje" },
                { () => context.MedicalHistories.AnyAsync(x => x.PatientId == medicalHistory.PatientId && x.Date >= medicalHistory.Date).Result, "Nie można dodać historii choroby. Istnieje już wpis przed podaną datą" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var newMedicalHistory = unitOfWork.Mapper.Map<MedicalHistory>(medicalHistory);

            newMedicalHistory.UserId = issuerId;

            await context.AddAsync(newMedicalHistory);
            await context.SaveChangesAsync();

            return DbResult<MedicalHistory>.CreateSuccess("Dodano historię choroby", newMedicalHistory);
        }

        public async Task<DbResult<MedicalHistory>> EditMedicalHistory(MedicalHistoryUpdateDTO medicalHistory, Guid issuerId)
        {
            var medicalHistoryUpdate = await GetMedicalHistory(medicalHistory.Id);

            if (medicalHistoryUpdate == null)
            {
                return DbResult<MedicalHistory>.CreateFail("Nie ma takiego wpisu w historii choroby");
            }

            var newest = await context.MedicalHistories.Where(x => x.PatientId == medicalHistoryUpdate.PatientId).OrderByDescending(x => x.Date).FirstOrDefaultAsync();

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => medicalHistoryUpdate.UserId != issuerId, "Tylko użytkownik, który dodał wpis do historii choroby może go edytować" },
                { () => newest.Id != medicalHistoryUpdate.Id, "Tej historii choroby nie można już edytować" },
                { () => context.MedicalHistories.AnyAsync(x => x.PatientId == medicalHistoryUpdate.PatientId && x.Date >= medicalHistory.Date && x.Id != medicalHistory.Id).Result, "Nie można dodać historii choroby. Istnieje już wpis przed podaną datą" },
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

            return DbResult<MedicalHistory>.CreateSuccess("Historia choroby została zedytowana", medicalHistoryUpdate);
        }

        public async Task<DbResult<List<MedicalHistory>>> ShowMedicalHistories(Guid patientId)
        {
            if (await unitOfWork.PatientRepository.GetPatient(patientId) == null)
            {
                return DbResult<List<MedicalHistory>>.CreateFail("Pacjent nie istnieje");
            }

            var result = await context.MedicalHistories
                .Include(x => x.User.Role)
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.Date)
                .ToListAsync();

            return DbResult<List<MedicalHistory>>.CreateSuccess("Powodzenie", result);
        }

        public async Task<DbResult<MedicalHistory>> ShowMedicalHistoryDetails(Guid medicalHistoryId)
        {
            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => GetMedicalHistory(medicalHistoryId).Result == null, "Historia choroby nie istnieje" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var data = await context.MedicalHistories
                .Include(x => x.User.Role)
                .FirstOrDefaultAsync(x => x.Id == medicalHistoryId);

            return DbResult<MedicalHistory>.CreateSuccess("Powodzenie", data);
        }

        private async Task<MedicalHistory> GetMedicalHistory(Guid medicalHistoryId) => await context.MedicalHistories.FirstOrDefaultAsync(x => x.Id == medicalHistoryId);
    }
}
