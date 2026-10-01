using Realms;


namespace POMuswick
{
    public class OrderHeader : Realms.RealmObject
    {
        [PrimaryKey]
        public string OrderNo { get; set; }
        public int CustId { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public string OrderDateDisplay { get; set; }
        public int Items { get; set; }
        public int Pieces { get; set; }
        public decimal Total { get; set; }
        public string TotalDisplay { get; set; }
        public string Status { get; set; }

        public OrderHeader CopyDetached()
        {
            return new OrderHeader
            {
                OrderNo = OrderNo,
                CustId = CustId,
                OrderDate = OrderDate,
                OrderDateDisplay = OrderDateDisplay,
                Items = Items,
                Pieces = Pieces,
                Total = Total,
                TotalDisplay = TotalDisplay,
                Status = Status
            };
        }
    }
}
