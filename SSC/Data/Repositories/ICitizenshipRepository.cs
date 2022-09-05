using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface ICitizenshipRepository
    {
        Task<Citizenship> GetCitizenshipByName(string citizenshipName);
    }
}
