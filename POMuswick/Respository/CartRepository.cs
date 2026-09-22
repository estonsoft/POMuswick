namespace POMuswick.Repository
{

    public class CartRepository : ICartRepository
    {
        private readonly Database _db;

        private readonly ISettingRepository _settingRepository;

        public CartRepository(Database db, ISettingRepository settingRepository)
        {
            _db = db;
            _settingRepository = settingRepository;
        }

        public async Task<int> GetCartPieces()
        {
            return _db.GetCartPieces();
        }
        public async Task<List<Item>> GetCartItems()
        {
            return _db.GetCartItems();
        }

        public async Task<List<Item>> GetCheckoutItem()
        {
            return _db.GetCheckoutItems();
        }
        public async Task RestoreCartItems()
        {
            var appSettings = _settingRepository.Load();
            _db.RestoreCartItems(appSettings.CustomerNo);
        }

        public async Task SuspendCartItems(string customerNumber)
        {
            _db.SuspendCartItems(customerNumber);
        }

        public async Task ClearCartItems()
        {
            _db.ClearCartItems();
        }
    }

    public interface ICartRepository
    {
        public Task<int> GetCartPieces();
        public Task<List<Item>> GetCartItems();
        public Task RestoreCartItems();
        public Task SuspendCartItems(string customerNumber);
        public Task ClearCartItems();
        public Task<List<Item>> GetCheckoutItem();
    }
}