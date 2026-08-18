namespace POMuswick.Data
{
    public class CommManager
    {
        private readonly ISoapService soapService;

        public CommManager(ISoapService service)
        {
            soapService = service;
        }

        public async Task GetSettings()
        {
            await App.UpdateProgress(0, "Downloading Settings");

            string response = await soapService.GetSettingsAsync();

            await App.UpdateProgress(2, "Saving Settings");

            await XMLResponseParser.commService_GetSettingsCompletedAsync(response);

            await App.UpdateProgress(5, "Settings completed");
        }

        public async Task GetBanners()
        {
            await App.UpdateProgress(5, "Downloading banner data");

            string banner = await soapService.GetBannersAsync();

            await App.UpdateProgress(7, "Saving banner data");

            await XMLResponseParser.commService_GetBannersCompleted(banner);

            await App.UpdateProgress(10, "Banner data completed");
        }

        public async Task GetCategoriesAndSubcategories()
        {
            await App.UpdateProgress(10, "Downloading Categories data");

            string response =
                await soapService.GetCategoriesAndSubcategoriesAsync();

            await App.UpdateProgress(12, "Saving Categories data");

            await XMLResponseParser
                .commService_GetCategoriesAndSubcategoriesCompleted(response);

            await App.UpdateProgress(15, "Categories data completed");
        }

        public async Task GetCategoriesAndSubcategoriesCust(string sCust)
        {
            await App.UpdateProgress(10, "Downloading Categories data");

            string response =
                await soapService.GetCategoriesAndSubcategoriesCustAsync(sCust);

            await App.UpdateProgress(12, "Saving Categories data");

            await XMLResponseParser
                .commService_GetCategoriesAndSubcategoriesCustCompleted(response);

            await App.UpdateProgress(15, "Categories data completed");
        }

        public async Task GetItems(string sCustomer, string sDate)
        {
            await App.UpdateProgress(20, "Downloading Items data");

            string response =
                await soapService.GetItemsAsync(sCustomer, sDate);

            await App.UpdateProgress(37, "Saving Items data");

            await XMLResponseParser
                .commService_GetItemsCompletedAsync(response);
        }

        public async Task GetOrderHistory(string sCustNo)
        {
            await App.UpdateProgress(70, "Downloading Order History");

            string response =
                await soapService.GetOrderHistoryAsync(sCustNo);

            await App.UpdateProgress(75, "Saving Order History");

            await XMLResponseParser
                .commService_GetOrderHistoryCompletedAsync(response);

            await App.UpdateProgress(80, "Order History completed");
        }

        public async Task GetItemQOH(string sCustomer)
        {
            await App.UpdateProgress(60, "Downloading Item quantity");

            string response =
                await soapService.GetItemQOHAsync(sCustomer);

            await App.UpdateProgress(65, "Saving Item quantity");

            await XMLResponseParser
                .commService_GetItemQOHCompletedAsync(response);

            await App.UpdateProgress(68, "Item quantity completed");
        }

        public async Task GetItemQOH2(string sUser, string sCustomer)
        {
            await App.UpdateProgress(87, "Downloading Item quantity");

            string response =
                await soapService.GetItemQOH2Async(sUser, sCustomer);

            await App.UpdateProgress(95, "Saving Item quantity");

            await XMLResponseParser
                .commService_GetItemQOH2CompletedAsync(response);

            await App.UpdateProgress(100, "Data sync completed");
        }

        public async Task ValidateLogin(
            string sUser,
            string sPassword,
            string sDeviceId)
        {
            string response =
                await soapService.ValidateLoginAsync(
                    sUser,
                    sPassword,
                    sDeviceId);

            await XMLResponseParser
                .commService_ValidateLoginCompletedAsync(response);
        }

        public async Task ValidateUserActive(string sUser)
        {
            string response =
                await soapService.ValidateUserActiveAsync(sUser);

            await XMLResponseParser
                .commService_ValidateUserActiveCompletedAsync(response);
        }

        public async Task SubmitOrder(
            string sCustNo,
            string sPO,
            string sPaymentMethod,
            string sCCInfo,
            string sOrderInfo,
            string sDeliveryPickup,
            string sUser,
            string sNotes,
            int iHoldForReview,
            string sOrderType)
        {
            string response =
                await soapService.SubmitOrderAsync(
                    sCustNo,
                    sPO,
                    sPaymentMethod,
                    sCCInfo,
                    sOrderInfo,
                    sDeliveryPickup,
                    sUser,
                    sNotes,
                    iHoldForReview,
                    sOrderType);

            Console.WriteLine("SubmitOrder response: " + response);

            await XMLResponseParser
                .commService_SubmitOrderCompletedAsync(response);
        }

        public async Task SubmitOrder2(
            string sCustNo,
            string sPO,
            string sPaymentMethod,
            string sCCInfo,
            string sOrderInfo,
            string sDeliveryPickup,
            string sUser,
            string sNotes,
            int iHoldForReview,
            string sOrderType)
        {
            string response =
                await soapService.SubmitOrder2Async(
                    sCustNo,
                    sPO,
                    sPaymentMethod,
                    sCCInfo,
                    sOrderInfo,
                    sDeliveryPickup,
                    sUser,
                    sNotes,
                    iHoldForReview,
                    sOrderType);

            Console.WriteLine("SubmitOrder2 response: " + response);

            await XMLResponseParser
                .commService_SubmitOrder2CompletedAsync(response);
        }

        public async Task<ValidateResponse> ValidateOrderQOH(
            string sCustNo,
            string sOrderInfo)
        {
            string response =
                await soapService.ValidateOrderQOHAsync(
                    sCustNo,
                    sOrderInfo);

            return await XMLResponseParser
                .commService_ValidateOrderQOHCompletedAsync(response);
        }

        public async Task GetSalespersonCustomers(string sUser)
        {
            string response =
                await soapService.GetSalespersonCustomersAsync(sUser);

            await XMLResponseParser
                .commService_GetSalespersonCustomersCompletedAsync(response);
        }

        public async Task GetFlyerItemsPDF()
        {
            string response =
                await soapService.GetFlyerItemsPDFAsync();

            await XMLResponseParser
                .commService_GetFlyerItemsPDFCompleted(response);
        }
    }
}