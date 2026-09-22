using POMuswick.Data;
using POMuswick.Parsers;
using POMuswick.Repository;
namespace POMuswick.Services
{
    public class BannerService : IBannerService
    {
        private readonly CommManager _comm;
        private readonly BannerParser _parser;
        private readonly IBannerRepository _bannerRepository;

        public BannerService(
            CommManager comm,
            BannerParser parser,
            IBannerRepository bannerRepository)
        {
            _comm = comm;
            _parser = parser;
            _bannerRepository = bannerRepository;
        }

        public async Task<BannerResult> FetchBannerAsync()
        {
            var response = await _comm.GetBanners();

            BannerResult result = await _parser.Parse(response);

            if (result.banners.Count > 0)
            {
                _bannerRepository.Save(result.banners);
            }
            return result;
        }

        public async Task<BannerResult> GetBannerAsync()
        {
            var response = await _bannerRepository.Load();

            BannerResult result = new()
            {
                banners = response
            };
            return result;
        }
    }

    public interface IBannerService
    {
        public Task<BannerResult> FetchBannerAsync();
        public Task<BannerResult> GetBannerAsync();
    }
}