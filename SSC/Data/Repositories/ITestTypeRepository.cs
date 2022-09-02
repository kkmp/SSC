using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface ITestTypeRepository
    {
        Task<bool> AnyTestType(string testTypeName);
        Task<TestType> GetTestTypeByName(string testTypeName);
    }
}
