using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public class TestTypeRepository : ITestTypeRepository
    {
        private readonly DataContext context;

        public TestTypeRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<TestType> GetTestTypeByName(string testTypeName)
        {
            return await context.TestTypes.FirstOrDefaultAsync(x => x.Name == testTypeName);
        }
    }
}
