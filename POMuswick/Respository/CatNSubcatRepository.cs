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


        public Task<List<Category>> LoadCategories()
        {
            return Task.Run(() => _db.GetCategories() ?? new List<Category>());
        }

        public Task<List<Subcategory>> LoadSubCategories(string cateCode)
        {
            return Task.Run(() => _db.GetSubcategory(cateCode) ?? new List<Subcategory>());
        }

        public Task<List<Category>> LoadHomePageCategories()
        {
            return Task.Run(() => _db.GetHomePageCategories() ?? new List<Category>());
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

        public Task<List<Category>> LoadCategories();

        public Task<List<Category>> LoadHomePageCategories();

        public Task<List<Subcategory>> LoadSubCategories(string cateCode);

        public void Clear();
    }
}