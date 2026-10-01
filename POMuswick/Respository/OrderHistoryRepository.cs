namespace POMuswick.Repository
{

    public partial class OrderHistoryRepository : IOrderHistoryRepository
    {
        private readonly Database _db;

        public OrderHistoryRepository(Database db)
        {
            _db = db;
        }
        public Task SaveOrderHeaders(OrderHeader orderHeader)
        {
            return Task.Run(() => _db.SaveOrderHeader(orderHeader));
        }

        public Task SaveOrderDetails(OrderDetail orderDetail)
        {
            return Task.Run(() => _db.SaveOrderDetail(orderDetail));
        }

        public async Task SaveOrderHeaders(List<OrderHeader> orderHeaders)
        {
            await ClearOrderHistory();
            await Task.Run(() => _db.SaveAllOrderHeader(orderHeaders));
        }

        public async Task SaveOrderDetails(List<OrderDetail> orderDetail)
        {
            await ClearOrderDetail("");
            await Task.Run(() => _db.SaveAllOrderDetail(orderDetail));
        }
        public Task<List<OrderHeader>> LoadOrderheaders(string orderNumber)
        {
            return Task.Run(() => _db.GetOrderHeaders(orderNumber) ?? new List<OrderHeader>());//OrderNumer
        }
        public Task<List<OrderDetail>> LoadOrderDetails(string orderNumber)
        {
            return Task.Run(() => _db.GetOrderDetail(orderNumber) ?? new List<OrderDetail>());//OrderNumer
        }
        public Task<OrderHeader> LoadOrderHeader(string orderNumber)
        {
            return Task.Run(() => _db.GetOrderHeader(orderNumber));
        }
        public Task ClearOrderDetail(string orderNumber)
        {
            return Task.Run(() => _db.DeleteOrderDetail(orderNumber));//OrderNumer
        }

        public Task ClearOrderHistory()
        {
            return Task.Run(() => _db.DeleteOrderHistory());
        }
    }

    public interface IOrderHistoryRepository
    {
        public Task SaveOrderHeaders(OrderHeader orderHeaders);

        public Task SaveOrderDetails(OrderDetail orderDetails);
        public Task SaveOrderHeaders(List<OrderHeader> orderHeaders);

        public Task SaveOrderDetails(List<OrderDetail> orderDetails);
        public Task<List<OrderHeader>> LoadOrderheaders(string orderNumber);
        public Task<OrderHeader> LoadOrderHeader(string orderNumber);
        public Task<List<OrderDetail>> LoadOrderDetails(string orderNumber);

        public Task ClearOrderDetail(string orderNumber);

        public Task ClearOrderHistory();
    }
}