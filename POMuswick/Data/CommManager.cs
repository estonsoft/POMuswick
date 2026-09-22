namespace POMuswick.Data
{
    public class CommManager
    {
        ISoapService soapService;

        public CommManager(ISoapService service)
        {
            soapService = service;
        }

        public async Task<string> GetBanners()
        {
            return await soapService.GetBannersAsync();
        }

        public async Task<string> GetCategoriesAndSubcategories()
        {
            return await soapService.GetCategoriesAndSubcategoriesAsync();
        }

        public async Task<string> GetCategoriesAndSubcategoriesCust(string sCust)
        { 
            return await soapService.GetCategoriesAndSubcategoriesCustAsync(sCust);
        }

        public async Task<string> GetItems(String sCustomer, String sDate)
        {
            return await soapService.GetItemsAsync(sCustomer, sDate);
        }

        public async Task<string> GetItemQOH(String sCustomer)
        {
            return await soapService.GetItemQOHAsync(sCustomer);
        }

        public async Task<string> GetItemQOH2(String sUser, String sCustomer)
        {
            return await soapService.GetItemQOH2Async(sUser, sCustomer);
        }

        public async Task<string> ValidateLogin(String sUser, String sPassword, String sDeviceId)
        {
            return await soapService.ValidateLoginAsync(sUser, sPassword, sDeviceId);
        }

        public async Task<string> ValidateUserActive(String sUser)
        {
            return await soapService.ValidateUserActiveAsync(sUser);
        }

        public async Task<string> GetSettings()
        {
            return await soapService.GetSettingsAsync();
        }

        public async Task<string> SubmitOrder(string sCustNo, string sPO, string sPaymentMethod, string sCCInfo, string sOrderInfo, string sDeliveryPickup, string sUser, string sNotes, int iHoldForReview, string sOrderType)
        {
            return await soapService.SubmitOrderAsync(sCustNo, sPO, sPaymentMethod, sCCInfo, sOrderInfo, sDeliveryPickup, sUser, sNotes, iHoldForReview, sOrderType);
        }

        public async Task<string> SubmitOrder2(string sCustNo, string sPO, string sPaymentMethod, string sCCInfo, string sOrderInfo, string sDeliveryPickup, string sUser, string sNotes, int iHoldForReview, string sOrderType)
        {
            return await soapService.SubmitOrder2Async(sCustNo, sPO, sPaymentMethod, sCCInfo, sOrderInfo, sDeliveryPickup, sUser, sNotes, iHoldForReview, sOrderType);
        }

        public async Task<string> ValidateOrderQOH(string sCustNo, string sOrderInfo)
        {
            return await soapService.ValidateOrderQOHAsync(sCustNo, sOrderInfo);
        }

        public async Task<string> GetOrderHistory(string sCustNo)
        {
            return await soapService.GetOrderHistoryAsync(sCustNo);
        }

        public async Task<string> GetSalespersonCustomers(string sUser)
        {
            return await soapService.GetSalespersonCustomersAsync(sUser);
        }

        public async Task<string> GetFlyerItemsPDF()
        {
            return await soapService.GetFlyerItemsPDFAsync();
        }
    }
}
