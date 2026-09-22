namespace POMuswick.Repository
{

    public class CustomerRepository : ICustomerRepository
    {
        private readonly Database _db;

        public CustomerRepository(Database db)
        {
            _db = db;
        }
        public void Save(Customer currentCustomer)
        {
            _db.SaveCustomer(currentCustomer);
        }

        public Customer Load()
        {
            var customer = _db.GetCustomer();
            return customer ?? new Customer();
        }

        public void Clear()
        {
            _db.DeleteCustomer();
        }
    }

    public interface ICustomerRepository
    {
        public void Save(Customer currentCustomer);
        public Customer Load();

        public void Clear();
    }
}