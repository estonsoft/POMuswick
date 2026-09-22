using POMuswick.Data;
using POMuswick.Parsers;
using POMuswick.Repository;
namespace POMuswick.Services
{
    public class CatNSubCatService : ICatNSubCatService
    {
        private readonly CommManager _comm;
        private readonly CatNSubCatParser _parser;
        private readonly ICatNSubcatRepository _catNSubcatRepository;

        public CatNSubCatService(
            CommManager comm,
            CatNSubCatParser parser,
            ICatNSubcatRepository catNSubcatRepository)
        {
            _comm = comm;
            _parser = parser;
            _catNSubcatRepository = catNSubcatRepository;
        }

        public async Task<CatNSubcatResult> GetCategoriesAndSubCategories()
        {
            var response = await _comm.GetCategoriesAndSubcategories();

            CatNSubcatResult result = await _parser.Parse(response);

            if (result.categories.Count > 0)
            {
                _catNSubcatRepository.SaveCategories(result.categories);
            }
            if (result.subcategories.Count > 0)
            {
                _catNSubcatRepository.SaveSubCategories(result.subcategories);
            }
            return result;
        }
        
        public async Task<CatNSubcatResult> GetCategoriesAndSubCategories(string selectedCustomer)
        {
            var response = await _comm.GetCategoriesAndSubcategoriesCust(selectedCustomer);

            CatNSubcatResult result = await _parser.Parse(response);
            _catNSubcatRepository.Clear();
            if (result.categories.Count > 0)
            {
                _catNSubcatRepository.SaveCategories(result.categories);
            }
            if (result.subcategories.Count > 0)
            {
                _catNSubcatRepository.SaveSubCategories(result.subcategories);
            }
            return result;
        }

        public async Task<List<Category>> GetCategories()
        {
            return await _catNSubcatRepository.LoadCategories();
        }

        public async Task<List<Subcategory>> GetSubCategories(string cateCode)
        {
            return await _catNSubcatRepository.LoadSubCategories(cateCode);
        }
    }

    public interface ICatNSubCatService
    {
        public Task<CatNSubcatResult> GetCategoriesAndSubCategories();
        public Task<CatNSubcatResult> GetCategoriesAndSubCategories(string selectedCustomer);
        public Task<List<Category>> GetCategories();
        public Task<List<Subcategory>> GetSubCategories(string cateCode);
    }
}