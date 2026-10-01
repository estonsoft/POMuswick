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
            var bannersToSave = banners.Select(banner => banner.CopyDetached()).ToList();
            Clear();
            _db.SaveBannerAsync(bannersToSave);
        }

        public Task<List<Banner>> Load()
        {
            return Task.Run(() => _db.GetBanners() ?? new List<Banner>());
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