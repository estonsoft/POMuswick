using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using POMuswick.Common;
using POMuswick.Models;
using POMuswick.Services;
using POMuswick.UIModels;

namespace POMuswick.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private const int HomeItemsPageSize = 20;

        [ObservableProperty]
        public ObservableCollection<UIItems> _cartItems = new();

        [ObservableProperty]
        public ObservableCollection<UIItems> _newItems = new();
        [ObservableProperty]
        private bool _isLoadingHomeItems;

        [ObservableProperty]
        ImageSource? bannerImage = null;

        [ObservableProperty]
        string welcomeTitle = "";
        [ObservableProperty]
        string customerTitle = "";

        [ObservableProperty]
        string searchText = "";

        private readonly INavigationService _navigationService;
        private readonly IBannerService _bannerService;
        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;
        private readonly IItemService _itemService;
        private bool _isActive;
        private bool _isTimerRunning;
        private bool _isLoadingCartPage;
        private bool _isLoadingNewItemsPage;
        private List<Item> _cartItemSource = new();
        private List<Item> _newItemSource = new();
        private AppSettings _homeSettings = new();
        BannerResult? bannerResult;


        public HomeViewModel(IAppServices appServices) : base(appServices)
        {
            Title = PageTitles.Home;
            _navigationService = appServices._navigationService;
            _customerService = appServices._customerService;
            _settingService = appServices._settingService;
            _bannerService = appServices._bannerService;
            _itemService = appServices._itemService;
            BannerImage = ImageSource.FromFile("logo.jpg");
        }

        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            _isActive = true;
            IsLoadingHomeItems = true;
            try
            {
                var appSettings = await _settingService.LoadSetting();
                await SetHomeUIControls(appSettings);
                await LoadHomeItemsAsync(appSettings);
                bannerResult = await _bannerService.GetBannerAsync();
                StartTimer();
            }
            finally
            {
                IsLoadingHomeItems = false;
            }
        }

        public override Task OnDisappearingAsync()
        {
            _isActive = false;
            return base.OnDisappearingAsync();
        }

        private void StartTimer()
        {
            if (_isTimerRunning)
                return;

            _isTimerRunning = true;
            Application.Current?.Dispatcher.StartTimer(
                TimeSpan.FromSeconds(5),
                () =>
                {
                    if (!_isActive)
                    {
                        _isTimerRunning = false;
                        return false;
                    }

                    UpdateBanner();
                    return true;
                });
        }

        private async void UpdateBanner()
        {
            try
            {
                var banners = bannerResult?.banners;
                if (banners == null || banners.Count == 0)
                {
                    BannerImage = ImageSource.FromFile("logo.jpg");
                    return;
                }

                int currentIndex = -1;
                foreach (var banner in banners)
                {
                    if (BannerImage?.ToString()?.Contains(banner.BannerName) == true)
                    {
                        currentIndex = banners.IndexOf(banner);
                        break;
                    }
                }

                for (int offset = 1; offset <= banners.Count; offset++)
                {
                    int nextIndex = (currentIndex + offset) % banners.Count;
                    string? bannerUrl = banners[nextIndex].BannerURL;
                    if (Uri.TryCreate(bannerUrl, UriKind.Absolute, out Uri? uri) &&
                        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                    {
                        BannerImage = ImageSource.FromUri(uri);
                        return;
                    }
                }

                BannerImage = ImageSource.FromFile("logo.jpg");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Banner update failed: {ex}");
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
        private async Task LoadHomeItemsAsync(AppSettings appSettings)
        {
            _homeSettings = appSettings;
            var cartItemsTask = _itemService.LoadItemsAsync();
            var newItemsTask = _itemService.FetchNewItemAsync(appSettings.BlockItemsNoQOH);
            await Task.WhenAll(cartItemsTask, newItemsTask);

            _cartItemSource = (await cartItemsTask)
                .Where(item => item.Status == "A")
                .ToList();
            _newItemSource = (await newItemsTask).items;

            CartItems.Clear();
            NewItems.Clear();
            AppendNextPage(_cartItemSource, CartItems);
            AppendNextPage(_newItemSource, NewItems);
        }

        private void AppendNextPage(List<Item> source, ObservableCollection<UIItems> destination)
        {
            foreach (var item in source.Skip(destination.Count).Take(HomeItemsPageSize))
            {
                var uiItem = item.ToUI();
                UpdateItemDisplayState(uiItem, _homeSettings);
                destination.Add(uiItem);
            }
        }

        [RelayCommand]
        private void LoadMoreCartItems()
        {
            if (_isLoadingCartPage)
                return;

            _isLoadingCartPage = true;
            try
            {
                AppendNextPage(_cartItemSource, CartItems);
            }
            finally
            {
                _isLoadingCartPage = false;
            }
        }

        [RelayCommand]
        private void LoadMoreNewItems()
        {
            if (_isLoadingNewItemsPage)
                return;

            _isLoadingNewItemsPage = true;
            try
            {
                AppendNextPage(_newItemSource, NewItems);
            }
            finally
            {
                _isLoadingNewItemsPage = false;
            }
        }

        private void UpdateItemDisplayState(UIItems item, AppSettings appSettings)
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
                await _navigationService.GoToAsync(AppRoutes.ReorderItems);
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
            IsLoadingHomeItems = true;
            try
            {
                await SyncAppAsync();
                var appSettings = await _settingService.LoadSetting();
                await LoadHomeItemsAsync(appSettings);
                bannerResult = await _bannerService.GetBannerAsync();
            }
            finally
            {
                IsLoadingHomeItems = false;
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task MenuTappedAsync()
        {
            Shell.Current.FlyoutIsPresented = true;
        }

        [RelayCommand]
        private async Task IncreaseQtyAsync(UIItems item)
        {
            if (item == null)
                return;

            if (item.QtyOrder >= 999)
                return;

            if (item.MaxOrderQty > 0 &&
                item.QtyOrder >= item.MaxOrderQty)
                return;

            var newQuantity = item.QtyOrder + 1;
            await _itemService.UpdateItemQtySet(item.ItemNo, newQuantity);

            item.QtyOrder = newQuantity;
            item.IsStepperVisible = true;
            item.IsAddToOrderVisible = false;
        }

        [RelayCommand]
        private async Task DecreaseQtyAsync(UIItems item)
        {
            if (item == null || item.QtyOrder <= 0)
                return;

            var newQuantity = item.QtyOrder - 1;
            await _itemService.UpdateItemQtySet(item.ItemNo, newQuantity);

            item.QtyOrder = newQuantity;

            if (item.QtyOrder == 0)
            {
                item.IsStepperVisible = false;
                item.IsAddToOrderVisible = true;
            }
        }
    }
}