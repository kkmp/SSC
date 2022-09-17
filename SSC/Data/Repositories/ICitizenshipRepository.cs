using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface ICitizenshipRepository
    {
        Task<List<Citizenship>> GetCitizenships();
        Task<Citizenship> GetCitizenship(Guid citizenshipId);
    }
}
