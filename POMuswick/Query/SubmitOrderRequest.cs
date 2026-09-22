public class SubmitOrderRequest
{
    public string CustomerNo { get; set; } = string.Empty;

    public string PurchaseOrder { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;

    public string CreditCardInfo { get; set; } = string.Empty;

    public string OrderInfo { get; set; } = string.Empty;

    public string DeliveryPickup { get; set; } = string.Empty;

    public string User { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public bool HoldForReview { get; set; }

    public string OrderType { get; set; } = string.Empty;
}