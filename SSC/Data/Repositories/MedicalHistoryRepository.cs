using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public class MedicalHistoryRepository : IMedicalHistoryRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public MedicalHistoryRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<DbResult<MedicalHistory>> AddMedicalHistory(MedicalHistoryViewModel medicalHistory, Guid issuerId)
        {
            if (await context.MedicalHistories.AnyAsync(x => x.Date >= medicalHistory.Date))
            {
                return DbResult<MedicalHistory>.CreateFail("Cannot add medical history. There is already an entry before the given date");
            }

            var newMedicalHistory = mapper.Map<MedicalHistory>(medicalHistory);
            newMedicalHistory.UserId = issuerId;

            await context.AddAsync(newMedicalHistory);
            await context.SaveChangesAsync();

            return DbResult<MedicalHistory>.CreateSuccess("Medical history added", newMedicalHistory);
        }

        public async Task<DbResult<MedicalHistory>> EditMedicalHistory(MedicalHistoryEditViewModel medicalHistory, Guid issuerId)
        {
            var medicalHistoryUpdate = await context.MedicalHistories.FirstOrDefaultAsync(x => x.Id == medicalHistory.Id);

            if (medicalHistoryUpdate == null)
            {
                return DbResult<MedicalHistory>.CreateFail("There is no such entry in the medical history");
            }

            if (medicalHistoryUpdate.UserId != issuerId)
            {
                return DbResult<MedicalHistory>.CreateFail("Only the user who added the medical history entry can edit");
            }

            var newest = await context.MedicalHistories.Where(x => x.PatientId == medicalHistoryUpdate.PatientId).OrderByDescending(x => x.Date).FirstOrDefaultAsync();
            if(newest != null && newest.Id != medicalHistoryUpdate.Id)
            {
                return DbResult<MedicalHistory>.CreateFail("The medical history cannot be edited anymore");
            }

            if (await context.MedicalHistories.AnyAsync(x => x.PatientId == medicalHistoryUpdate.PatientId && x.Date >= medicalHistory.Date && x.Id != medicalHistory.Id))
            {
                return DbResult<MedicalHistory>.CreateFail("Cannot edit medical history. There is already an entry before the given date");
            }

            medicalHistoryUpdate.Date = medicalHistory.Date;
            medicalHistoryUpdate.Description = medicalHistory.Description;

            context.Update(medicalHistoryUpdate);
            await context.SaveChangesAsync();
            return DbResult<MedicalHistory>.CreateSuccess("Medical history has been edited", medicalHistoryUpdate);
        }

        public async Task<List<MedicalHistory>> ShowMedicalHistories(Guid patientId)
        {
            return await context.MedicalHistories
                .Include(x => x.User.Role)
                .Where(x => x.PatientId == patientId).ToListAsync();
        }
    }
}
