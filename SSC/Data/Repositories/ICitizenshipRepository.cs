using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface ICitizenshipRepository
    {
        Task<bool> AnyCitizenship(string citizenshipName);
        Task<Citizenship> GetCitizenshipByName(string citizenshipName);
    }
}
