using POMuswick.Data;
using POMuswick.Parsers;
using POMuswick.Repository;
namespace POMuswick.Services
{
    public class SalesPersonCustomersService : ISalesPersonCustomersService
    {
        private readonly CommManager _comm;
        private readonly SalesPersonCustomersParser _parser;
        private readonly ISalesPersonCustomersRepository _SalesPersonCustomersRepository;

        public SalesPersonCustomersService(
            CommManager comm,
            SalesPersonCustomersParser parser,
            ISalesPersonCustomersRepository SalesPersonCustomersRepository)
        {
            _comm = comm;
            _parser = parser;
            _SalesPersonCustomersRepository = SalesPersonCustomersRepository;
        }

        public async Task<SalesPersonCustomersResult> FetchSalesPersonCustomersAsync(string user)
        {
            var response = await _comm.GetSalespersonCustomers(user);
            SalesPersonCustomersResult result = await _parser.Parse(response);
            await _SalesPersonCustomersRepository.Clear();
            await _SalesPersonCustomersRepository.Save(result.salesCustomers);
            return result;
        }

        public async Task<SalesCustomer> FindSalesCustomer(string custNo)
        {
            return await _SalesPersonCustomersRepository.FindSalesCustomer(custNo);
        }
        public async Task<List<SalesCustomer>> GetSalesCustomer(string search)
        {
            return await _SalesPersonCustomersRepository.GetSalesCustomers(search);
        }

        public async Task Clear()
        {
            await _SalesPersonCustomersRepository.Clear();
        }
    }

    public interface ISalesPersonCustomersService
    {
        public Task<SalesPersonCustomersResult> FetchSalesPersonCustomersAsync(string user);
        public Task<SalesCustomer> FindSalesCustomer(string custNo);
        public Task<List<SalesCustomer>> GetSalesCustomer(string search);
         public Task Clear();
    }
}