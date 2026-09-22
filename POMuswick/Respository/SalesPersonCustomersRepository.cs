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
            _db.SaveSalesCustomer(salesPersonCustomers);
           return Task.CompletedTask;
        }

        public Task<List<SalesCustomer>> Load()
        {
            var salesPersonCustomers = _db.GetSalesCustomers();
            return Task.FromResult(salesPersonCustomers ?? new List<SalesCustomer>());
        }

        public Task Clear()
        {
            _db.DeleteSalesCustomers();
            return Task.CompletedTask;
        }

        public async Task<SalesCustomer> FindSalesCustomer(string custNo)
        {
            var salesPersonCustomers = _db.FindSalesCustomer(custNo);
            return salesPersonCustomers;
        }
        public async Task<List<SalesCustomer>> GetSalesCustomers(string search)
        {
            var salesPersonCustomers = _db.GetSalesCustomers(search);
            return salesPersonCustomers;
        }
    }

    public interface ISalesPersonCustomersRepository
    {
        public Task Save(List<SalesCustomer> SalesPersonCustomerss);
        public Task<List<SalesCustomer>> Load();
        public Task<List<SalesCustomer>>GetSalesCustomers(string search);
        public Task Clear();
        public Task<SalesCustomer> FindSalesCustomer(string custNo);
    }
}