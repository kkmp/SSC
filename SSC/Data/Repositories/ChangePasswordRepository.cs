using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Data.UnitOfWork;
using SSC.Tools;
using System.Security.Cryptography;

namespace SSC.Data.Repositories
{
    public class ChangePasswordRepository : BaseRepository, IChangePasswordRepository
    {
        private readonly DataContext context;
        private readonly IUnitOfWork unitOfWork;

        public ChangePasswordRepository(DataContext context, IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
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

            return DbResult<ChangePasswordCode>.CreateSuccess("Powodzenie", code);
        }

        public async Task<DbResult> ChangeCode(string password, string code)
        {
            var codeToCheck = await context.ChangePasswordCodes.FirstOrDefaultAsync(x => x.Code == code); 

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
               { () => codeToCheck == null, "Kod nie istnieje" },
               { () => codeToCheck.ExpiredDate < DateTime.Now, "Kod stracił ważność" },
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
            var user = await unitOfWork.UserRepository.GetUser(issuerId);

            HMACSHA512 hmac = new HMACSHA512();
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));

            context.Update(user);
            await context.SaveChangesAsync();

            return DbResult.CreateSuccess("Hasło użytkownika zostało zmienione");
        }

        public async Task<DbResult> ChangePassword(string oldPassword, string newPassword, Guid issuerId)
        {
            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
               { () => !CheckPassword(oldPassword, issuerId).Result, "Hasło niepoprawne" },
               { () => oldPassword == newPassword, "Stare hasło nie może być takie samo jak nowe hasło" },
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
            var user = await unitOfWork.UserRepository.GetUser(issuerId);

            HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);
            byte[] computedHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));

            return unitOfWork.UserRepository.CompareHash(computedHash, user.PasswordHash);
        }
    }
}