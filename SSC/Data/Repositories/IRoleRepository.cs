using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface IRoleRepository
    {
        Task<DbResult<Role>> GetRole(Guid roleId);
        Task<Role> GetRoleByName(string roleName);
    }
}
