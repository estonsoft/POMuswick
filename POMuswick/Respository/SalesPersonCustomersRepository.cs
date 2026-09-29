namespace POMuswick.Repository
{

    public partial class SalesPersonCustomersRepository : ISalesPersonCustomersRepository
    {
        private readonly Database _db;

        public SalesPersonCustomersRepository(Database db)
        {
            _db = db;
        }
        public Task Save(List<SalesCustomer> salesPersonCustomers)
        {
            return Task.Run(() => _db.SaveSalesCustomer(salesPersonCustomers));
        }

        public Task<List<SalesCustomer>> Load()
        {
            return Task.Run(() => _db.GetSalesCustomers() ?? new List<SalesCustomer>());
        }

        public Task Clear()
        {
            return Task.Run(() => _db.DeleteSalesCustomers());
        }

        public Task<SalesCustomer> FindSalesCustomer(string custNo)
        {
            return Task.Run(() => _db.FindSalesCustomer(custNo));
        }
        public Task<List<SalesCustomer>> GetSalesCustomers(string search)
        {
            return Task.Run(() => _db.GetSalesCustomers(search));
        }

        public Task<List<SalesCustomer>> GetSalesCustomers(string search, int skip, int take)
        {
            return Task.Run(() => _db.GetSalesCustomers(search, skip, take));
        }
    }

    public interface ISalesPersonCustomersRepository
    {
        public Task Save(List<SalesCustomer> SalesPersonCustomerss);
        public Task<List<SalesCustomer>> Load();
        public Task<List<SalesCustomer>> GetSalesCustomers(string search);
        public Task<List<SalesCustomer>> GetSalesCustomers(string search, int skip, int take);
        public Task Clear();
        public Task<SalesCustomer> FindSalesCustomer(string custNo);
    }
}