using SSC.Data.Models;
using SSC.Tools;
using System.Security.Cryptography;

namespace SSC.Data.Repositories
{
    public class ChangePasswordRepository : IChangePasswordRepository
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

        public async Task<DbResult<User>> ChangePassword(string password, Guid issuerId)
        {
            var user = await userRepository.GetUser(issuerId);

            HMACSHA512 hmac = new HMACSHA512();
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));

            context.Update(user);
            await context.SaveChangesAsync();

            return DbResult<User>.CreateSuccess("User password has been changed", user);
        }
    }
}