﻿using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Data.UnitOfWork;
using SSC.DTO.Treatment;
using SSC.DTO.TreatmentDiseaseCourse;

namespace SSC.Data.Repositories
{
    public class TreatmentDiseaseCourseRepository : BaseRepository<TreatmentDiseaseCourse>, ITreatmentDiseaseCourseRepository
    {
        private readonly DataContext context;
        private readonly IUnitOfWork unitOfWork;

        public TreatmentDiseaseCourseRepository(DataContext context, IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
        }

        public async Task<List<TreatmentDiseaseCourse>> GetTreatmentDiseaseCourses(Guid provinceId, DateTime dateFrom, DateTime dateTo)
        {
            return await context.TreatmentDiseaseCourses
            .Include(x => x.DiseaseCourse)
            .Include(x => x.Treatment)
            .ThenInclude(x => x.Patient.City)
            .Where(x => x.Treatment.Patient.City.ProvinceId == provinceId && x.Date >= dateFrom && x.Date <= dateTo)
           .ToListAsync();
        }

        public async Task<DbResult<TreatmentDiseaseCourse>> AddTreatmentDiseaseCourse(TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourse, Guid issuerId)
        {
            var diseaseCourse = await unitOfWork.DiseaseCourseRepository.GetDiseaseCourse(treatmentDiseaseCourse.DiseaseCourseId.Value);

            var conditions = new Dictionary<Func<bool>, string>
            {
                { () => unitOfWork.PatientRepository.GetPatient(treatmentDiseaseCourse.PatientId.Value).Result == null, "Pacjent nie istnieje" },
                { () => diseaseCourse == null, "Powikłanie nie istnieje" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var treatment = await unitOfWork.TreatmentRepository.TreatmentLasts(treatmentDiseaseCourse.PatientId.Value);
            if (treatment == null)
            {
                var newTreatment = new TreatmentCreateDTO
                {
                    StartDate = treatmentDiseaseCourse.Date,
                    PatientId = treatmentDiseaseCourse.PatientId.Value,
                    TreatmentStatusId = unitOfWork.TreatmentStatusRepository.GetTreatmentStatusByName(TreatmentStatusOptions.Started).Result.Id //status "Rozpoczęto"
                };
                var info = await unitOfWork.TreatmentRepository.AddTreatment(newTreatment, issuerId);
                treatment = info.Data;
            }

            conditions.Clear();
            conditions.Add(() => treatmentDiseaseCourse.Date < treatment.StartDate, "Nie można dodać wpisu przed datą rozpoczęcia leczenia");
            conditions.Add(() => context.TreatmentDiseaseCourses.AnyAsync(x => treatmentDiseaseCourse.Date < x.Date && x.TreatmentId == treatment.Id).Result, "Nie można dodać wpisu przed innym powikłaniem");

            result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var newTreatmentDiseaseCourse = unitOfWork.Mapper.Map<TreatmentDiseaseCourse>(treatmentDiseaseCourse);

            newTreatmentDiseaseCourse.TreatmentId = treatment.Id;
            newTreatmentDiseaseCourse.UserId = issuerId;

            await context.TreatmentDiseaseCourses.AddAsync(newTreatmentDiseaseCourse);
            await context.SaveChangesAsync();

            return DbResult<TreatmentDiseaseCourse>.CreateSuccess("Powikłanie zostało dodane", newTreatmentDiseaseCourse);
        }

        public async Task<DbResult<List<TreatmentDiseaseCourse>>> ShowTreatmentDiseaseCourses(Guid patientId)
        {
            if (await unitOfWork.PatientRepository.GetPatient(patientId) == null)
            {
                return DbResult<List<TreatmentDiseaseCourse>>.CreateFail("Pacjent nie istnieje");
            }

            var data =  await context.TreatmentDiseaseCourses
                .Include(x => x.DiseaseCourse)
                .Where(x => x.Treatment.PatientId == patientId)
                .OrderByDescending(x => x.Date)
                .ToListAsync();

            return DbResult<List<TreatmentDiseaseCourse>>.CreateSuccess("Powodzenie", data);
        }

        public async Task<DbResult<TreatmentDiseaseCourse>> EditTreatmentDiseaseCourse(TreatmentDiseaseCourseUpdateDTO treatmentDiseaseCourse, Guid issuerId)
        {
            var checkTreatmentDiseaseCourse = await GetTreatmentDiseaseCourse(treatmentDiseaseCourse.Id);

            if (checkTreatmentDiseaseCourse == null)
            {
                return DbResult<TreatmentDiseaseCourse>.CreateFail("Powikłanie nie istnieje");
            }

            var treatment = await unitOfWork.TreatmentRepository.GetTreatment(checkTreatmentDiseaseCourse.TreatmentId.Value);
            var diseaseCourse = await unitOfWork.DiseaseCourseRepository.GetDiseaseCourse(treatmentDiseaseCourse.DiseaseCourseId);
            var newest = await context.TreatmentDiseaseCourses.OrderByDescending(x => x.Date).FirstOrDefaultAsync(x => x.TreatmentId == checkTreatmentDiseaseCourse.TreatmentId);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => checkTreatmentDiseaseCourse.UserId != issuerId, "Tylko użytkownik, który dodał powikłanie może je edytować"},
                { () => treatment.EndDate != null, "Tego powikłania nie można już edytować - leczenie zostało zakończone"},
                { () => checkTreatmentDiseaseCourse.Id != newest.Id,  "Nie można edytować wpisu przed innym powikłaniem" },
                { () => treatmentDiseaseCourse.Date < treatment.StartDate, "Nie można dodać wpisu przed datą rozpoczęcia leczenia"},
                { () => context.TreatmentDiseaseCourses.AnyAsync(x => x.Id != treatmentDiseaseCourse.Id && treatmentDiseaseCourse.Date < x.Date && x.TreatmentId == checkTreatmentDiseaseCourse.TreatmentId).Result, "Nie można dodać wpisu przed innym powikłaniem" },
                { () => diseaseCourse == null, "Powikłanie nie znalezione" },
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            unitOfWork.Mapper.Map(treatmentDiseaseCourse, checkTreatmentDiseaseCourse);

            checkTreatmentDiseaseCourse.DiseaseCourse = diseaseCourse;

            context.Update(checkTreatmentDiseaseCourse);
            await context.SaveChangesAsync();

            return DbResult<TreatmentDiseaseCourse>.CreateSuccess("Powikłanie zostało zedytowane", checkTreatmentDiseaseCourse);
        }

        private async Task<TreatmentDiseaseCourse> GetTreatmentDiseaseCourse(Guid treatmentDiseaseCourseId) => await context.TreatmentDiseaseCourses.FirstOrDefaultAsync(x => x.Id == treatmentDiseaseCourseId);

        public async Task<DbResult<TreatmentDiseaseCourse>> ShowTreatmentDiseaseCourseDetails(Guid treatmentDiseaseCourseId)
        {
            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => GetTreatmentDiseaseCourse(treatmentDiseaseCourseId).Result == null, "Powikłanie nie istnieje" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var data = await context.TreatmentDiseaseCourses
                .Include(x => x.DiseaseCourse)
                .Include(x => x.Treatment)
                .Include(x => x.User.Role)
                .FirstOrDefaultAsync(x => x.Id == treatmentDiseaseCourseId);

            return DbResult<TreatmentDiseaseCourse>.CreateSuccess("Powodzenie", data);
        }
    }
}
