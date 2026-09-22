namespace POMuswick.Repository
{

    public class CatNSubcatRepository : ICatNSubcatRepository
    {
        private readonly Database _db;

        public CatNSubcatRepository(Database db)
        {
            _db = db;
        }
        public void SaveCategories(List<Category> categories)
        {
            Clear();
            _db.SaveCategory(categories);
        }

        public void SaveSubCategories(List<Subcategory> subcategories)
        {
            _db.SaveSubcategory(subcategories);
        }


        public async Task<List<Category>> LoadCategories()
        {
            var categories = _db.GetCategories();
            return categories ?? new List<Category>();
        }

        public async Task<List<Subcategory>> LoadSubCategories(string cateCode)
        {
            var subcategories = _db.GetSubcategory(cateCode);
            return subcategories ?? new List<Subcategory>();
        }

        public async Task<List<Category>> LoadHomePageCategories()
        {
            var homePageCategories = _db.GetHomePageCategories();
            return homePageCategories ?? new List<Category>();
        }

        public void Clear()
        {
            _db.DeleteAllCategory();
            _db.DeleteAllSubcategory();
        }
    }

    public interface ICatNSubcatRepository
    {
        public void SaveCategories(List<Category> categories);
        public void SaveSubCategories(List<Subcategory> subcategories);

        public  Task<List<Category>> LoadCategories();

        public Task<List<Category>> LoadHomePageCategories();

        public Task<List<Subcategory>> LoadSubCategories(string cateCode);

        public void Clear();
    }
}