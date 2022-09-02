namespace SSC.Data.Repositories
{
    public interface IPlaceRepository
    {
        Task<bool> AnyPlace(Guid placeId);
    }
}
