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

        public AppSyncService(IBannerService bannerService, IOrderHistoryService orderHistoryService, ISettingService settingService,
            ICatNSubCatService catNSubCatService, IItemQQHService itemQQHService, ISalesPersonCustomersService salesPersonCustomersService,
            ICartService cartService, IItemService itemService)
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

        public async Task<AppSyncResult> SyncApp(string selectedCustomer, IProgress<SyncProgress>? progress = null)
        {
            Location location = new Location();
            location.Refresh();
            progress?.Report(new SyncProgress("Loading settings", 0));
            await _settingService.SaveChanges(new Dictionary<string, string>
            {
                [nameof(AppSettings.CustomerNo)] = selectedCustomer
            });
            AppSettings appSettings = await _settingService.LoadSetting();
            if (appSettings.IsLoggedIn)
            {
                AppSyncResult appSyncResult = new AppSyncResult();

                progress?.Report(new SyncProgress("Loading settings", 10));
                appSyncResult.settingResult = await _settingService.FetchSettingAsync();

                progress?.Report(new SyncProgress("Loading banners", 20));
                appSyncResult.bannerResult = await _bannerService.FetchBannerAsync();

                progress?.Report(new SyncProgress("Loading categories", 50));
                appSyncResult.catNSubcatResult = await _catNSubCatService.GetCategoriesAndSubCategories(selectedCustomer);

                progress?.Report(new SyncProgress("Loading items", 60));
                appSyncResult.itemResult = await _itemService.FetchItemAsync();

                progress?.Report(new SyncProgress("Loading item quantities", 70));
                appSyncResult.itemQQHResult = await _itemQQHService.FetchItemQQH2Async();

                progress?.Report(new SyncProgress("Loading order history", 70));
                appSyncResult.orderHistoryResult = await _orderHistoryService.FetchOrderHistoryAsync();

                progress?.Report(new SyncProgress("Loading customers", 85));
                appSyncResult.salesPersonCustomersResult = await _salesPersonCustomersService.FetchSalesPersonCustomersAsync("");

                progress?.Report(new SyncProgress("Restoring cart", 95));
                await _cartService.RestoreCart();

                progress?.Report(new SyncProgress("Sync complete", 100));
                return appSyncResult;
            }
            else
            {
                progress?.Report(new SyncProgress("User not logged in", 100));
                return new AppSyncResult();
            }
        }
    }

    public sealed record SyncProgress(string Status, int Percentage);

    public interface IAppSyncService
    {
        public Task<AppSyncResult> SyncApp(string selectedCustomer = "0", IProgress<SyncProgress>? progress = null);
    }
}