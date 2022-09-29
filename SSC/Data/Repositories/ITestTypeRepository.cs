using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface ITestTypeRepository
    {
        Task<TestType> GetTestTypeByName(string testTypeName);
        Task<TestType> GetTestType(Guid testTypeId);
        Task<List<TestType>> GetTestTypes();
    }
}
