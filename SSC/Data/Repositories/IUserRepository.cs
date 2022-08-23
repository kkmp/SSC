using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public interface IUserRepository
    {
        Task<DbResult<User>> AuthenticateUser(string email, string password);
        Task<DbResult<User>> AddUser(UserViewModel user);
        Task<List<User>> GetUsers();
        Task<DbResult<User>> ChangeActivity(Guid userId, Guid issuerId, bool activation);
        Task<User> UserDetails(Guid userId);
        Task<DbResult<User>> EditUser(UserEditViewModel user, Guid issuerId);
        Task<User> GetUser(Guid userId);
    }
}
