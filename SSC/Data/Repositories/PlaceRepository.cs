using Microsoft.EntityFrameworkCore;

namespace SSC.Data.Repositories
{
    public class PlaceRepository : IPlaceRepository
    {
        private readonly DataContext context;

        public PlaceRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<bool> AnyPlace(Guid placeId)
        {
            return await context.Places.AnyAsync(x => x.Id == placeId);
        }
    }
}
