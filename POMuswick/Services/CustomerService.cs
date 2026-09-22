using POMuswick.Data;
using POMuswick.Repository;
namespace POMuswick.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CommManager _comm;
        private readonly ICustomerRepository _CustomerRepository;

        public CustomerService(
            CommManager comm,
            ICustomerRepository CustomerRepository)
        {
            _comm = comm;
            _CustomerRepository = CustomerRepository;
        }

        public async Task<Customer> GetCustomerAsync()
        {
            return _CustomerRepository.Load();
        }

        public async Task SaveCustomerAsync(Customer customer)
        {
            _CustomerRepository.Save(customer);
        }

        public async Task ClearCustomer()
        {
            _CustomerRepository.Clear();
        }
    }

    public interface ICustomerService
    {
        public Task<Customer> GetCustomerAsync();
        public Task SaveCustomerAsync(Customer customer);
        public Task ClearCustomer();
    }
}