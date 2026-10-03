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
        MenuItem myAccountMenu;

        public AppShell(IAppServices appServices)
        {
            InitializeComponent();

            RegisterRoutes();

            _navigationService = appServices._navigationService;
            _settingService = appServices._settingService;
            _dialogService = appServices._dialogService;
            _cartService = appServices._cartService;
            _customerService = appServices._customerService;

            custMenu = CreateMenuItem("Customers  ›", "\uF500", MenuCustomers_Clicked);
            myAccountMenu = CreateMenuItem("My Account  ›", "\uF007", MenuMyAccount_Clicked);

            Shell.SetNavBarIsVisible(this, false);
        }

        private static MenuItem CreateMenuItem(string text, string glyph, EventHandler clicked)
        {
            var primary = Application.Current?.Resources.TryGetValue("Primary", out var value) == true && value is Color color
                ? color
                : Colors.Teal;
            var menuItem = new MenuItem
            {
                Text = text,
                StyleClass = new List<string> { "MenuItemLayoutStyle" },
                IconImageSource = new FontImageSource
                {
                    FontFamily = "FontAwesomeFreeSolid",
                    Glyph = glyph,
                    Color = primary
                }
            };
            menuItem.Clicked += clicked;
            return menuItem;
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
            SetOptionalMenuItemVisible(custMenu, "Customers", false);
        }

        public void HideMyAccountMenu()
        {
            SetOptionalMenuItemVisible(myAccountMenu, "My Account", false);
        }

        public void ShowCustomerMenu()
        {
            SetOptionalMenuItemVisible(custMenu, "Customers", true);
        }

        public void ShowMyAccountMenu()
        {
            SetOptionalMenuItemVisible(myAccountMenu, "My Account", true);
        }

        private void SetOptionalMenuItemVisible(MenuItem menuItem, string title, bool isVisible)
        {
            for (int index = Items.Count - 1; index >= 0; index--)
            {
                if (ReferenceEquals(Items[index], menuItem) || Items[index].Title?.StartsWith(title, StringComparison.Ordinal) == true)
                    Items.RemoveAt(index);
            }

            if (isVisible)
                Items.Add(menuItem);
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

        public void ShowMainTabs()
        {
            CurrentItem = MainTabBar;
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
        private async void MenuMyAccount_Clicked(object? sender, EventArgs e)
        {
            await _navigationService.GoToAsync(AppRoutes.MyAccount);
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuCustomers_Clicked(object? sender, EventArgs e)
        {
            await _navigationService.GoToAsync(AppRoutes.CustomerList);
            Shell.Current.FlyoutIsPresented = false;
        }
    }
}