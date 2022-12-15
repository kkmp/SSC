using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Data.UnitOfWork;
using SSC.DTO.MailRequest;
using SSC.DTO.User;
using SSC.Tools;
using System.Security.Cryptography;

namespace SSC.Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly DataContext context;
        private readonly IUnitOfWork unitOfWork;

        public UserRepository(DataContext context, IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
        }

        public async Task<DbResult<User>> AuthenticateUser(string email, string password)
        {
            var user = await GetUserByEmail(email);

            var conditions = new Dictionary<Func<bool>, string>
            {
                { () => user == null, "Hasło nie jest poprawne" },
                { () => !user.IsActive, "Konto użytkownika nie jest aktywne" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);
            byte[] computedHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));

            conditions.Clear();
            conditions.Add(() => !CompareHash(computedHash, user.PasswordHash), "Hasło nie jest poprawne");

            result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            return DbResult<User>.CreateSuccess("Powodzenie uwierzytelniania", user);
        }

        public async Task<DbResult<User>> AddUser(UserCreateDTO user)
        {
            var conditions = new Dictionary<Func<bool>, string>
            {
                { () => GetUserByEmail(user.Email).Result != null, "Adres email został już wykorzystany" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var newUser = unitOfWork.Mapper.Map<User>(user);

            var password = PasswordGenerator.CreatePassword(5);
            HMACSHA512 hmac = new HMACSHA512();
            newUser.PasswordSalt = hmac.Key;
            newUser.PasswordHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));

            newUser.IsActive = true;
            newUser.Role = await unitOfWork.RoleRepository.GetRoleByName(user.RoleName);

            await context.AddAsync(newUser);
            await context.SaveChangesAsync();

            await unitOfWork.MailService.SendEmailAsync(new MailRequestDTO(user.Email, "Dostęp do nowego konta", "Tymczasowe hasło logowania do nowego konta: " + password));

            return DbResult<User>.CreateSuccess(password, newUser);
        }

        public async Task<List<User>> GetUsers()
        {
            return await context.Users.Include(x => x.Role).ToListAsync();
        }

        public async Task<DbResult<User>> ChangeActivity(Guid userId, Guid issuerId, bool activation)
        {
            var user = await GetUser(userId);

            var conditions = new Dictionary<Func<bool>, string>
            {
                { () => user == null, "Użytkownik nie istnieje" },
                { () => user.Id == issuerId, "Nie można dezaktywować własnego konta" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            user.IsActive = activation;

            context.Update(user);
            await context.SaveChangesAsync();

            return DbResult<User>.CreateSuccess("Aktywność konta użytkownika została zmieniona", user);
        }

        public async Task<DbResult<User>> UserDetails(Guid userId)
        {
            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
               { () => GetUser(userId).Result == null, "Użytkownik nie istnieje" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var data = await context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == userId);

            return DbResult<User>.CreateSuccess("Powodzenie", data);
        }

        public async Task<DbResult<User>> EditUser(UserUpdateDTO user, Guid issuerId)
        {
            var userToCheck = await GetUser(user.Id);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
               { () => userToCheck == null, "Użytkownik nie istnieje" },
               { () => user.Email != userToCheck.Email && GetUserByEmail(user.Email).Result != null, "Adres email został już wykorzystany" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            unitOfWork.Mapper.Map(user, userToCheck);

            userToCheck.Role = await unitOfWork.RoleRepository.GetRoleByName(user.RoleName);

            context.Update(userToCheck);
            await context.SaveChangesAsync();

            return DbResult<User>.CreateSuccess("Użytkownik został zedytowany", userToCheck);
        }

        public async Task<User> GetUserByEmail(string email) => await context.Users.FirstOrDefaultAsync(x => x.Email == email);

        public async Task<User> GetUser(Guid userId) => await context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        public bool CompareHash(byte[] h1, byte[] h2)
        {
            return h1.Select((x, i) => h1[i] == h2[i]).All(x => x);
        }
    }
}
