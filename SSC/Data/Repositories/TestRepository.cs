using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly DataContext context;
        private readonly ITreatmentRepository treatmentRepository;

        public TestRepository(DataContext context, ITreatmentRepository treatmentRepository)
        {
            this.context = context;
            this.treatmentRepository = treatmentRepository;
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

        private async Task<Test> GetTest(string orderNumber) => await context.Tests.FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);
        private async Task<Patient> GetPatient(Guid patientId) => await context.Patients.FirstOrDefaultAsync(x => x.Id == patientId);

    }
}
