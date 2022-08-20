using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface IRoleRepository
    {
        Task<Role> GetRole(Guid roleId);
    }
}
