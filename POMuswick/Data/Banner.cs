using Realms;

namespace POMuswick
{
    public class Banner : Realms.RealmObject
    {
        [PrimaryKey]
        public string BannerName { get; set; } = "";
        public string BannerURL { get; set; } = "";

        public Banner CopyDetached()
        {
            return new Banner
            {
                BannerName = BannerName,
                BannerURL = BannerURL
            };
        }
    }
}
