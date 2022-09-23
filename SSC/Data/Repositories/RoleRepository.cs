using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        private readonly DataContext context;

        public RoleRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<DbResult<Role>> GetRole(Guid roleId)
        {
            var data = await context.Roles.FirstOrDefaultAsync(x => x.Id == roleId);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => data == null, "Rola nie istnieje" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            return DbResult<Role>.CreateSuccess("Powodzenie", data);
        }

        public async Task<Role> GetRoleByName(string roleName)
        {
            return await context.Roles.FirstOrDefaultAsync(x => x.Name == roleName);
        }
    }
}
