using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Models;
using System.Security.Cryptography;
using System.Text;

namespace SSC.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;

        public UserRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<DbResult<User>> AuthenticateUser(string email, string password)
        {
            var user = await GetUserEmail(email);
            if (user == null)
            {
                return DbResult<User>.CreateFail("User does not exist");
            }

            if (!user.IsActive)
            {
                return DbResult<User>.CreateFail("User account is not active");
            }

            HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);
            byte[] computedHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));
            if (!CompareHash(computedHash, user.PasswordHash))
            {
                return DbResult<User>.CreateFail("Password is not correct");
            }

            return DbResult<User>.CreateSuccess("Authentication success", user);
        }

        public async Task<DbResult<User>> AddUser(UserViewModel u)
        {
            if (await GetUserEmail(u.Email) != null)
            {
                return DbResult<User>.CreateFail("Email has already been used");
            }

            var user = new User();
            user.Name = u.Name;
            user.Surname = u.Surname;
            user.Email = u.Email;
            user.PhoneNumber = u.PhoneNumber;

            var password = CreatePassword(5);
            HMACSHA512 hmac = new HMACSHA512();
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));

            user.IsActive = true;
            user.Role = await context.Roles.FirstOrDefaultAsync(x => x.Name == u.Role);

            await context.AddAsync(user);
            await context.SaveChangesAsync();
            return DbResult<User>.CreateSuccess("User created with password " + password, user);
        }

        public async Task<List<User>> GetUsers()
        {
            return await context.Users.Include(x => x.Role).ToListAsync();
        }

        public async Task<DbResult<User>> DeactivateUser(string useremail, Guid id)
        {
            var user = await GetUserEmail(useremail);
            if (user == null)
            {
                return DbResult<User>.CreateFail("User does not exist");
            }
            if (user.Id == id)
            {
                return DbResult<User>.CreateFail("Cannot deactivate own account");
            }
            var test = await context.Roles.FirstOrDefaultAsync(x => x.Name == Roles.Admin);
            if (user.Role == test)
            {
                return DbResult<User>.CreateFail("Cannot deactivate admin's account");
            }
            user.IsActive = false;
            context.Update(user);
            await context.SaveChangesAsync();
            return DbResult<User>.CreateSuccess("User has been deactivated", user);
        }

        public async Task<DbResult<User>> ActivateUser(string useremail, Guid id)
        {
            var user = await GetUserEmail(useremail);
            if (user == null)
            {
                return DbResult<User>.CreateFail("User does not exist");
            }
            if (user.Id == id)
            {
                return DbResult<User>.CreateFail("Cannot activate own account");
            }
            var test = await context.Roles.FirstOrDefaultAsync(x => x.Name == Roles.Admin);
            if (user.Role == test)
            {
                return DbResult<User>.CreateFail("Cannot activate admin's account");
            }
            user.IsActive = true;
            context.Update(user);
            await context.SaveChangesAsync();
            return DbResult<User>.CreateSuccess("User has been activated", user);
        }

        public async Task<User> UserDetails(Guid userId)
        {
            return await context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<DbResult<User>> EditUser(UserEditViewModel user, Guid id)
        {
            var u = await context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
            if (u == null)
            {
                return DbResult<User>.CreateFail("User does not exist");
            }
            var test = await context.Roles.FirstOrDefaultAsync(x => x.Name == Roles.Admin);
            if (u.Role == test && user.Id != id)
            {
                return DbResult<User>.CreateFail("Cannot edit admin's account");
            }
            u.Name = user.Name;
            u.Surname = user.Surname;
            u.Email = user.Email;
            u.PhoneNumber = user.PhoneNumber;
            u.IsActive = user.IsActive;
            u.Role = await context.Roles.FirstOrDefaultAsync(x => x.Name == user.Role);
            context.Update(u);
            await context.SaveChangesAsync();
            return DbResult<User>.CreateSuccess("User has been edited", u);
        }

        private async Task<User> GetUserEmail(string useremail) => await context.Users.FirstOrDefaultAsync(x => x.Email == useremail);

        private string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        private static bool CompareHash(byte[] h1, byte[] h2)
        {
            return h1.Select((x, i) => h1[i] == h2[i]).All(x => x);
        }
    }
}
