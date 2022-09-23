using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.DTO.User;
using SSC.Models;
using SSC.Services;
using SSC.Tools;
using System.Security.Cryptography;
using System.Text;

namespace SSC.Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly DataContext context;
        private readonly IRoleRepository roleRepository;
        private readonly IMailService mailService;
        private readonly IMapper mapper;

        public UserRepository(DataContext context, IRoleRepository roleRepository, IMailService mailService, IMapper mapper)
        {
            this.context = context;
            this.roleRepository = roleRepository;
            this.mailService = mailService;
            this.mapper = mapper;
        }

        public async Task<DbResult<User>> AuthenticateUser(string email, string password)
        {
            var user = await GetUserByEmail(email);

            var conditions = new Dictionary<Func<bool>, string>
            {
                { () => user == null, "Użytkownik nie istnieje" },
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

            var newUser = mapper.Map<User>(user);

            var password = PasswordGenerator.CreatePassword(5);
            HMACSHA512 hmac = new HMACSHA512();
            newUser.PasswordSalt = hmac.Key;
            newUser.PasswordHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));

            newUser.IsActive = true;
            newUser.Role = await roleRepository.GetRoleByName(user.RoleName);

            await context.AddAsync(newUser);
            await context.SaveChangesAsync();

            await mailService.SendEmailAsync(new MailRequest(user.Email, "Dostęp do nowego konta", "Tymczasowe hasło logowania do nowego konta: " + password));

            return DbResult<User>.CreateSuccess("Utworzono użytkownika z następującym hasłem: " + password, newUser);
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
               { () => userToCheck == null, "Użytkownik nie istnieje" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            mapper.Map(user, userToCheck);

            userToCheck.Role = await roleRepository.GetRoleByName(Roles.Admin);

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
