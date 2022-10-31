using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Data.UnitOfWork;
using SSC.DTO.Place;

namespace SSC.Data.Repositories
{
    public class PlaceRepository : BaseRepository<Place>, IPlaceRepository
    {
        private readonly DataContext context;
        private readonly IUnitOfWork unitOfWork;

        public PlaceRepository(DataContext context, IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
        }

        public async Task<DbResult<Place>> AddPlace(PlaceCreateDTO place)
        {
            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => context.Places.AnyAsync(x => x.Name == place.Name).Result, "Placówka została już dodana" },
                { () => unitOfWork.CityRepository.GetCity(place.CityId.Value).Result == null, "Miejscowość nie istnieje" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            Place newPlace = unitOfWork.Mapper.Map<Place>(place);

            await context.AddAsync(newPlace);
            await context.SaveChangesAsync();

            return DbResult<Place>.CreateSuccess("Placówka została dodana", newPlace);
        }

        public async Task<bool> AnyPlace(Guid placeId)
        {
            return await context.Places.AnyAsync(x => x.Id == placeId);
        }

        public async Task<List<Place>> GetPlaces()
        {
            return await context.Places
                .Include(x => x.City.Province)
                .ToListAsync();
        }
    }
}
