using POMuswick.Repository;
namespace POMuswick.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _LocationRepository;

        public LocationService(
            ILocationRepository LocationRepository)
        {
            _LocationRepository = LocationRepository;
        }

        public async Task<Location> GetLocationAsync(int location)
        {
            return _LocationRepository.Load(location);
        }

        public async Task SaveLocationAsync(Location location)
        {
            _LocationRepository.Save(location);
        }
    }

    public interface ILocationService
    {
        public Task<Location> GetLocationAsync(int location);
        public Task SaveLocationAsync(Location location);
    }
}