using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface IChangePasswordRepository
    {
        Task<DbResult<ChangePasswordCode>> AddCode(Guid userId);
        Task<DbResult> ChangePassword(string password, Guid issuerId);
        Task<DbResult> ChangePassword(string oldPassword, string newPassword, Guid issuerId);
        Task<DbResult> ChangeCode(string password, string code);
    }
}
