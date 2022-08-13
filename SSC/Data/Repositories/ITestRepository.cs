using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public interface ITestRepository
    {
        Task<List<Test>> GetTests(Guid provinceId, DateTime dateFrom, DateTime dateTo);
        Task<DbResult<Test>> AddTest(TestViewModel test, Guid id);
        Task<DbResult<Test>> EditTest(TestEditViewModel test, Guid id);
        Task<DbResult<List<Test>>> ShowTests(Guid patientId);
        Task<Test> TestDetails(Guid testId);
    }
}
