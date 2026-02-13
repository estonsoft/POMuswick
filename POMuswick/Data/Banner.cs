using SQLite;

namespace POMuswick
{
    public class Banner
    {
        [PrimaryKey]
        public string BannerName { get; set; }
        public string BannerURL { get; set; }
    }
}
