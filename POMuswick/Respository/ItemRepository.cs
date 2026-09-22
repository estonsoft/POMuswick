namespace POMuswick.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly Database _db;

        public ItemRepository(Database db)
        {
            _db = db;
        }

        public Task SaveItems(List<Item> items)
        {
            List<Item> lstCartItems = _db.GetCartItems();
            var cartDict = lstCartItems.ToDictionary(c => c.ItemNo);
            items.ForEach(item =>
            {
                item.QtyOrder = cartDict.TryGetValue(item.ItemNo, out var cart)
                    ? cart.QtyOrder
                    : 0;
            });
            _db.SaveItems(items);
            return Task.CompletedTask;
        }
        public Task DeletedDiscontinuedItem(List<int> itemNumbers)
        {
            _db.DeleteDiscontinuedItems(itemNumbers);
            return Task.CompletedTask;
        }

        public Task UpdateDiscontinuedItem()
        {
            _db.UpdateDiscontinuedItems();
            return Task.CompletedTask;
        }

        public async Task<List<Item>> Load()
        {
            var items = _db.GetItems();
            return items ?? new List<Item>();
        }

        public async Task<List<Item>> LoadReorderItems()
        {
            var items = _db.GetReorderItems();
            return items ?? new List<Item>();
        }

        public async Task<List<Item>> LoadNewItems(bool stock)
        {
            var items = _db.GetNewItems("",true,stock);
            return items ?? new List<Item>();
        }

        public async Task<List<Item>> SearchItemsQuickEntry(string searchTerm)
        {
            var items = _db.SearchItemsQuickEntry(searchTerm);
            return items ?? new List<Item>();
        }

        public async Task<List<Item>> SearchItemsAsync(bool stock,string searchText, Category category, string scanBarcode, Subcategory subcategory)
        {
            var items = _db.SearchItems(stock,searchText, category, scanBarcode, subcategory);
            return items;
        }

        public async Task<Item> GetItemByItemNo(int itemNo)
        {
            var item = _db.FindItem(itemNo);
            return item;
        }
        public async Task UpdateItemQtySet(int itemNo, int qty)
        {
            IDictionary<int, int> itemQtyDict = new Dictionary<int, int>
            {
                { itemNo, qty }
            };
            _db.UpdateItemQtySet(itemQtyDict.ToDictionary());
        }

        public async Task<List<Item>> SearchItemsKeyword(string searchText,bool stock)
        {
            var items = _db.SearchItemsKeyword(searchText,stock);
            return items ?? new List<Item>();
        }

        public async Task<int> GetItemQty(int itemNo)
        {
            var items = _db.GetItemQty(itemNo);
            return items;
        }
        public Task Clear()
        {
            _db.DeleteItems();
            return Task.CompletedTask;
        }
    }

    public interface IItemRepository
    {
        public Task SaveItems(List<Item> items);
        public Task DeletedDiscontinuedItem(List<int> itemNumbers);
        public Task UpdateDiscontinuedItem();
        public Task<List<Item>> Load();
        public Task<List<Item>> LoadReorderItems();
        public Task<List<Item>> LoadNewItems(bool stock);
        public Task UpdateItemQtySet(int itemNo, int qty);

        public Task<List<Item>> SearchItemsQuickEntry(string searchTerm);
        public Task<int> GetItemQty(int itemNo);
        public Task<Item> GetItemByItemNo(int itemNo);
        public Task Clear();
        public Task<List<Item>> SearchItemsAsync(bool stock,string searchText, Category category, string scanBarcode, Subcategory subcategory);
        Task<List<Item>> SearchItemsKeyword(string searchText,bool stock);
    }
}