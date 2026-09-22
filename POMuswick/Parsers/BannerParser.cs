using System.Collections.Concurrent;
using POMuswick.Models;

namespace POMuswick.Parsers
{
    public class BannerParser : BaseParser
    {
        public BannerParser()
        {

        }
        public async Task<BannerResult> Parse(string response)
        {
            Console.WriteLine("Get Banners returned");
            BannerResult result = new();
            String sBanners = response;
            String[] aBanners = sBanners.Split('|');
            ConcurrentBag<Banner> lstBanners = new ConcurrentBag<Banner>();
            if (aBanners.Length >= 1)
            {
                Parallel.ForEach(aBanners, s =>
                {
                    string bannerName = s.Trim();
                    if (string.IsNullOrWhiteSpace(bannerName))
                    {
                        return;
                    }

                    Banner banner = new Banner();
                    banner.BannerName = bannerName;
                    banner.BannerURL = Constants.BannerUrl + banner.BannerName;
                    lstBanners.Add(banner);
                });
            }
            result.banners = lstBanners.ToList<Banner>();
            return result;
        }
    }
}