using banditoth.MAUI.DeviceId.Interfaces;
using POMuswick.Controls;
using POMuswick.Data;
using POMuswick.Views;

namespace POMuswick
{
    public partial class App : Application
    {
        public static App g_App;
        public static AppShell g_Shell;

        public static Database g_db;

        public static ItemSearchPage g_SearchPage;
        public static HomePage g_HomePage;
        public static LoginPage g_LoginPage;
        public static CustomerListPage g_CustomerPage;
        public static ShoppingCartPage g_ShoppingCartPage;
        public static CheckoutPage g_CheckoutPage;
        public static Customer g_Customer;
        public static Category g_Category;
        public static Subcategory g_Subcategory;

        //public static List<Category> g_CategoryList;
        public static List<Category> g_HomePageCategoryList;
        public static List<Item> g_ItemList;
        public static List<Item> g_ReorderItemList;

        public static CommManager CommManager { get; set; }
        public static String g_SearchText { get; set; }
        public static String g_SectionName { get; set; }
        public static String g_ScanBarcode { get; set; }
        public static String g_UserName { get; set; }
        public static String g_ServerURL { get; set; }
        public static String g_Company { get; set; }
        public static String g_CurrentPage { get; set; }
        public static String g_SearchFromPage { get; set; }
        public static Boolean g_IsLoggedIn { get; set; }
        public static Boolean g_InStockOnly { get; set; }
        public static String g_IsCredits { get; set; }
        public static Boolean g_HoldForReview { get; set; }
        public static Boolean g_ForceSubmit { get; set; }
        public static Boolean g_BlockItemsNoQOH { get; set; }
        public static Boolean g_IsOrderSubmitting { get; set; }
        public static String g_QOHDisplay { get; set; }
        public static String g_OrderNo { get; set; }
        public static String g_HeaderTitle { get; set; }
        public static int g_ShoppingCartItems { get; set; }
        public static int g_NewItemIndex { get; set; }
        public static double g_CategoryScrollY { get; set; }
        public static bool g_IsScannerInit { get; set; }
        public static Boolean g_IsSalesUser { get; set; }

        public static ImageAlertView imageAlertView;

        public static String app_uniqueId { get; set; }

        public class MessageKeys
        {
            public const string OnStart = nameof(OnStart);
            public const string OnSleep = nameof(OnSleep);
            public const string OnResume = nameof(OnResume);
        }

        public App(IDeviceIdProvider deviceIdProvider, CommManager _commManager)
        {
            InitializeComponent();
            CommManager = _commManager;

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Njk0OTc0QDMyMzAyZTMyMmUzMFdNblBzcjZVWWc5Q0VMdHZRdXQxeFYyRGhGdlF5ZGIzUjQ2VGdLU2ZBbGM9");
            app_uniqueId = deviceIdProvider.GetDeviceId();

            try
            {
                if (g_db == null)
                {
                    g_db = new Database();
                }
            }
            catch
            {
                g_db = new Database();
            }

            g_NewItemIndex = 0;

            g_App = this;

            _ = InitializeApp();

        }

        public async Task<bool> InitializeApp()
        {
            try
            {
                if (!g_IsLoggedIn)
                {
                    g_IsLoggedIn = false;
                }
            }
            catch
            {
                g_IsLoggedIn = false;
            }

            g_IsScannerInit = false;

            if (!g_IsLoggedIn)
            {
                g_UserName = "";

                if (await g_db.GetSetting("LoggedIn") == "1")
                {
                    g_IsLoggedIn = true;
                    g_UserName = await g_db.GetSetting("UserName");
                }

                g_Company = "";
                g_SearchText = "";
                g_SearchFromPage = "";
                g_ScanBarcode = "";
                g_SectionName = "";
                g_CurrentPage = "";
                g_IsOrderSubmitting = false;
                g_OrderNo = "";
                g_HeaderTitle = "";
                g_CategoryScrollY = 0;

                g_IsCredits = await g_db.GetSetting("Credits");
                g_QOHDisplay = await g_db.GetSetting("QOHDisplay");
                if (await g_db.GetSetting("HoldForReview") == "1")
                {
                    g_HoldForReview = true;
                }
                else
                {
                    g_HoldForReview = false;
                }
                if (await g_db.GetSetting("BlockItemsNoQOH") == "1")
                {
                    g_BlockItemsNoQOH = true;
                }
                else
                {
                    g_BlockItemsNoQOH = false;
                }
                if (await g_db.GetSetting("IsSalesUser") == "1")
                {
                    g_IsSalesUser = true;
                }
                else
                {
                    g_IsSalesUser = false;
                }

                if (App.g_UserName == "app_test")
                {
                    App.g_ServerURL = "https://store.qwikpoint.net";
                }
                else
                {
                    g_ServerURL = "https://muswicksales.ddns.net";    // await g_db.GetSetting("ServerURL");
                }

                UpdateServerLinks();

                g_Category = new Category();
                g_Category.Code = "";
                g_Category.Description = "ALL CATEGORIES";

                g_Subcategory = new Subcategory();
                g_Subcategory.Code = "";
                g_Subcategory.Description = "ALL SUBCATEGORIES";


                Constants.Load();

                Location location = new Location();
                location.Refresh();

                g_Customer = new Customer();
                g_ShoppingCartItems = await g_db.GetCartPieces();

                await Task.Run(async () =>
                {
                    try
                    {
                        g_Customer = new Customer();
                        if (App.g_IsLoggedIn)
                        {
                            g_Customer = await g_db.GetCustomer();
                            if (g_Customer == null)
                            {
                                g_Customer = new Customer();
                            }
                            else
                            {
                                await g_db.RestoreCartItems(App.g_Customer.CustNo);
                            }
                        }
                        // await RefreshAll();
                        // await RefreshOrderHistory();
                        // await RefreshQOH();
                    }
                    catch
                    {
                        g_Customer = new Customer();
                    }
                });

                //g_CategoryList = await g_db.GetCategories();
                g_HomePageCategoryList = await g_db.GetHomePageCategories();
                g_ItemList = await g_db.GetItems();
                g_ReorderItemList = await g_db.GetReorderItems();

                try
                {
                    await App.CommManager.GetSettings();
                }
                catch { }

                // if (g_IsSalesUser)
                // {
                //     await App.CommManager.GetSalespersonCustomers(g_UserName);
                // }
            }
            return true;
        }

        public async Task<bool> InitializeAppAfterLogin()
        {
            try
            {
                await App.CommManager.GetSettings();
                await RefreshAll();
                await RefreshOrderHistory();
                await RefreshQOH();
                if (g_IsSalesUser)
                {
                    await App.CommManager.GetSalespersonCustomers(g_UserName);
                }
            }
            catch (Exception ex)
            {
                String sMsg = ex.Message;
                Console.WriteLine("InitializeAppAfterLogin exception" + sMsg + ex.StackTrace);
            }
            return true;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        public static void UpdateServerLinks()
        {
            Constants.BaseURL = App.g_ServerURL;
            Constants.SoapUrl = App.g_ServerURL + "/RemotePhoneApp.asmx";
            Constants.LogoUrl = App.g_ServerURL + "/images/logo/logo.png";
            Constants.BannerUrl = App.g_ServerURL + "/images/banner phone/";
            Constants.CategoryImageUrl = App.g_ServerURL + "/images/category/";
            Constants.ItemImageUrl = App.g_ServerURL + "/images/items/";
        }

        public static async Task RefreshAll()
        {
            // start with banners  services will call next when one is done
            await App.CommManager.GetBanners();
        }

        public static async Task RefreshQOH()
        {
            if ((App.g_Customer.CustNo != null) && (App.g_Customer.CustNo != "") && (App.g_Customer.CustNo != "0"))
            {
                await App.CommManager.GetItemQOH2(App.g_UserName, App.g_Customer.CustNo);
            }
        }

        public static async Task RefreshOrderHistory()
        {
            if ((App.g_Customer.CustNo != null) && (App.g_Customer.CustNo != "") && (App.g_Customer.CustNo != "0"))
            {
                await App.CommManager.GetOrderHistory(App.g_Customer.CustNo);
            }
        }


        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected async override void OnResume()
        {
            g_IsOrderSubmitting = false;
            await App.CommManager.GetSettings();
            if (App.g_IsLoggedIn)
            {
                await App.CommManager.ValidateUserActive(App.g_UserName);
            }

            await RefreshQOH();

            await RefreshAll();

            await RefreshOrderHistory();
        }

        private static CancellationTokenSource? _progressCts;
        private static int _actualProgress;
        private static int _displayProgress;

        public static async Task StartProgress(int progress, string status)
        {
            _actualProgress = progress;
            _displayProgress = progress;

            _progressCts?.Cancel();
            _progressCts = new CancellationTokenSource();

            await UpdateProgressUI(_displayProgress, status);

            _ = RunProgressAnimationAsync(status, _progressCts.Token);
        }

        private static async Task RunProgressAnimationAsync(
            string status,
            CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(1500, token);

                    if (_displayProgress < _actualProgress + 40 &&
                        _displayProgress < 99)
                    {
                        _displayProgress++;

                        await UpdateProgressUI(
                            _displayProgress,
                            status);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Expected when next real progress arrives
            }
        }

        public static async Task UpdateProgress(
            int progress,
            string status)
        {
            _actualProgress = progress;

            // If actual progress is already behind the animated value,
            // don't move backwards.
            if (_displayProgress < progress)
                _displayProgress = progress;

            await UpdateProgressUI(
                _displayProgress,
                status);

            // Restart the 5% animation for this new operation
            _progressCts?.Cancel();

            _progressCts = new CancellationTokenSource();

            _ = RunProgressAnimationAsync(
                status,
                _progressCts.Token);
        }

        public static async Task UpdateProgressUI(double current,
            string status)
        {
            switch (g_CurrentPage)
            {
                case "LoginPage":
                    g_LoginPage.UpdateSyncProgress(current, status);
                    break;
                case "HomePage":
                    g_HomePage.UpdateSyncProgress(current, status);
                    break;
                case "CustomerListPage":
                    g_CustomerPage.UpdateSyncProgress(current, status);
                    break;
            }
        }
    }
}
