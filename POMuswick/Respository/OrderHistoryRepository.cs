namespace POMuswick.Repository
{

    public partial class OrderHistoryRepository : IOrderHistoryRepository
    {
        private readonly Database _db;

        public OrderHistoryRepository(Database db)
        {
            _db = db;
        }
        public async Task SaveOrderHeaders(OrderHeader orderHeader)
        {
            _db.SaveOrderHeader(orderHeader);
        }

        public async Task SaveOrderDetails(OrderDetail orderDetail)
        {
            _db.SaveOrderDetail(orderDetail);
        }

        public async Task SaveOrderHeaders(List<OrderHeader> orderHeaders)
        {
            await ClearOrderHistory();
            _db.SaveAllOrderHeader(orderHeaders);
        }

        public async Task SaveOrderDetails(List<OrderDetail> orderDetail)
        {
            await ClearOrderDetail("");
            _db.SaveAllOrderDetail(orderDetail);
        }
        public async Task<List<OrderHeader>> LoadOrderheaders(string orderNumber)
        {
            var orderHeader = _db.GetOrderHeaders(orderNumber);//OrderNumer
            return orderHeader ?? new List<OrderHeader>();
        }

        public async Task<List<OrderDetail>> LoadOrderDetails(string orderNumber)
        {
            var orderDetails = _db.GetOrderDetail(orderNumber);//OrderNumer
            return orderDetails ?? new List<OrderDetail>();
        }

        public Task<OrderHeader> LoadOrderHeader(string orderNumber)
        {
            return Task.FromResult(_db.GetOrderHeader(orderNumber));
        }

        public async Task ClearOrderDetail(string orderNumber)
        {
            _db.DeleteOrderDetail(orderNumber);//OrderNumer
        }

        public async Task ClearOrderHistory()
        {
            _db.DeleteOrderHistory();
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