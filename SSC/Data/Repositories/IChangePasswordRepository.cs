using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface IChangePasswordRepository
    {
        Task<DbResult<ChangePasswordCode>> AddCode(Guid userId);
        Task<DbResult<User>> ChangePassword(string password, Guid issuerId);
    }
}
