
namespace POMuswick.Services
{

    public class AppServices : IAppServices
    {
        public IAppSyncService _appSyncService{ get; }
        public  IBannerService _bannerService{ get; }
        public  ICartService _cartService{ get; }
        public  ICatNSubCatService _catNSubCatService{ get; }
        public  ICustomerService _customerService{ get; }
        public  IDialogService _dialogService{ get; }
        public  IItemQQHService _itemQQHService{ get; }
        public  IItemService _itemService{ get; }
        public  ILocationService _locationService{ get; }
        public  ILoginService _loginService{ get; }

        public  INavigationService _navigationService{ get; }
        public  IOrderHistoryService _orderHistoryService{ get; }
        public  IPDFFilesService _pDFFilesService{ get; }
        public  ISalesPersonCustomersService _salesPersonCustomersService{ get; }
        public  ISettingService _settingService{ get; }
        public  ISubmitOrderService _submitOrderService{ get; }



        public AppServices(
            IAppSyncService appSyncService,
            IBannerService bannerService,
            ICartService cartService,
            ICatNSubCatService catNSubCatService,
            ICustomerService customerService,
            IDialogService dialogService,
            IItemQQHService itemQQHService,
            IItemService itemService,
            ILocationService locationService,
            ILoginService loginService,
            INavigationService navigationService,
            IOrderHistoryService orderHistoryService,
            IPDFFilesService pDFFilesService,
            ISalesPersonCustomersService salesPersonCustomersService,
            ISettingService settingService,
            ISubmitOrderService submitOrderService
        )
        {
            _appSyncService = appSyncService;
            _bannerService = bannerService;
            _cartService = cartService;
            _catNSubCatService = catNSubCatService;
            _customerService = customerService;
            _dialogService = dialogService;
            _itemQQHService = itemQQHService;
            _itemService = itemService;
            _locationService = locationService;
            _loginService = loginService;
            _navigationService = navigationService;
            _orderHistoryService = orderHistoryService;
            _pDFFilesService = pDFFilesService;
            _salesPersonCustomersService = salesPersonCustomersService;
            _settingService = settingService;
            _submitOrderService = submitOrderService;
        }
    }

    public interface IAppServices
    {
         public IAppSyncService _appSyncService{ get; }
        public  IBannerService _bannerService{ get; }
        public  ICartService _cartService{ get; }
        public  ICatNSubCatService _catNSubCatService{ get; }
        public  ICustomerService _customerService{ get; }
        public  IDialogService _dialogService{ get; }
        public  IItemQQHService _itemQQHService{ get; }
        public  IItemService _itemService{ get; }
        public  ILocationService _locationService{ get; }
        public  ILoginService _loginService{ get; }

        public  INavigationService _navigationService{ get; }
        public  IOrderHistoryService _orderHistoryService{ get; }
        public  IPDFFilesService _pDFFilesService{ get; }
        public  ISalesPersonCustomersService _salesPersonCustomersService{ get; }
        public  ISettingService _settingService{ get; }
        public  ISubmitOrderService _submitOrderService{ get; }
    }
}