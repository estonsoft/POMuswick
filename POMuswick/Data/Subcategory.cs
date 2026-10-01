using Realms;


namespace POMuswick
{
    public class Subcategory : Realms.RealmObject
    {
        [PrimaryKey]
        public string Code { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Rank { get; set; }
        public string ImageBase64 { get; set; }

        public Subcategory CopyDetached()
        {
            return new Subcategory
            {
                Code = Code,
                Category = Category,
                Description = Description,
                Rank = Rank,
                ImageBase64 = ImageBase64
            };
        }
    }
}
