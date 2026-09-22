using System.Collections.Concurrent;
using POMuswick.Models;

namespace POMuswick.Parsers
{
    public class OrderHistoryParser : BaseParser
    {
        public OrderHistoryParser()
        {

        }
        public OrderHistoryResult Parse(string response)
        {
            OrderHistoryResult result = new();

            var headerLookup = new Dictionary<string, OrderHeader>();

            foreach (var row in response.Split('~'))
            {
                var order = row.Split('|');

                if (order.Length < 24)
                    continue;

                if (!headerLookup.TryGetValue(order[0], out var header))
                {
                    header = BuildHeader(order);

                    headerLookup.Add(header.OrderNo, header);

                    result.Headers.Add(header);
                }

                result.Details.Add(BuildDetail(order));
            }

            return result;
        }

        private OrderHeader BuildHeader(string[] order)
        {
            return new OrderHeader
            {
                OrderNo = order[0],
                CustId = GetIntegerValue("Customer ID", order[1], 0),
                OrderDate = GetDateTime("Order Date", order[2]),
                OrderDateDisplay = order[2],
                Total = GetDecimalValue("Order Total", order[3], 0),
                TotalDisplay = string.Format("{0:C}", GetDecimalValue("Order Total", order[3], 0)),
                Items = GetIntegerValue("Items", order[4], 0),
                Pieces = GetIntegerValue("Pieces", order[5], 0)
            };
        }
        private OrderDetail BuildDetail(string[] order)
        {
            var detail = new OrderDetail
            {
                OrderNo = order[0],
                LineNo = GetIntegerValue("Line No", order[6], 0),
                ItemNo = GetIntegerValue("Item No", order[7], 0),
                ItemNoDisplay = order[7],
                QtyOrdered = GetIntegerValue("Qty Ordered", order[8], 0),
                QtyShipped = GetIntegerValue("Qty Shipped", order[8], 0),
                Price = GetDecimalValue("Price", order[9], 0),
                PriceDisplay = string.Format("{0:C}", GetDecimalValue("Price", order[9], 0)),
                UPC = order[10],
                Description = order[11],
                UOM = order[12],
                SellUnitsInPurch = order[13],
                Size = order[14],
                Form = order[15],
                CategoryCode = order[16],
                CategoryDesc = order[17],
                SubcategoryCode = order[18],
                SubcategoryDesc = order[19],
                VendorId = order[20],
                VendorName = order[21],
                Status = order[22],
                QOH = GetIntegerValue("QOH", order[23], 0)
            };

            detail.ItemNoDisplayUPC =
                string.IsNullOrWhiteSpace(detail.UPC)
                    ? ""
                    : $"({detail.UPC})";

            detail.SizeDisplay = $"{detail.UOM}/{detail.SellUnitsInPurch}";
            detail.SizeUOM = "/" + detail.UOM;

            detail.IsAvailable =
                detail.Status == "A" &&
                detail.QOH > 0;

            detail.ImageURL =
                $"{Constants.ItemImageUrl}{detail.ItemNo}.jpg";

            return detail;
        }
    }
}