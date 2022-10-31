using SSC.Data.Models;
using SSC.DTO.Place;

namespace SSC.Data.Repositories
{
    public interface IPlaceRepository
    {
        Task<bool> AnyPlace(Guid placeId);
        Task<DbResult<Place>> AddPlace(PlaceCreateDTO place);
        Task<List<Place>> GetPlaces();
    }
}
