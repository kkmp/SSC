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

        public async Task<DbResult<MedicalHistory>> AddMedicalHistory(MedicalHistoryViewModel medicalHistory, Guid userId)
        {
            if (await context.MedicalHistories.AnyAsync(x => x.Date >= medicalHistory.Date))
            {
                return DbResult<MedicalHistory>.CreateFail("Cannot add medical history. There is already an entry before the given date");
            }

            var newMedicalHistory = mapper.Map<MedicalHistory>(medicalHistory);
            newMedicalHistory.UserId = userId;

            await context.AddAsync(newMedicalHistory);
            await context.SaveChangesAsync();

            return DbResult<MedicalHistory>.CreateSuccess("Medical history added", newMedicalHistory);
        }

        public async Task<DbResult<MedicalHistory>> EditMedicalHistory(EditMedicalHistoryViewModel medicalHistory, Guid userId)
        {
            var medicalHistoryUpdate = await context.MedicalHistories.FirstOrDefaultAsync(x => x.Id == medicalHistory.Id);

            if (medicalHistoryUpdate == null)
            {
                return DbResult<MedicalHistory>.CreateFail("There is no such entry in the medical history");
            }

            if (medicalHistoryUpdate.UserId != userId)
            {
                return DbResult<MedicalHistory>.CreateFail("Only the user who added the medical history entry can edit");
            }

            if (await context.MedicalHistories.AnyAsync(x => x.Date < medicalHistoryUpdate.Date))
            {
                return DbResult<MedicalHistory>.CreateFail("The medical history cannot be edited anymore");
            }

            //tutaj poprawić
            var lastAvailableDate = await context.MedicalHistories.Where(x => x.Date < medicalHistoryUpdate.Date && x.Date != medicalHistoryUpdate.Date).MaxAsync(x => x.Date);
            if (lastAvailableDate < medicalHistory.Date)
            {
                return DbResult<MedicalHistory>.CreateFail("Cannot edit medical history. There is already an entry before the given date");
            }

            medicalHistoryUpdate.Date = medicalHistory.Date;
            medicalHistoryUpdate.Description = medicalHistory.Description;

            context.Update(medicalHistoryUpdate);
            await context.SaveChangesAsync();
            return DbResult<MedicalHistory>.CreateSuccess("Medical history has been edited", medicalHistoryUpdate);
        }
    }
}
