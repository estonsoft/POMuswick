namespace POMuswick.Repository
{

    public class BannerRepository : IBannerRepository
    {
        private readonly Database _db;

        public BannerRepository(Database db)
        {
            _db = db;
        }
        public void Save(List<Banner> banners)
        {
            Clear();
            _db.SaveBannerAsync(banners);
        }

        public Task<List<Banner>> Load()
        {
            var banners = _db.GetBanners() ?? new List<Banner>();
            return Task.FromResult(banners);
        }

        public void Clear()
        {
            _db.DeleteBannersAsync();
        }
    }

    public interface IBannerRepository
    {
        public void Save(List<Banner> banners);
        public Task<List<Banner>> Load();
        public void Clear();
    }
}