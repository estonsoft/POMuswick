
using POMuswick;

public class OrderHistoryResult
{
    public List<OrderHeader> Headers { get; } = new();

    public List<OrderDetail> Details { get; } = new();
}