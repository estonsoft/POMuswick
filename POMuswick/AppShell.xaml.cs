using POMuswick.Views;

namespace POMuswick
{
    public partial class AppShell : Shell
    {

        MenuItem custMenu;
        Boolean bIsCustMenuVisible = false;
        MenuItem myAccountMenu;
        Boolean bIsMyAccountMenuVisible = false;


        public AppShell()
        {
            InitializeComponent();

            App.g_Shell = this;

            //LogoURL = Constants.LogoUrl;

            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(DeliveryOptionsPage), typeof(DeliveryOptionsPage));
            Routing.RegisterRoute(nameof(LocationsPage), typeof(LocationsPage));
            Routing.RegisterRoute(nameof(ItemSearchPage), typeof(ItemSearchPage));
            Routing.RegisterRoute(nameof(CategoryPage), typeof(CategoryPage));
            Routing.RegisterRoute(nameof(SubcategoryPage), typeof(SubcategoryPage));
            Routing.RegisterRoute(nameof(MyAccountPage), typeof(MyAccountPage));
            Routing.RegisterRoute(nameof(ShoppingCartPage), typeof(ShoppingCartPage));
            Routing.RegisterRoute(nameof(CheckoutPage), typeof(CheckoutPage));
            Routing.RegisterRoute(nameof(SubmitOrderPage), typeof(SubmitOrderPage));
            Routing.RegisterRoute(nameof(PaymentMethodPage), typeof(PaymentMethodPage));
            Routing.RegisterRoute(nameof(PurchaseHistoryPage), typeof(PurchaseHistoryPage));
            Routing.RegisterRoute(nameof(PurchaseHistoryDetailPage), typeof(PurchaseHistoryDetailPage));
            Routing.RegisterRoute(nameof(ReorderItemsPage), typeof(ReorderItemsPage));
            Routing.RegisterRoute(nameof(QuickEntryPage), typeof(QuickEntryPage));
            Routing.RegisterRoute(nameof(CustomerListPage), typeof(CustomerListPage));

            custMenu = MenuCustomers;
            myAccountMenu = MenuMyAccount;

            Shell.SetTabBarIsVisible(this, false);
            Shell.SetNavBarIsVisible(this, false);

            if (App.g_IsLoggedIn)
            {
                ValidateUserActive();
            }
        }

        public static async Task<String> ValidateUserActive()
        {
            try
            {
                App.CommManager.ValidateUserActive(App.g_UserName);
            }
            catch { }

            return "";
        }

        public void HideCustomerMenu()
        {
            foreach (ShellItem item in Items)
            {
                if (item.Title == "Customers")
                {
                    Items.Remove(item);
                    break;
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

        public void ShowLoggedInMenu()
        {
            foreach (ShellItem item in Items)
            {
                if (item.Title == "Login")
                {
                    //Items.Remove(item);
                    break;
                }
            }
        }

        public void ShowCustomerMenu()
        {
            if (!bIsCustMenuVisible)
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

        public void Logout()
        {
            App.g_db.SaveSetting("LoggedIn", "0");
            App.g_IsLoggedIn = false;
            App.g_Shell.GoToLogin();
        }

        public async Task<int> GoToHome()
        {
            try
            {
                App.g_HeaderTitle = "Muswick Wholesale Grocers";
                await Current.GoToAsync("//HomePage");
            }
            catch
            {
                await Navigation.PopToRootAsync();
            }
            return 0;
        }
        public async Task<int> GoToShoppingCart()
        {
            App.g_HeaderTitle = "Shopping Cart";
            await Current.GoToAsync("//HomePage/ShoppingCartPage");
            return 0;
        }
        public async Task<int> GoToScanBarcode()
        {
            App.g_HeaderTitle = "Scan Barcode";
            //await Current.GoToAsync("//HomePage/BarcodeScanner");
            await Current.GoToAsync("//HomePage/QuickEntryPage");
            return 0;
        }
        public async Task<int> GoToMyPurchases()
        {
            App.g_HeaderTitle = "Order History";
            await Current.GoToAsync("//HomePage/PurchaseHistoryPage");
            return 0;
        }
        public async Task<int> GoToCategories()
        {
            App.g_HeaderTitle = "Product Categories";
            await Current.GoToAsync("//HomePage/CategoryPage");
            return 0;
        }
        public async Task<int> GoToLocations()
        {
            await Current.GoToAsync("//HomePage/LocationsPage");
            return 0;
        }
        public async Task<int> GoToLogin()
        {
            await Current.GoToAsync("//HomePage/LoginPage");
            return 0;
        }
        public async Task<int> GoToMyAccount()
        {
            App.g_HeaderTitle = "My Account";
            await Current.GoToAsync("//HomePage/MyAccountPage");
            return 0;
        }
        public async Task<int> GoToCheckout()
        {
            App.g_HeaderTitle = "Checkout";
            await Current.GoToAsync("//HomePage/ShoppingCartPage/CheckoutPage");
            return 0;
        }
        public async Task<int> GoToSubmitOrderPage()
        {
            App.g_HeaderTitle = "Submit Order";
            await Current.GoToAsync("//HomePage/ShoppingCartPage/CheckoutPage/SubmitOrderPage");
            return 0;
        }
        public async Task<int> GoToCustomerList()
        {
            App.g_HeaderTitle = "Customers";
            await Current.GoToAsync("//HomePage/CustomerListPage");
            return 0;
        }
        public async Task<int> PopModal()
        {
            try
            {
                await Navigation.PopModalAsync();
            }
            catch
            {
            }
            return 0;
        }
        public async Task<int> GoToOrderDetail()
        {
            App.g_HeaderTitle = "Order Detail";
            await Current.GoToAsync("//HomePage/PurchaseHistoryPage/PurchaseHistoryDetailPage");
            return 0;
        }
        public async Task<int> GoToReorderItems()
        {
            App.g_HeaderTitle = "Reorder Items";
            await Current.GoToAsync("//HomePage/ReorderItemsPage");
            return 0;
        }
        public async Task<int> GoToRegister()
        {
            await Current.GoToAsync("//HomePage/LoginPage");
            return 0;
        }
        public async Task<int> GoToRegisterVerify()
        {
            await Current.GoToAsync("//HomePage/RegisterVerifyPage");
            return 0;
        }
        public async Task<int> GoToItemSearch()
        {
            if (App.g_CurrentPage != "ItemSearchPage")
            {
                App.g_HeaderTitle = "Search Products";
                await Current.GoToAsync("//HomePage/CategoryPage/ItemSearchPage");
            }
            else
            {
                try
                {
                    App.g_SearchPage.RefreshList();
                }
                catch
                {
                }
            }

            return 0;
        }

        public void ShowNavBar()
        {
            SetNavBarIsVisible(this, true);
        }

        public bool bStopNavigating = true;
        public bool bStopHome = false;
        public string sNavTo = "";
        public string sLastNavTo = "";

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            // implement your logic
            base.OnNavigating(args);
        }

        private void MenuShoppingCart_Clicked(object sender, EventArgs e)
        {
            GoToShoppingCart();
            Shell.Current.FlyoutIsPresented = false;
        }
        private void MenuScanBarcode_Clicked(object sender, EventArgs e)
        {
            GoToScanBarcode();
            Shell.Current.FlyoutIsPresented = false;
        }
        private void MenuMyPurchases_Clicked(object sender, EventArgs e)
        {
            GoToMyPurchases();
            Shell.Current.FlyoutIsPresented = false;
        }
        private void MenuCategories_Clicked(object sender, EventArgs e)
        {
            GoToCategories();
            Shell.Current.FlyoutIsPresented = false;
        }
        private void MenuLocations_Clicked(object sender, EventArgs e)
        {
            GoToLocations();
            Shell.Current.FlyoutIsPresented = false;
        }
        private void MenuLogout_Clicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;
            if (!App.g_IsLoggedIn)
            {
                GoToLogin();
            }
            else
            {
                App.g_HomePage.ConfirmLogout();
            }
        }
        private void MenuMyAccount_Clicked(object sender, EventArgs e)
        {
            GoToMyAccount();
            Shell.Current.FlyoutIsPresented = false;
        }
        private void MenuCustomers_Clicked(object sender, EventArgs e)
        {
            GoToCustomerList();
            Shell.Current.FlyoutIsPresented = false;
        }
    }
}