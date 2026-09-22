namespace POMuswick.Repository
{

    public class ItemQQHRepository : IItemQQHRepository
    {
        private readonly Database _db;

        public ItemQQHRepository(Database db)
        {
            _db = db;
        }
        public void UpdateQQH(Dictionary<int,int> ItemQQHs)
        {
            _db.UpdateItemQOH(ItemQQHs);
        }

        public void UpdateItemQtySet(Dictionary<int,int> ItemQQHs)
        {
            _db.UpdateItemQtySet(ItemQQHs);
        }
        public void Clear()
        {
            _db.DeleteItems();
        }
    }

    public interface IItemQQHRepository
    {
        public void UpdateQQH(Dictionary<int,int> ItemQQHs);
        public void UpdateItemQtySet(Dictionary<int,int> ItemQQHs);
        void Clear();
    }
}