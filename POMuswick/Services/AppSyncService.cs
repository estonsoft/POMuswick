using POMuswick.Models;
namespace POMuswick.Services
{
    public class AppSyncService : IAppSyncService
    {
        private readonly IBannerService _bannerService;
        private readonly IOrderHistoryService _orderHistoryService;
        private readonly ISettingService _settingService;
        private readonly IItemQQHService _itemQQHService;

        private readonly IItemService _itemService;
        private readonly ICatNSubCatService _catNSubCatService;
        private readonly ISalesPersonCustomersService _salesPersonCustomersService;
        private readonly ICartService _cartService;

        public AppSyncService(IBannerService bannerService,IOrderHistoryService orderHistoryService,ISettingService settingService,
            ICatNSubCatService catNSubCatService,IItemQQHService itemQQHService,ISalesPersonCustomersService salesPersonCustomersService,
            ICartService cartService,IItemService itemService)
        {
            _bannerService = bannerService;
            _orderHistoryService = orderHistoryService;
            _settingService = settingService;
            _catNSubCatService = catNSubCatService;
            _itemQQHService = itemQQHService;
            _salesPersonCustomersService = salesPersonCustomersService;
            _cartService = cartService;
            _itemService = itemService;
        }

        public async Task<AppSyncResult> SyncApp(string selectedCustomer)
        {
            Location location = new Location();
            location.Refresh();
            AppSettings appSettings = await _settingService.LoadSetting();
            if (appSettings.IsLoggedIn)
            {
                await _cartService.RestoreCart();
            }
            AppSyncResult appSyncResult = new AppSyncResult();
            appSyncResult.bannerResult = await _bannerService.FetchBannerAsync();
            appSyncResult.orderHistoryResult = await _orderHistoryService.FetchOrderHistoryAsync();
            appSyncResult.settingResult = await _settingService.FetchSettingAsync();
            appSyncResult.catNSubcatResult = await _catNSubCatService.GetCategoriesAndSubCategories(selectedCustomer);
            appSyncResult.itemResult = await _itemService.FetchItemAsync();
            appSyncResult.itemQQHResult = await _itemQQHService.FetchItemQQH2Async();
            appSyncResult.salesPersonCustomersResult = await _salesPersonCustomersService.FetchSalesPersonCustomersAsync("");
            return appSyncResult;
        }
    }

    public interface IAppSyncService
    {
        public Task<AppSyncResult> SyncApp(string selectedCustomer = "0");
    }
}