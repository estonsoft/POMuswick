using Realms;

namespace POMuswick
{
    public class Category : Realms.RealmObject
    {
        [PrimaryKey]
        public string Code { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public int HomePage { get; set; }
        public int Rank { get; set; }
        public string ImageBase64 { get; set; }

        public Category CopyDetached()
        {
            return new Category
            {
                Code = Code,
                Description = Description,
                ImageURL = ImageURL,
                HomePage = HomePage,
                Rank = Rank,
                ImageBase64 = ImageBase64
            };
        }
    }
}
