using SSC.Data.Models;
using SSC.DTO.Test;

namespace SSC.Data.Repositories
{
    public interface ITestRepository
    {
        Task<List<Test>> GetTests(Guid provinceId, DateTime dateFrom, DateTime dateTo);
        Task<DbResult<Test>> AddTest(TestCreateDTO test, Guid issuerId);
        Task<DbResult<Test>> EditTest(TestUpdateDTO test, Guid issuerId);
        Task<DbResult<List<Test>>> ShowTests(Guid patientId);
        Task<DbResult<Test>> TestDetails(Guid testId);
    }
}
