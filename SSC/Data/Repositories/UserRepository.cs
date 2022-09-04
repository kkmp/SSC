using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
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
                { () => user == null, "User does not exist" },
                { () => !user.IsActive, "User account is not active" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);
            byte[] computedHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));

            conditions.Clear();
            conditions.Add(() => !CompareHash(computedHash, user.PasswordHash), "Password is not correct");

            result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            return DbResult<User>.CreateSuccess("Authentication success", user);
        }

        public async Task<DbResult<User>> AddUser(UserCreateDTO user)
        {
            var conditions = new Dictionary<Func<bool>, string>
            {
                { () => GetUserByEmail(user.Email).Result != null, "Email has already been used" }
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

            return DbResult<User>.CreateSuccess("User created with password " + password, newUser);
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
                { () => user == null, "User does not exist" },
                { () => user.Id == issuerId, "Cannot deactivate own account" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            user.IsActive = activation;

            context.Update(user);
            await context.SaveChangesAsync();

            return DbResult<User>.CreateSuccess("User activity has been changed", user);
        }

        public async Task<DbResult<User>> UserDetails(Guid userId)
        {
            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
               { () => GetUser(userId).Result == null, "User does not exist" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var data = await context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == userId);

            return DbResult<User>.CreateSuccess("Success", data);
        }

        public async Task<DbResult<User>> EditUser(UserUpdateDTO user, Guid issuerId)
        {
            var userToCheck = await GetUser(user.Id);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
               { () => userToCheck == null, "User does not exist" }
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

            return DbResult<User>.CreateSuccess("User has been edited", userToCheck);
        }

        public async Task<User> GetUserByEmail(string email) => await context.Users.FirstOrDefaultAsync(x => x.Email == email);

        public async Task<User> GetUser(Guid userId) => await context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        private static bool CompareHash(byte[] h1, byte[] h2)
        {
            return h1.Select((x, i) => h1[i] == h2[i]).All(x => x);
        }
    }
}
