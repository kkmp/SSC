using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DataContext context;

        public RoleRepository(DataContext context)
        {
            this.context = context;
        }
        public async Task<Role> GetRole(Guid roleId)
        {
            return await context.Roles.FirstOrDefaultAsync(x => x.Id == roleId);
        }
    }
}
