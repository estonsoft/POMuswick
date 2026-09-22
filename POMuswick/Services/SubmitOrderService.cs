using POMuswick.Data;
using POMuswick.Parsers;
using POMuswick.Repository;
namespace POMuswick.Services
{
    public class SubmitOrderService : ISubmitOrderService
    {
        private readonly CommManager _comm;
        private readonly SubmitOrderParser _parser;

        private readonly ItemQQHParser _itemQQHParser;

        private readonly IItemQQHRepository _itemQQHRepository;
        private readonly ICustomerRepository _customerRepository;

        public SubmitOrderService(
            CommManager comm,
            SubmitOrderParser parser,
            ItemQQHParser itemQQHParser,
            IItemQQHRepository itemQQHRepository,
            ICustomerRepository customerRepository)
        {
            _comm = comm;
            _parser = parser;
            _itemQQHParser = itemQQHParser;
            _itemQQHRepository = itemQQHRepository;
            _customerRepository = customerRepository;
        }

        public async Task<SubmitOrderResult> SubmitOrderAsync(SubmitOrderRequest request)
        {
            int hold = request.HoldForReview ? 1 : 0;
            var response = await _comm.SubmitOrder(
                sCustNo: request.CustomerNo,
                sPO: request.PurchaseOrder,
                sPaymentMethod: request.PaymentMethod,
                sCCInfo: request.CreditCardInfo,
                sOrderInfo: request.OrderInfo,
                sDeliveryPickup: request.DeliveryPickup,
                sUser: request.User,
                sNotes: request.Notes,
                iHoldForReview: hold,
                sOrderType: request.OrderType);
            SubmitOrderResult result = await _parser.Parse(response);
            return result;
        }

        public async Task<SubmitOrderResult> SubmitOrder2Async(SubmitOrderRequest request)
        {
            int hold = request.HoldForReview ? 1 : 0;
            var response = await _comm.SubmitOrder2(
                sCustNo: request.CustomerNo,
                sPO: request.PurchaseOrder,
                sPaymentMethod: request.PaymentMethod,
                sCCInfo: request.CreditCardInfo,
                sOrderInfo: request.OrderInfo,
                sDeliveryPickup: request.DeliveryPickup,
                sUser: request.User,
                sNotes: request.Notes,
                iHoldForReview: hold,
                sOrderType: request.OrderType);

            SubmitOrderResult result = await _parser.Parse(response);
            return result;
        }

        public async Task<SubmitOrderResult> ValidateOrderAsync(string orderInfo)
        {
            var custNo = _customerRepository.Load().CustNo;
            var response = await _comm.ValidateOrderQOH(custNo, orderInfo);//"Cusomter","Order"

            SubmitOrderResult submitOrderResult = await _parser.ValidateParse(response);
            if(submitOrderResult.validateResponse.IsValid && submitOrderResult.Status.Trim().Length > 0 )
            {
                ItemQQHResult result = await _itemQQHParser.Parse(submitOrderResult.Status);
                _itemQQHRepository.UpdateQQH(result.itemsQQH);
                _itemQQHRepository.UpdateItemQtySet(result.itemsQQH);
            }
            return submitOrderResult;
        }
    }

        

    public interface ISubmitOrderService
    {
        public Task<SubmitOrderResult> ValidateOrderAsync(string orderInfo);
        public Task<SubmitOrderResult> SubmitOrderAsync(SubmitOrderRequest submitOrderRequest);
        public Task<SubmitOrderResult> SubmitOrder2Async(SubmitOrderRequest submitOrderRequest);
    }
}