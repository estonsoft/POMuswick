
namespace POMuswick.Services
{

    // Resolves each service from the container on first access instead of constructing
    // all of them eagerly, so heavy dependency chains (Database/repositories) are only
    // built when a feature actually uses them.
    public class AppServices : IAppServices
    {
        private readonly IServiceProvider _serviceProvider;

        public AppServices(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public AppLoadingState _appLoadingState => _serviceProvider.GetRequiredService<AppLoadingState>();
        public IAppSyncService _appSyncService => _serviceProvider.GetRequiredService<IAppSyncService>();
        public IBannerService _bannerService => _serviceProvider.GetRequiredService<IBannerService>();
        public ICartService _cartService => _serviceProvider.GetRequiredService<ICartService>();
        public ICatNSubCatService _catNSubCatService => _serviceProvider.GetRequiredService<ICatNSubCatService>();
        public ICustomerService _customerService => _serviceProvider.GetRequiredService<ICustomerService>();
        public IDialogService _dialogService => _serviceProvider.GetRequiredService<IDialogService>();
        public IItemQQHService _itemQQHService => _serviceProvider.GetRequiredService<IItemQQHService>();
        public IItemService _itemService => _serviceProvider.GetRequiredService<IItemService>();
        public ILocationService _locationService => _serviceProvider.GetRequiredService<ILocationService>();
        public ILoginService _loginService => _serviceProvider.GetRequiredService<ILoginService>();

        public INavigationService _navigationService => _serviceProvider.GetRequiredService<INavigationService>();
        public IOrderHistoryService _orderHistoryService => _serviceProvider.GetRequiredService<IOrderHistoryService>();
        public IPDFFilesService _pDFFilesService => _serviceProvider.GetRequiredService<IPDFFilesService>();
        public ISalesPersonCustomersService _salesPersonCustomersService => _serviceProvider.GetRequiredService<ISalesPersonCustomersService>();
        public ISettingService _settingService => _serviceProvider.GetRequiredService<ISettingService>();
        public ISubmitOrderService _submitOrderService => _serviceProvider.GetRequiredService<ISubmitOrderService>();
    }

    public interface IAppServices
    {
        public AppLoadingState _appLoadingState { get; }
        public IAppSyncService _appSyncService { get; }
        public IBannerService _bannerService { get; }
        public ICartService _cartService { get; }
        public ICatNSubCatService _catNSubCatService { get; }
        public ICustomerService _customerService { get; }
        public IDialogService _dialogService { get; }
        public IItemQQHService _itemQQHService { get; }
        public IItemService _itemService { get; }
        public ILocationService _locationService { get; }
        public ILoginService _loginService { get; }

        public INavigationService _navigationService { get; }
        public IOrderHistoryService _orderHistoryService { get; }
        public IPDFFilesService _pDFFilesService { get; }
        public ISalesPersonCustomersService _salesPersonCustomersService { get; }
        public ISettingService _settingService { get; }
        public ISubmitOrderService _submitOrderService { get; }
    }
}