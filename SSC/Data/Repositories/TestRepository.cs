using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly DataContext context;
        private readonly ITreatmentRepository treatmentRepository;
        private readonly IMapper mapper;

        public TestRepository(DataContext context, ITreatmentRepository treatmentRepository, IMapper mapper)
        {
            this.context = context;
            this.treatmentRepository = treatmentRepository;
            this.mapper = mapper;
        }

        public async Task<DbResult<Test>> AddTest(TestViewModel test, Guid id)
        {
            if (await GetTest(test.OrderNumber) != null)
            {
                return DbResult<Test>.CreateFail("Test has already been added");
            }

            if (await GetPatient(test.PatientId.Value) == null)
            {
                return DbResult<Test>.CreateFail("Patient does not exist");
            }

            var newTest = new Test
            {
                TestDate = test.TestDate,
                OrderNumber = test.OrderNumber,
                ResultDate = test.ResultDate,
                Result = test.Result,
                TestType = await context.TestTypes.FirstOrDefaultAsync(x => x.Name == test.TestType),
                User = await context.Users.FirstOrDefaultAsync(x => x.Id == id),
                Place = await context.Places.FirstOrDefaultAsync(x => x.Id == test.Place),
            };

            var treatment = await treatmentRepository.TreatmentLasts(test.PatientId.Value);
           
            if (treatment == null)
            {
                var newTreatment = new TreatmentViewModel
                {
                    StartDate = DateTime.Now,
                    PatientId = test.PatientId.Value,
                    TreatmentStatusName = "Rozpoczęto"
                };
                var info = await treatmentRepository.AddTreatment(newTreatment, id);
                treatment = info.Data;
            }
            newTest.TreatmentId = treatment.Id;
            switch(newTest.Result)
            {
                case 'P':
                    treatment.IsCovid = true;
                    break;
                case 'N':
                    treatment.IsCovid = false;
                    break;
            }
            await context.AddAsync(newTest);
            await context.SaveChangesAsync();
            return DbResult<Test>.CreateSuccess("Test added", newTest);
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

        private async Task<Test> GetTest(string orderNumber) => await context.Tests.AsNoTracking().FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);
        private async Task<Patient> GetPatient(Guid patientId) => await context.Patients.FirstOrDefaultAsync(x => x.Id == patientId);

        public async Task<DbResult<Test>> EditTest(TestEditViewModel test, Guid id) //dużo tych warunków, może jakoś ładniej? + no tracking
        {
            var testToCheck = await GetTest(test.OrderNumber);

            if (testToCheck == null)
            {
                return DbResult<Test>.CreateFail("Test does not exist");
            }

            if (testToCheck.UserId != id)
            {
                return DbResult<Test>.CreateFail("Only the user who added the test can edit");
            }

            var treatment = await context.Treatments.FirstOrDefaultAsync(x => x.Id == testToCheck.TreatmentId);
            if (treatment.EndDate != null)
            {
                return DbResult<Test>.CreateFail("The test cannot be edited anymore - the treatment has been ended");
            }

            if (test.TestDate < testToCheck.TestDate || test.ResultDate < testToCheck.TestDate)
            {
                return DbResult<Test>.CreateFail("The test date and result date cannot be earlier than the treatment start date");
            }

            if (test.TestDate > test.ResultDate)
            {
                return DbResult<Test>.CreateFail("The test result date cannot be earlier than the test date");
            }

            var newTest = mapper.Map<Test>(test);
            newTest.TestType = await context.TestTypes.FirstOrDefaultAsync(x => x.Name == test.TestTypeName);
            newTest.TreatmentId = testToCheck.TreatmentId;
            newTest.UserId = testToCheck.UserId;

            context.Update(newTest);
            await context.SaveChangesAsync();
            return DbResult<Test>.CreateSuccess("Test has been edited", newTest);
        }

        public async Task<List<Test>> ShowTests(Guid patientId)
        {
            if (await GetPatient(patientId) == null)
            {
                return null; //co zrobić gdy pacjent nie istnieje?
            }

            return await context.Tests
                .Include(x => x.Treatment)
                .Include(x => x.TestType)
                .Include(x => x.Place)
                .Where(x => x.Treatment.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<Test> TestDetails(Guid testId)
        {
            return await context.Tests
                .Include(x => x.TestType)
                .Include(x => x.Treatment)
                .Include(x => x.Place)
                .Include(x => x.User.Role)
                .FirstOrDefaultAsync(x => x.Id == testId);
        }
    }
}
