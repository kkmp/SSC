﻿using AutoMapper;
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
            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
               { () => GetTestByOrderNumber(test.OrderNumber).Result != null, "Test has already been added" },
               { () => patientRepository.GetPatient(test.PatientId.Value).Result == null, "Patient does not exist" },
               { () => !context.TestTypes.Any(x => x.Name == test.TestTypeName), "Test type does not exist" },
               { () => !context.Places.Any(x => x.Id == test.PlaceId), "Place does not exist" },
               { () => test.TestDate > test.ResultDate, "Test result date cannot be earlier than the test date" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

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

            conditions.Clear();
            conditions.Add(() => test.TestDate < treatment.StartDate, "Cannot add entry before treatment start date");
            conditions.Add(() => context.Tests.Any(x => test.TestDate <= x.TestDate && x.TreatmentId == treatment.Id), "Cannot add entry before another test");

            result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            newTest.TreatmentId = treatment.Id;

            switch (newTest.Result)
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

            //możliwość edycji tylko najnowszego
            //brak możliwości dodania kilku z tą samą datą

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => testToCheck == null, "Test does not exist"},
                { () => testToCheck.UserId != issuerId, "Only the user who added the test can edit"},
                { () => treatment.EndDate != null, "The test cannot be edited anymore - the treatment has been ended"},
                { () => test.TestDate > test.ResultDate,  "Test result date cannot be earlier than the test date"},
                { () => !context.Places.AnyAsync(x => x.Id == test.PlaceId).Result, "Place does not exist" },
                { () => test.TestDate < treatment.StartDate, "Cannot add entry before treatment start date" },
                { () => context.Tests.Any(x => x.Id != testToCheck.Id && test.TestDate < x.TestDate && x.TreatmentId == testToCheck.TreatmentId) , "Cannot add entry before another test" }
            };

            var result = Validate(conditions);
            if (result != null)
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

        public async Task<DbResult<Test>> TestDetails(Guid testId)
        {
            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => GetTest(testId).Result == null, "Test does not exist" }
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

            return DbResult<Test>.CreateSuccess("Success", data);
        }

        private async Task<Test> GetTestByOrderNumber(string orderNumber) => await context.Tests.FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);

        private async Task<Test> GetTest(Guid testId) => await context.Tests.FirstOrDefaultAsync(x => x.Id == testId);
    }
}
