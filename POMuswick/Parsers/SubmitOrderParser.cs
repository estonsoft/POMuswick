using System.Collections.Concurrent;
using POMuswick.Data;
using POMuswick.Models;

namespace POMuswick.Parsers
{
    public class SubmitOrderParser : BaseParser
    {
        public SubmitOrderParser()
        {

        }
        public async Task<SubmitOrderResult> Parse(string response)
        {
            Console.WriteLine("Get SubmitOrders returned");
            SubmitOrderResult submitOrderResult = new SubmitOrderResult();
            if (response == "V")
            {
                ValidateResponse validateResponse = new ValidateResponse()
                {
                    IsValid = true,
                    Message = "Order is valid."
                };
                submitOrderResult.Status = response;
                submitOrderResult.validateResponse = validateResponse;
            }
            else if (response.StartsWith("F~"))
            {
                ValidateResponse validateResponse = new ValidateResponse()
                {
                    IsValid = false,
                    Message = "Some items in your cart are now out of stock.  Please review your shopping cart."
                };
                submitOrderResult.Status = response;
                submitOrderResult.validateResponse = validateResponse;
            }
            else
            {
                ValidateResponse validateResponse = new ValidateResponse()
                {
                    IsValid = false,
                    Message = "Error in submitting order.  Please try again."
                };
                submitOrderResult.Status = string.Empty;
                submitOrderResult.validateResponse = validateResponse;
            }
            return submitOrderResult;
        }

        public async Task<SubmitOrderResult> ValidateParse(string response)
        {
            Console.WriteLine("Get SubmitOrders returned");
            SubmitOrderResult submitOrderResult = new SubmitOrderResult();
            if (response == "V")
            {
                ValidateResponse validateResponse = new ValidateResponse()
                {
                    IsValid = true,
                    Message = "Order is valid."
                };
                submitOrderResult.Status = response;
                submitOrderResult.validateResponse = validateResponse;
            }
            else if (response.StartsWith("F~"))
            {
                ValidateResponse validateResponse = new ValidateResponse()
                {
                    IsValid = false,
                    Message = "Some items in your cart are now out of stock.  Please review your shopping cart."
                };
                submitOrderResult.Status = response;
                submitOrderResult.validateResponse = validateResponse;
            }
            else
            {
                ValidateResponse validateResponse = new ValidateResponse()
                {
                    IsValid = false,
                    Message = "Error validating order.  Please try again."
                };
                submitOrderResult.Status = string.Empty;
                submitOrderResult.validateResponse = validateResponse;
            }

            return submitOrderResult;
        }
    }
}