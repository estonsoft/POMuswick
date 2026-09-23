using POMuswick.Models;
using POMuswick.Services;
using POMuswick.Views;

namespace POMuswick
{
    public partial class AppShell : Shell
    {
        private static bool _routesRegistered;
        private static readonly object RouteRegistrationLock = new();
        private readonly INavigationService _navigationService;
        private readonly ISettingService _settingService;
        private readonly IDialogService _dialogService;
        private readonly ICartService _cartService;
        private readonly ICustomerService _customerService;
        MenuItem custMenu;
        Boolean bIsCustMenuVisible = false;
        MenuItem myAccountMenu;
        Boolean bIsMyAccountMenuVisible = false;

        public AppShell(IAppServices appServices)
        {
            InitializeComponent();

            RegisterRoutes();

            _navigationService = appServices._navigationService;
            _settingService = appServices._settingService;
            _dialogService = appServices._dialogService;
            _cartService = appServices._cartService;
            _customerService = appServices._customerService;

            custMenu = MenuCustomers;
            myAccountMenu = MenuMyAccount;

            Shell.SetNavBarIsVisible(this, false);
        }

        private void RegisterRoutes()
        {
            lock (RouteRegistrationLock)
            {
                if (_routesRegistered)
                    return;

                Routing.RegisterRoute(nameof(ItemSearchPage), typeof(ItemSearchPage));
                Routing.RegisterRoute(AppRoutes.Categories, typeof(CategoryPage));
                Routing.RegisterRoute(nameof(SubcategoryPage), typeof(SubcategoryPage));
                Routing.RegisterRoute(nameof(MyAccountPage), typeof(MyAccountPage));
                Routing.RegisterRoute(nameof(ShoppingCartPage), typeof(ShoppingCartPage));
                Routing.RegisterRoute(nameof(CheckoutPage), typeof(CheckoutPage));
                Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
                Routing.RegisterRoute(nameof(SubmitOrderPage), typeof(SubmitOrderPage));
                Routing.RegisterRoute(nameof(PurchaseHistoryPage), typeof(PurchaseHistoryPage));
                Routing.RegisterRoute(nameof(PurchaseHistoryDetailPage), typeof(PurchaseHistoryDetailPage));
                Routing.RegisterRoute(nameof(ReorderItemsPage), typeof(ReorderItemsPage));
                Routing.RegisterRoute(nameof(QuickEntryPage), typeof(QuickEntryPage));
                Routing.RegisterRoute(nameof(CustomerListPage), typeof(CustomerListPage));
                _routesRegistered = true;
            }
        }

        public void HideCustomerMenu()
        {
            for (int index = Items.Count - 1; index >= 0; index--)
            {
                if (ReferenceEquals(Items[index], custMenu) || Items[index].Title == "Customers")
                {
                    Items.RemoveAt(index);
                }
            }
            bIsCustMenuVisible = false;
        }

        public void HideMyAccountMenu()
        {
            foreach (ShellItem item in Items)
            {
                if (item.Title == "My Account")
                {
                    Items.Remove(item);
                    break;
                }
            }
            bIsMyAccountMenuVisible = false;
        }

        public void ShowCustomerMenu()
        {
            if (!bIsCustMenuVisible && !Items.Contains(custMenu))
            {
                bIsCustMenuVisible = true;
                Items.Add(custMenu);
            }
        }

        public void ShowMyAccountMenu()
        {
            if (!bIsMyAccountMenuVisible)
            {
                bIsMyAccountMenuVisible = true;
                Items.Add(myAccountMenu);
            }
        }

        public async Task Logout()
        {
            bool logout = await _dialogService.ConfirmAsync(
                "Logout",
                "Are you sure you want to logout?");

            if (!logout)
                return;
            else
            {
                AppSettings appSettings = await _settingService.LoadSetting();
                if (appSettings.IsSalesUser)
                {
                    await _cartService.SuspendCartItems(appSettings.CustomerNo);
                    await _cartService.ClearCartItems();
                }
                await _customerService.ClearCustomer();
                await _settingService.SaveChanges(new Dictionary<string, string>
                {
                    [nameof(AppSettings.IsLoggedIn)] = "0"
                });
                await _navigationService.GoToRootAsync(AppRoutes.Login);
            }
        }

        public void ShowNavBar()
        {
            SetNavBarIsVisible(this, true);
        }

        public void SetCartTabCount(int count)
        {
            CartTab.Title = count > 0 ? $"Cart ({count})" : "Cart";
        }

        private async void MenuShoppingCart_Clicked(object sender, EventArgs e)
        {
            await _navigationService.GoToAsync(AppRoutes.ShoppingCart);
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuScanBarcode_Clicked(object sender, EventArgs e)
        {
            await _navigationService.GoToAsync(AppRoutes.QuickEntry);
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuMyPurchases_Clicked(object sender, EventArgs e)
        {
            await _navigationService.GoToAsync(AppRoutes.PurchaseHistory);
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuCategories_Clicked(object sender, EventArgs e)
        {
            await _navigationService.GoToAsync(AppRoutes.Categories);
            Shell.Current.FlyoutIsPresented = false;
        }

        private async void MenuLogout_Clicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;
            AppSettings appSettings = await _settingService.LoadSetting();
            if (!appSettings.IsLoggedIn)
            {
                await _navigationService.GoToRootAsync(AppRoutes.Login);
                return;
            }
            await Logout();
        }
        private async void MenuMyAccount_Clicked(object sender, EventArgs e)
        {
            await _navigationService.GoToAsync(AppRoutes.MyAccount);
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuCustomers_Clicked(object sender, EventArgs e)
        {
            await _navigationService.GoToAsync(AppRoutes.CustomerList);
            Shell.Current.FlyoutIsPresented = false;
        }
    }
}