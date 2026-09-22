using POMuswick.Repository;
namespace POMuswick.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _CartRepository;

        public CartService(
            ICartRepository CartRepository)
        {
            _CartRepository = CartRepository;
        }

        public async Task<List<Item>> GetCartItems()
        {
            return await _CartRepository.GetCartItems();
        }
        public async Task<int> GetCartPieces()
        {
            return await _CartRepository.GetCartPieces();
        }

        public async Task<List<Item>> GetCheckoutItem()
        {
            return await _CartRepository.GetCheckoutItem();
        }

        public async Task RestoreCart()
        {
            await _CartRepository.RestoreCartItems();
        }

        public async Task SuspendCartItems(string customerNumber)
        {
            await _CartRepository.SuspendCartItems(customerNumber);
        }

        public async Task ClearCartItems()
        {
            await _CartRepository.ClearCartItems();
        }
    }

    public interface ICartService
    {
        public Task<List<Item>> GetCartItems();
        public Task<int> GetCartPieces();
        public Task RestoreCart();
        public Task SuspendCartItems(string customerNumber);
        public Task ClearCartItems();
        public Task<List<Item>> GetCheckoutItem();
    }
}