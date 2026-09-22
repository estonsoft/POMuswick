namespace POMuswick.Repository
{

    public class LocationRepository:ILocationRepository
    {
        private readonly Database _db;

        public LocationRepository(Database db)
        {
            _db = db;
        }
        public void Save(Location currentLocation)
        {
            _db.SaveLocation(currentLocation);
        }

        public Location Load(int currentlocation)
        {
            var Location = _db.GetLocation(currentlocation);
            return Location ?? new Location();
        }

        public void Clear()
        {
            _db.SaveLocation(new Location());
        }
    }

    public interface ILocationRepository
    {
        public void Save(Location currentLocation);
        public Location Load(int currentlocation);
        public void Clear();
    }
}