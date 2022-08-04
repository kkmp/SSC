using SSC.Data.Models;
using SSC.Models;

namespace SSC.Data.Repositories
{
    public interface IUserRepository
    {
        Task<DbResult<User>> AuthenticateUser(string email, string password);
        Task<DbResult<User>> AddUser(UserViewModel user);
        Task<List<User>> GetUsers();
        Task<DbResult<User>> DeactivateUser(string useremail, Guid id);
        Task<DbResult<User>> ActivateUser(string useremail, Guid id);
        Task<User> UserDetails(Guid userId);
        Task<DbResult<User>> EditUser(UserEditViewModel user, Guid id);

    }
}
