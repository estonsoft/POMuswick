using POMuswick.Data;
using POMuswick.Parsers;
using POMuswick.Repository;
namespace POMuswick.Services
{
    public class ItemService : IItemService
    {
        private readonly CommManager _comm;
        private readonly ItemParser _parser;
        private readonly IItemRepository _ItemRepository;
        private readonly ICustomerRepository _customerRepository;

        public ItemService(
            CommManager comm,
            ItemParser parser,
            IItemRepository itemRepository,
            ICustomerRepository customerRepository)
        {
            _comm = comm;
            _parser = parser;
            _ItemRepository = itemRepository;
            _customerRepository = customerRepository;
        }

        public async Task<ItemResult> FetchItemAsync()
        {
            var customer = _customerRepository.Load();

            var response = await _comm.GetItems(customer.CustNo,"0");

            ItemResult result = await _parser.Parse(response);

            await _ItemRepository.SaveItems(result.items);

            await _ItemRepository.DeletedDiscontinuedItem(result.itemNumbers);

            await _ItemRepository.UpdateDiscontinuedItem();

            return result;
        }

        public async Task<ItemResult> FetchNewItemAsync(bool stock)
        {
            var newItems = await _ItemRepository.LoadNewItems(stock);
            ItemResult itemResult = new ItemResult()
            {
                items = newItems
            };
            return itemResult;
        }
        
        public async Task<List<Item>> SearchItemsQuickEntry(string searchTerm)
        {
            var items = await _ItemRepository.SearchItemsQuickEntry(searchTerm);
            return items;
        }

        public async Task<List<Item>> SearchItemsAsync(bool stock,string searchText, Category category, string scanBarcode, Subcategory subcategory)
        {
           var items =  await _ItemRepository.SearchItemsAsync(stock,searchText, category, scanBarcode, subcategory);
            return items;
        }

        public async Task<Item> GetItemByItemNo(int itemNo)
        {
            return await _ItemRepository.GetItemByItemNo(itemNo);
        }

        public async Task UpdateItemQtySet(int itemNo, int qty)
        {
            await _ItemRepository.UpdateItemQtySet(itemNo, qty);
        }

        public async Task<int> GetItemQty(int itemNo)
        {
            return await _ItemRepository.GetItemQty(itemNo);
        }

        public async Task<List<Item>> SearchItemsKeyword(string searchText,bool stock)
        {
            return await _ItemRepository.SearchItemsKeyword(searchText,stock);
        }

        public async Task<ItemResult> FetchReorderItemsAsync()
        {
            var reorderItems = await _ItemRepository.LoadReorderItems();
            ItemResult itemResult = new ItemResult()
            {
                items = reorderItems
            };
            return itemResult;
        }
    }

    public interface IItemService
    {
        public Task<ItemResult> FetchItemAsync();

        public Task<ItemResult> FetchNewItemAsync(bool stock);
        public Task<ItemResult> FetchReorderItemsAsync();
        public Task UpdateItemQtySet(int itemNo, int qty);

        public Task<List<Item>> SearchItemsQuickEntry(string searchTerm);
        public Task<Item> GetItemByItemNo(int itemNo);
        public Task<int> GetItemQty(int itemNo);
        public Task<List<Item>>SearchItemsAsync(bool stock,string searchText, Category category,string scanBarcode, Subcategory subcategory);
        public Task<List<Item>> SearchItemsKeyword(string searchText,bool stock);
    }
}