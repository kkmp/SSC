using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Tools;
using System.Security.Cryptography;

namespace SSC.Data.Repositories
{
    public class ChangePasswordRepository : BaseRepository, IChangePasswordRepository
    {
        private readonly DataContext context;
        private readonly IUserRepository userRepository;

        public ChangePasswordRepository(DataContext context, IUserRepository userRepository)
        {
            this.context = context;
            this.userRepository = userRepository;
        }

        public async Task<DbResult<ChangePasswordCode>> AddCode(Guid userId)
        {
            var code = new ChangePasswordCode
            {
                UserId = userId,
                Code = PasswordGenerator.CreatePassword(100),
                ExpiredDate = DateTime.Now.AddMinutes(60)
            };

            await context.AddAsync(code);
            await context.SaveChangesAsync();

            return DbResult<ChangePasswordCode>.CreateSuccess("Success", code);
        }

        public async Task<DbResult> ChangeCode(string password, string code)
        {
            var codeToCheck = await context.ChangePasswordCodes.FirstOrDefaultAsync(x => x.Code == code); 

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
               { () => codeToCheck == null, "Code does not exist" },
               { () => codeToCheck.ExpiredDate < DateTime.Now, "Code has been expired" },
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var call = await ChangePassword(password, codeToCheck.UserId.Value);
            if (call.Success)
            {
                context.Remove(codeToCheck);
                await context.SaveChangesAsync();
            }

            return call;
        }

        public async Task<DbResult> ChangePassword(string password, Guid issuerId)
        {
            var user = await userRepository.GetUser(issuerId);

            HMACSHA512 hmac = new HMACSHA512();
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));

            context.Update(user);
            await context.SaveChangesAsync();

            return DbResult.CreateSuccess("User password has been changed");
        }

        public async Task<DbResult> ChangePassword(string oldPassword, string newPassword, Guid issuerId)
        {
            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
               { () => oldPassword == newPassword, "The old password cannot be the same as the new password" },
               { () => !CheckPassword(oldPassword, issuerId).Result, "Password not correct" },
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            return await ChangePassword(newPassword, issuerId);
        }

        private async Task<bool> CheckPassword(string password, Guid issuerId)
        {
            var user = await userRepository.GetUser(issuerId);

            HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);
            byte[] computedHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));

            return userRepository.CompareHash(computedHash, user.PasswordHash);
        }
    }
}