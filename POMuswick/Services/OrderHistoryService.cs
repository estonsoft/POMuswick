using POMuswick.Data;
using POMuswick.Parsers;
using POMuswick.Repository;
namespace POMuswick.Services
{
    public class OrderHistoryService : IOrderHistoryService
    {
        private readonly CommManager _comm;
        private readonly OrderHistoryParser _parser;
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly ICustomerRepository _customerRepository;

        public OrderHistoryService(
            CommManager comm,
            OrderHistoryParser parser,
            IOrderHistoryRepository orderHistoryRepository,
            ICustomerRepository customerRepository)
        {
            _comm = comm;
            _parser = parser;
            _orderHistoryRepository = orderHistoryRepository;
            _customerRepository = customerRepository;
        }

        public async Task<OrderHistoryResult> FetchOrderHistoryAsync()
        {
            var result = await FetchOrderHistoryAsync(_customerRepository.Load().CustNo);
            return result;
        }

        public async Task<OrderHistoryResult> FetchOrderHistoryAsync(string customerNumber)
        {
            var response = await _comm.GetOrderHistory(customerNumber);

            OrderHistoryResult result = _parser.Parse(response);
            await _orderHistoryRepository.SaveOrderHeaders(result.Headers);
            await _orderHistoryRepository.SaveOrderDetails(result.Details);
            return result;
        }

        public async Task<OrderHeader> FetchOrderHeaderAsync(string orderNumber)
        {
            return await _orderHistoryRepository.LoadOrderHeader(orderNumber);
        }

        public async Task<List<OrderHeader>> FetchOrderHeadersAsync(string orderNumber)
        {
            return await _orderHistoryRepository.LoadOrderheaders(orderNumber);
        }

public async Task<List<OrderDetail>> FetchOrderDetailsAsync(string orderNumber)
        {
            return await _orderHistoryRepository.LoadOrderDetails(orderNumber);
        }

        public async Task ClearOrderHistory()
        {
            await _orderHistoryRepository.ClearOrderHistory();
        }
    }

    public interface IOrderHistoryService
    {
        public Task<OrderHistoryResult> FetchOrderHistoryAsync();
        public Task<OrderHistoryResult> FetchOrderHistoryAsync(string customerNumber);
        public Task<OrderHeader> FetchOrderHeaderAsync(string orderNumber);
        public Task<List<OrderHeader>> FetchOrderHeadersAsync(string orderNumber);
        public Task<List<OrderDetail>> FetchOrderDetailsAsync(string orderNumber);
        public Task ClearOrderHistory();
    }
}