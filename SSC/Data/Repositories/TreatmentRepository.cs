﻿using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Data.UnitOfWork;
using SSC.DTO.Treatment;

namespace SSC.Data.Repositories
{
    public class TreatmentRepository : BaseRepository<Treatment>, ITreatmentRepository
    {
        private readonly DataContext context;
        private readonly IUnitOfWork unitOfWork;

        public TreatmentRepository(DataContext context, IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
        }

        public async Task<DbResult<Treatment>> AddTreatment(TreatmentCreateDTO treatment, Guid issuerId)
        {
            var treatmentStatus = await unitOfWork.TreatmentStatusRepository.GetTreatmentStatus(treatment.TreatmentStatusId.Value);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => unitOfWork.PatientRepository.GetPatient(treatment.PatientId.Value).Result == null, "Pacjent nie istnieje" },
                { () => treatmentStatus == null, "Status leczenia nie istnieje" },
                { () => context.Treatments.AnyAsync(x => x.PatientId == treatment.PatientId && x.EndDate == null).Result, "Ostatnie leczenie nie zostało jeszcze zakończone"},
                { () => context.Treatments.AnyAsync(x => treatment.StartDate > x.StartDate && treatment.StartDate < x.EndDate && x.PatientId == treatment.PatientId).Result, "Niepoprawna data rozpoczęcia leczenia" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var newTreatment = unitOfWork.Mapper.Map<Treatment>(treatment);

            newTreatment.UserId = issuerId;

            await context.Treatments.AddAsync(newTreatment);
            await context.SaveChangesAsync();

            return DbResult<Treatment>.CreateSuccess("Leczenie zostało dodane", newTreatment);
        }

        public async Task<DbResult<Treatment>> EditTreatment(TreatmentUpdateDTO treatment, Guid issuerId)
        {
            var treatmentToCheck = await context.Treatments.FirstOrDefaultAsync(x => x.Id == treatment.Id);
            var treatmentStatus = await unitOfWork.TreatmentStatusRepository.GetTreatmentStatus(treatment.TreatmentStatusId.Value);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () =>  treatmentToCheck == null, "Leczenie nie istnieje"},
                { () =>  treatmentToCheck.UserId != issuerId, "Tylko użytkownik, który dodał leczenie może je edytować"},
                { () =>  treatmentStatus == null, "Status leczenia nie istnieje" },
                { () =>  treatmentToCheck.EndDate != null, "Tego leczenia nie można już edytować - leczenie zostało zakończone"},
                { () =>  treatment.StartDate > treatment.EndDate, "Data rozpoczęcia leczenia nie może być wcześniejsza niż data jego zakończenia"},
                { () =>  context.Treatments.AnyAsync(x => treatment.StartDate < x.EndDate && treatment.Id != x.Id && x.PatientId == treatmentToCheck.PatientId).Result, "Data rozpoczęcia leczenia nie może być starsza niż data zakończenia innego leczenia"},
                { () =>  context.Tests.AnyAsync(x => x.TreatmentId == treatment.Id && treatment.StartDate > x.TestDate).Result, "Data rozpoczęcia leczenia nie może następować po istniejącym teście" },
                { () =>  context.TreatmentDiseaseCourses.AnyAsync(x => x.TreatmentId == treatment.Id && treatment.StartDate > x.Date).Result, "Data rozpoczęcia leczenia nie może następować po istniejącym wpisie o powikłaniach" },
                { () =>  context.Tests.AnyAsync(x => x.TreatmentId == treatment.Id && x.ResultDate == null).Result, "Niektóre testy powiązane z leczeniem nie posiadają daty wyniku"}
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            unitOfWork.Mapper.Map(treatment, treatmentToCheck);

            context.Update(treatmentToCheck);
            await context.SaveChangesAsync();

            return DbResult<Treatment>.CreateSuccess("Leczenie zostało zedytowane", treatmentToCheck);
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

        public async Task<DbResult<Treatment>> ShowTreatmentDetails(Guid treatmentId)
        {
            if (await GetTreatment(treatmentId) == null)
            {
                return DbResult<Treatment>.CreateFail("Leczenie nie istnieje");
            }

            var result = await context.Treatments
                .Include(x => x.User.Role)
                .Include(x => x.TreatmentStatus)
                .FirstOrDefaultAsync(x => x.Id == treatmentId);

            return DbResult<Treatment>.CreateSuccess("Powodzenie", result);
        }

        public async Task<DbResult<List<Treatment>>> ShowTreatments(Guid patientId)
        {
            if (await unitOfWork.PatientRepository.GetPatient(patientId) == null)
            {
                return DbResult<List<Treatment>>.CreateFail("Pacjent nie istnieje");
            }

            var result = await context.Treatments
                .Include(x => x.TreatmentStatus)
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.StartDate)
                .ToListAsync();

            return DbResult<List<Treatment>>.CreateSuccess("Powodzenie", result);
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
