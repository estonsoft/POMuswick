using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Common;
using POMuswick.Models;
using POMuswick.Services;
using POMuswick.UIModels;

namespace POMuswick.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        [ObservableProperty]
        List<UIItems> lstItems;

        [ObservableProperty]
        ImageSource bannerImage;

        [ObservableProperty]
        string welcomeTitle;
        [ObservableProperty]
        string customerTitle;

        [ObservableProperty]
        string searchText;

        private readonly INavigationService _navigationService;
        private readonly IBannerService _bannerService;
        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;
        private readonly IItemService _itemService;
        private readonly IAppSyncService _appSyncService;

        public HomeViewModel(IAppServices appServices) : base(appServices)
        {
            Title = PageTitles.Home;
            _navigationService = appServices._navigationService;
            _customerService = appServices._customerService;
            _settingService = appServices._settingService;
            _bannerService = appServices._bannerService;
            _itemService = appServices._itemService;
            _appSyncService = appServices._appSyncService;
        }

        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            var appSetting = await _settingService.LoadSetting();
            await SetHomeUIControls(appSetting);
            await RefreshNewItemsList(appSetting);
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            BannerImage = ImageSource.FromFile("logo.jpg");
            Application.Current?.Dispatcher.StartTimer(
                TimeSpan.FromSeconds(5),
                () =>
                {
                    UpdateBanner();
                    return true;
                });
        }

        private async void UpdateBanner()
        {
            var bannerResult = await _bannerService.GetBannerAsync();
            var banners = bannerResult.banners;
            if (banners.Count == 0)
            {
                {
                    BannerImage = ImageSource.FromFile("logo.jpg");
                    return;
                }
            }
            int iNextIndex = 0;

            foreach (var b in banners)
            {
                iNextIndex++;

                if (BannerImage.ToString().Contains(b.BannerName))
                {
                    break;
                }
            }

            if (iNextIndex >= banners.Count)
            {
                iNextIndex = 0;
            }
            try
            {
                Banner banner = banners[iNextIndex];

                BannerImage = ImageSource.FromUri(new Uri(banner.BannerURL));
            }
            catch(Exception e)
            {
                Console.WriteLine("Banner Exception"+e.Message);
            }      
        }

        public async Task SetHomeUIControls(AppSettings appSettings)
        {
            var customer = await _customerService.GetCustomerAsync();
            WelcomeTitle = "Welcome - " + appSettings.UserName;
            CustomerTitle = customer.CustNo != "0" ? $"{customer.CustNo} - {customer.CompanyName}" : string.Empty;


            await _navigationService.HideCustomerMenu();
            await _navigationService.HideMyAccountMenu();

            if (appSettings.IsSalesUser)
                await _navigationService.ShowCustomerMenu();
            else
                await _navigationService.ShowMyAccountMenu();
        }
        public async Task RefreshNewItemsList(AppSettings appSettings)
        {
            LstItems = null;
            var itemResult = await _itemService.FetchNewItemAsync(true);
            foreach (var item in itemResult.items)
            {
                UpdateItemDisplayState(item, appSettings);
            }
            List<UIItems> uiList = itemResult.items
            .Select(x => x.ToUI())
            .ToList();
            LstItems = uiList;
        }

        private void UpdateItemDisplayState(Item item, AppSettings appSettings)
        {
            bool hasOrder = item.QtyOrder > 0;
            bool hasStock = item.QOH > 0;
            bool blockNoStock = appSettings.BlockItemsNoQOH;

            item.IsLoggedIn = appSettings.IsLoggedIn;

            // Order
            item.IsStepperVisible = hasOrder;
            item.IsAddToOrderVisible = !hasOrder;

            // Stock Display
            item.IsQOHVisible = false;
            item.IsQOHBlackVisible = false;
            item.IsQOHRedVisible = false;
            item.IsInStockVisible = false;
            item.IsOutOfStockVisible = false;

            switch (appSettings.QOHDisplay)
            {
                case "Q":
                    item.IsQOHVisible = true;
                    item.IsQOHBlackVisible = hasStock;
                    item.IsQOHRedVisible = !hasStock;
                    break;

                case "I":
                    item.IsInStockVisible = hasStock;
                    item.IsOutOfStockVisible = !hasStock;
                    break;
            }

            item.IsStockRowVisible =
                item.IsQOHVisible ||
                item.IsInStockVisible ||
                item.IsOutOfStockVisible;

            // Max Order Qty
            bool showMaxQty = item.MaxOrderQty > 0 &&
                              item.MaxOrderQty < 9999;

            item.IsMaxOrderQtyVisible = showMaxQty;

            if (showMaxQty)
                item.MaxOrderQtyDisplay = $"Max {item.MaxOrderQty}";
            if (item.ItemNo == 89770)
                item.MaxOrderQtyDisplay = item.MaxOrderQtyDisplay;
            // Block items with no stock
            if (blockNoStock && !hasStock)
            {
                item.IsStepperVisible = false;
                item.IsAddToOrderVisible = false;
                item.IsMaxOrderQtyVisible = false;
            }
        }

        [RelayCommand]
        public async Task ShopNowAsync()
        {
            await _navigationService.GoToAsync(AppRoutes.ItemSearch);
        }

        [RelayCommand]
        public async Task NewItemsAllAsync()
        {
            await _navigationService.GoToAsync(AppRoutes.ItemSearch, new ShellNavigationQueryParameters
        {
            { "NEW ITEMS", "NEW ITEMS" }
        });
        }
        [RelayCommand]
        public async Task PastPurchasesAsync()
        {
            var reorderItemsResult = await _itemService.FetchReorderItemsAsync();
            int iReorderItems = reorderItemsResult.items.Count;

            if (iReorderItems == 0)
            {
                await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Past purchases not found", "Ok");
            }
            else
            {
                await _navigationService.GoToAsync(AppRoutes.ItemSearch);
            }
        }

        [RelayCommand]
        public async Task SearchTappedAsync()
        {
            await _navigationService.GoToAsync(AppRoutes.ItemSearch, new ShellNavigationQueryParameters
            {
                { "SearchText", SearchText }
            });
        }

        [RelayCommand]
        public async Task RefreshDataAsync()
        {
            IsBusy = true;
            await _appSyncService.SyncApp();
            IsBusy = false;
        }

        [RelayCommand]
        public async Task MenuTappedAsync()
        {
            Shell.Current.FlyoutIsPresented = true;
        }
    }
}