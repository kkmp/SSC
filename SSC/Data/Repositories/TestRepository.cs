using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public class TestRepository : BaseRepository<Test>, ITestRepository
    {
        private readonly DataContext context;
        private readonly ITreatmentRepository treatmentRepository;
        private readonly IPatientRepository patientRepository;
        private readonly IMapper mapper;

        public TestRepository(DataContext context, ITreatmentRepository treatmentRepository, IPatientRepository patientRepository, IMapper mapper)
        {
            this.context = context;
            this.treatmentRepository = treatmentRepository;
            this.patientRepository = patientRepository;
            this.mapper = mapper;
        }

        public async Task<DbResult<Test>> AddTest(TestViewModel test, Guid issuerId)
        {
            //sprawdzenie czy poszczególne wartości istnieją
            //!!!
            if (await GetTestByOrderNumber(test.OrderNumber) != null)
            {
                return DbResult<Test>.CreateFail("Test has already been added");
            }

            //!!!
            if (await patientRepository.GetPatient(test.PatientId.Value) == null)
            {
                return DbResult<Test>.CreateFail("Patient does not exist");
            }

            /*
            var newTest = new Test
            {
                TestDate = test.TestDate,
                OrderNumber = test.OrderNumber,
                ResultDate = test.ResultDate,
                Result = test.Result,
                TestType = await context.TestTypes.FirstOrDefaultAsync(x => x.Name == test.TestType),
                UserId = issuerId,
                PlaceId = test.Place
            };
            */
            var newTest = mapper.Map<Test>(test);

            newTest.TestType = await context.TestTypes.FirstOrDefaultAsync(x => x.Name == test.TestTypeName);
            newTest.UserId = issuerId;

            var treatment = await treatmentRepository.TreatmentLasts(test.PatientId.Value);
           
            if (treatment == null)
            {
                var newTreatment = new TreatmentViewModel
                {
                    StartDate = DateTime.Now,
                    PatientId = test.PatientId.Value,
                    TreatmentStatusName = "Rozpoczęto" //status z seedera
                };
                var info = await treatmentRepository.AddTreatment(newTreatment, issuerId);
                treatment = info.Data;
            }

            //!!!
            if(test.TestDate < treatment.StartDate)
            {
                return DbResult<Test>.CreateFail("Test date cannot be older than treatment start date");
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

        public async Task<DbResult<Test>> EditTest(TestEditViewModel test, Guid issuerId)
        {
            var testToCheck = await GetTestByOrderNumber(test.OrderNumber);
            var treatment = await treatmentRepository.GetTreatment(testToCheck.TreatmentId.Value);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => testToCheck == null, "Test does not exist"},
                { () => testToCheck.UserId != issuerId, "Only the user who added the test can edit"},
                { () => treatment.EndDate != null, "The test cannot be edited anymore - the treatment has been ended"},
                { () => test.TestDate < treatment.StartDate, "Test date cannot be older than the treatment start date" },
                { () => test.TestDate > test.ResultDate,  "The test result date cannot be earlier than the test date"},
                { () => !context.Places.AnyAsync(x => x.Id == test.PlaceId).Result, "Place does not exist" },
                { () => test.TestDate < treatment.StartDate || test.ResultDate < treatment.StartDate, "Cannot add entry before treatment start date" },
                { () => context.Tests.Any(x => x.Id != testToCheck.Id && test.TestDate < x.TestDate && x.TreatmentId == testToCheck.TreatmentId) , "Cannot add entry before another test" }
            };

            var result = Validate(conditions);
            if(result != null)
            {
                return result;
            }

            mapper.Map(test, testToCheck);
            testToCheck.TestType = await context.TestTypes.FirstOrDefaultAsync(x => x.Name == test.TestTypeName);

            context.Update(testToCheck);
            await context.SaveChangesAsync();
            return DbResult<Test>.CreateSuccess("Test has been edited", testToCheck);
        }

        public async Task<DbResult<List<Test>>> ShowTests(Guid patientId)
        {
            if (await patientRepository.GetPatient(patientId) == null)
            {
                return DbResult<List<Test>>.CreateFail("Patient does not exist");
            }

            var result = await context.Tests
                .Include(x => x.Treatment)
                .Include(x => x.TestType)
                .Include(x => x.Place)
                .Where(x => x.Treatment.PatientId == patientId)
                .ToListAsync();

            return DbResult<List<Test>>.CreateSuccess("Success", result);
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

        private async Task<Test> GetTestByOrderNumber(string orderNumber) => await context.Tests.FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);
    }
}
