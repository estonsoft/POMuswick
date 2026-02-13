using banditoth.MAUI.DeviceId.Interfaces;
using POMuswick.Data;
using POMuswick.ViewModels;
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
        public static ScanditViewModelBase g_ScanditViewModel { get; set; }
        public static Boolean g_IsSalesUser { get; set; }

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

            // 19.2 version Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTAzNTg1QDMxMzkyZTMyMmUzMGMySndvR0x2aHJwSHJWcFpwSG93MVMxMFRub1pFRkhTbnRHakhEVTd3WlE9");
            // 20.1.0.57 Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjQ1MDgwQDMyMzAyZTMxMmUzMEZVVGd2ZDhxb2liR2MzV0pybTM5ZE5SemU4Mml6SWthYnFFa3ZGZ0F6VlU9");
            // 20.2.0.43
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
            g_ScanditViewModel = null;

            if (!g_IsLoggedIn)
            {
                //Database db = new Database();

                g_UserName = "";

                if (g_db.GetSetting("LoggedIn") == "1")
                {
                    g_IsLoggedIn = true;
                    g_UserName = g_db.GetSetting("UserName");
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

                g_IsCredits = g_db.GetSetting("Credits");
                g_QOHDisplay = g_db.GetSetting("QOHDisplay");
                if (g_db.GetSetting("HoldForReview") == "1")
                {
                    g_HoldForReview = true;
                }
                else
                {
                    g_HoldForReview = false;
                }
                if (g_db.GetSetting("BlockItemsNoQOH") == "1")
                {
                    g_BlockItemsNoQOH = true;
                }
                else
                {
                    g_BlockItemsNoQOH = false;
                }
                if (g_db.GetSetting("IsSalesUser") == "1")
                {
                    g_IsSalesUser = true;
                }
                else
                {
                    g_IsSalesUser = false;
                }

                g_ServerURL = "https://muswicksales.ddns.net";    // g_db.GetSetting("ServerURL");
                //g_ServerURL = "http://192.168.1.175:8080";    // g_db.GetSetting("ServerURL");
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
                g_ShoppingCartItems = App.g_db.GetCartPieces();

                Task.Run(async () =>
                {
                    try
                    {
                        g_Customer = new Customer();
                        if (App.g_IsLoggedIn)
                        {
                            g_Customer = App.g_db.GetCustomer();
                            if (g_Customer == null)
                            {
                                g_Customer = new Customer();
                            }
                            else
                            {
                                App.g_db.RestoreCartItems(App.g_Customer.CustNo);
                            }
                        }
                    }
                    catch
                    {
                        g_Customer = new Customer();
                    }
                });

                //g_CategoryList = App.g_db.GetCategories();
                g_HomePageCategoryList = App.g_db.GetHomePageCategories();
                g_ItemList = App.g_db.GetItems();
                g_ReorderItemList = App.g_db.GetReorderItems();

                try
                {
                    App.CommManager.GetSettings();
                }
                catch { }

                RefreshAll();
                InitializeAllTimer();

                RefreshOrderHistory();
                InitializeOrderHistoryTimer();

                RefreshQOH();
                InitializeQOHTimer();

                if (g_IsSalesUser)
                {
                    App.CommManager.GetSalespersonCustomers(g_UserName);
                }
            }

            MainPage = new AppShell();
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

        private void InitializeAllTimer()
        {
            Task.Run(async () =>
                 {
                     _ = await RefreshAll();
                     return true;
                 });
        }

        public static async Task<String> RefreshAll()
        {
            // start with banners  services will call next when one is done
            App.CommManager.GetBanners();
            return "";
        }

        private void InitializeQOHTimer()
        {
            Task.Run(async () =>
                {
                    _ = await RefreshQOH();
                    return true;
                });
        }

        public static async Task<String> RefreshQOH()
        {
            try
            {
                if ((App.g_Customer.CustNo != null) && (App.g_Customer.CustNo != "") && (App.g_Customer.CustNo != "0"))
                {
                    App.CommManager.GetItemQOH2(App.g_UserName, App.g_Customer.CustNo);
                }
            }
            catch { }

            return "";
        }

        private void InitializeBannerTimer()
        {
            Task.Run(async () =>
            {
                _ = await RefreshBanners();
                return true;
            });
        }

        private async Task<String> RefreshBanners()
        {
            return "";
        }

        private void InitializeItemTimer()
        {
            Task.Run(async () =>
            {
                _ = await RefreshItems();
                return true;
            });
        }

        private async Task<String> RefreshItems()
        {
            //Database db = new Database();

            //g_Customer = await db.GetCustomerAsync();

            return "";
        }

        private void InitializeOrderHistoryTimer()
        {
            Task.Run(async () =>
            {
                _ = await RefreshOrderHistory();
                return true;
            });
        }

        public static async Task<String> RefreshOrderHistory()
        {
            try
            {
                if ((App.g_Customer.CustNo != null) && (App.g_Customer.CustNo != "") && (App.g_Customer.CustNo != "0"))
                {
                    App.CommManager.GetOrderHistory(App.g_Customer.CustNo);
                }
            }
            catch { }

            return "";
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

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
            g_IsOrderSubmitting = false;

            try
            {
                App.CommManager.GetSettings();
            }
            catch { }

            if (App.g_IsLoggedIn)
            {
                ValidateUserActive();
            }

            RefreshQOH();

            RefreshAll();

            RefreshOrderHistory();
        }
    }
}
