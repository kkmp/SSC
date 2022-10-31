using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Data.UnitOfWork;
using SSC.DTO.Test;
using SSC.DTO.Treatment;

namespace SSC.Data.Repositories
{
    public class TestRepository : BaseRepository<Test>, ITestRepository
    {
        private readonly DataContext context;
        private readonly IUnitOfWork unitOfWork;

        public TestRepository(DataContext context, IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
        }

        public async Task<DbResult<Test>> AddTest(TestCreateDTO test, Guid issuerId)
        {
            var testType = await unitOfWork.TestTypeRepository.GetTestType(test.TestTypeId.Value);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
               { () => GetTestByOrderNumber(test.OrderNumber).Result != null, "Test został już dodany" },
               { () => unitOfWork.PatientRepository.GetPatient(test.PatientId.Value).Result == null, "Pacjent nie istnieje" },
               { () => testType == null, "Typ testu nie istnieje" },
               { () => !unitOfWork.PlaceRepository.AnyPlace(test.PlaceId.Value).Result, "Miejsce nie istnieje" },
               { () => test.TestDate > test.ResultDate, "Data wyniku testu nie może być wcześniejsza niż data testu" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var newTest = unitOfWork.Mapper.Map<Test>(test);

            newTest.UserId = issuerId;

            var treatment = await unitOfWork.TreatmentRepository.TreatmentLasts(test.PatientId.Value);

            if (treatment == null)
            {
                var newTreatment = new TreatmentCreateDTO
                {
                    StartDate = test.TestDate,
                    PatientId = test.PatientId.Value,
                    TreatmentStatusId = unitOfWork.TreatmentStatusRepository.GetTreatmentStatusByName(TreatmentStatusOptions.Started).Result.Id //status "Rozpoczęto"
                };
                var info = await unitOfWork.TreatmentRepository.AddTreatment(newTreatment, issuerId);
                treatment = info.Data;
            }

            conditions.Clear();
            conditions.Add(() => test.TestDate < treatment.StartDate, "Nie można dodać wpisu przed datą rozpoczęcia leczenia");
            conditions.Add(() => context.Tests.AnyAsync(x => test.TestDate < x.TestDate && x.TreatmentId == treatment.Id).Result, "Nie można dodać wpisu przed innym testem");

            result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            newTest.TreatmentId = treatment.Id;

            switch (newTest.Result)
            {
                case TestResultOptions.Positive:
                    treatment.IsCovid = true;
                    break;
                case TestResultOptions.Negative:
                    treatment.IsCovid = false;
                    break;
            }

            await context.AddAsync(newTest);
            await context.SaveChangesAsync();

            return DbResult<Test>.CreateSuccess("Test został dodany", newTest);
        }

        public async Task<List<Test>> GetTests(Guid provinceId, DateTime dateFrom, DateTime dateTo)
        {
            return await context.Tests
                .Include(x => x.TestType)
                .Include(x => x.Place)
                .ThenInclude(x => x.City)
                .Where(x => x.Place.City.ProvinceId == provinceId && x.TestDate >= dateFrom && x.TestDate <= dateTo)
                .ToListAsync();
        }

        public async Task<DbResult<Test>> EditTest(TestUpdateDTO test, Guid issuerId)
        {
            var testToCheck = await GetTest(test.Id);

            if (testToCheck == null)
            {
                return DbResult<Test>.CreateFail("Test nie istnieje");
            }

            var treatment = await unitOfWork.TreatmentRepository.GetTreatment(testToCheck.TreatmentId.Value);
            var newest = await context.Tests.OrderByDescending(x => x.TestDate).FirstOrDefaultAsync(x => x.TreatmentId == testToCheck.TreatmentId);
            var testType = await unitOfWork.TestTypeRepository.GetTestType(test.TestTypeId.Value);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => testToCheck.Id != newest.Id,  "Tego testu nie można już edytować" },
                { () => testToCheck.UserId != issuerId, "Tylko użytkownik, który dodał test może go edytować"},
                { () => treatment.EndDate != null, "Tego testu nie można już edytować - leczenie zostało zakończone"},
                { () => test.TestDate > test.ResultDate,  "Data wyniku testu nie może być wcześniejsza niż data testu"},
                { () => !unitOfWork.PlaceRepository.AnyPlace(test.PlaceId.Value).Result, "Miejsce nie istnieje" },
                { () => testType == null, "Typ testu nie istnieje"},
                { () => test.TestDate < treatment.StartDate, "Nie można dodać wpisu przed datą rozpoczęcia leczenia" },
                { () => context.Tests.AnyAsync(x => x.Id != testToCheck.Id && test.TestDate < x.TestDate && x.TreatmentId == testToCheck.TreatmentId).Result, "Nie można dodać wpisu przed innym testem" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            switch (test.Result)
            {
                case TestResultOptions.Inconclusive:
                    treatment.IsCovid = null;
                    break;
                case TestResultOptions.Positive:
                    treatment.IsCovid = true;
                    break;
                case TestResultOptions.Negative:
                    treatment.IsCovid = false;
                    break;
            }

            unitOfWork.Mapper.Map(test, testToCheck);

            context.Update(testToCheck);
            context.Update(treatment);
            await context.SaveChangesAsync();

            return DbResult<Test>.CreateSuccess("Test został zedytowany", testToCheck);
        }

        public async Task<DbResult<List<Test>>> ShowTests(Guid patientId)
        {
            if (await unitOfWork.PatientRepository.GetPatient(patientId) == null)
            {
                return DbResult<List<Test>>.CreateFail("Pacjent nie istnieje");
            }

            var result = await context.Tests
                .Include(x => x.Treatment)
                .Include(x => x.TestType)
                .Include(x => x.Place)
                .Where(x => x.Treatment.PatientId == patientId)
                .OrderByDescending(x => x.TestDate)
                .ToListAsync();

            return DbResult<List<Test>>.CreateSuccess("Powodzenie", result);
        }

        public async Task<DbResult<Test>> TestDetails(Guid testId)
        {
            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => GetTest(testId).Result == null, "Test nie istnieje" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var data = await context.Tests
                .Include(x => x.TestType)
                .Include(x => x.Treatment)
                .Include(x => x.Place)
                .Include(x => x.User.Role)
                .FirstOrDefaultAsync(x => x.Id == testId);

            return DbResult<Test>.CreateSuccess("Powodzenie", data);
        }

        private async Task<Test> GetTestByOrderNumber(string orderNumber) => await context.Tests.FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);

        private async Task<Test> GetTest(Guid testId) => await context.Tests.FirstOrDefaultAsync(x => x.Id == testId);
    }
}
