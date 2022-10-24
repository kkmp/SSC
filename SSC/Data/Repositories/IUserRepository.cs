using SSC.Data.Models;
using SSC.DTO.User;

namespace SSC.Data.Repositories
{
    public interface IUserRepository
    {
        Task<DbResult<User>> AuthenticateUser(string email, string password);
        Task<DbResult<User>> AddUser(UserCreateDTO user);
        Task<List<User>> GetUsers();
        Task<DbResult<User>> ChangeActivity(Guid userId, Guid issuerId, bool activation);
        Task<DbResult<User>> UserDetails(Guid userId);
        Task<DbResult<User>> EditUser(UserUpdateDTO user, Guid issuerId);
        Task<User> GetUser(Guid userId);
        Task<User> GetUserByEmail(string email);
        bool CompareHash(byte[] h1, byte[] h2);
    }
}
