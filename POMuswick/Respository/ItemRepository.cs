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
            return Task.Run(() =>
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
            });
        }
        public Task DeletedDiscontinuedItem(List<int> itemNumbers)
        {
            return Task.Run(() => _db.DeleteDiscontinuedItems(itemNumbers));
        }

        public Task UpdateDiscontinuedItem()
        {
            return Task.Run(() => _db.UpdateDiscontinuedItems());
        }

        public Task<List<Item>> Load()
        {
            return Task.Run(() => _db.GetItems() ?? new List<Item>());
        }
        public Task<List<Item>> LoadReorderItems()
        {
            return Task.Run(() => _db.GetReorderItems() ?? new List<Item>());
        }
        public Task<List<Item>> LoadNewItems(bool stock)
        {
            return Task.Run(() => _db.GetNewItems("", false, stock) ?? new List<Item>());
        }
        public Task<List<Item>> SearchItemsQuickEntry(string searchTerm)
        {
            return Task.Run(() => _db.SearchItemsQuickEntry(searchTerm) ?? new List<Item>());
        }
        public Task<List<Item>> SearchItemsAsync(bool stock, string searchText, Category category, string scanBarcode, Subcategory subcategory)
        {
            return Task.Run(() => _db.SearchItems(stock, searchText, category, scanBarcode, subcategory));
        }
        public Task<Item> GetItemByItemNo(int itemNo)
        {
            return Task.Run(() => _db.FindItem(itemNo));
        }
        public Task UpdateItemQtySet(int itemNo, int qty)
        {
            return Task.Run(() =>
            {
                IDictionary<int, int> itemQtyDict = new Dictionary<int, int>
                {
                    { itemNo, qty }
                };
                _db.UpdateItemQtySet(itemQtyDict.ToDictionary());
            });
        }

        public Task<List<Item>> SearchItemsKeyword(string searchText, bool stock)
        {
            return Task.Run(() => _db.SearchItemsKeyword(searchText, stock) ?? new List<Item>());
        }
        public Task<int> GetItemQty(int itemNo)
        {
            return Task.Run(() => _db.GetItemQty(itemNo));
        }
        public Task Clear()
        {
            return Task.Run(() => _db.DeleteItems());
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
        public Task<List<Item>> SearchItemsAsync(bool stock, string searchText, Category category, string scanBarcode, Subcategory subcategory);
        Task<List<Item>> SearchItemsKeyword(string searchText, bool stock);
    }
}