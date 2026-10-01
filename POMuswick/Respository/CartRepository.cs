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

        public Task<int> GetCartPieces()
        {
            return Task.Run(() => _db.GetCartPieces());
        }
        public Task<List<Item>> GetCartItems()
        {
            return Task.Run(() => _db.GetCartItems());
        }
        public Task<List<Item>> GetCheckoutItem()
        {
            return Task.Run(() => _db.GetCheckoutItems());
        }

        public Task RestoreCartItems()
        {
            return Task.Run(() =>
            {
                var appSettings = _settingRepository.Load();
                _db.RestoreCartItems(appSettings.CustomerNo);
            });
        }

        public Task SuspendCartItems(string customerNumber)
        {
            return Task.Run(() => _db.SuspendCartItems(customerNumber));
        }

        public Task ClearCartItems()
        {
            return Task.Run(() => _db.ClearCartItems());
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