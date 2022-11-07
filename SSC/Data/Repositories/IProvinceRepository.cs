using SSC.Data.Models;

namespace SSC.Data.Repositories
{
    public interface IProvinceRepository
    {
        Task<List<Province>> GetProvinces();
    }
}
