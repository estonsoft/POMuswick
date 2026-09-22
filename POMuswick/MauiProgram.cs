using banditoth.MAUI.DeviceId;
using BarcodeScanning;
using FFImageLoading.Maui;
using Microsoft.Extensions.Logging;
using POMuswick.Data;
using POMuswick.Parsers;
using POMuswick.Repository;
using POMuswick.Services;
using POMuswick.ViewModels;
using POMuswick.Views;
using Scandit.DataCapture.Barcode;
using Scandit.DataCapture.Core;
using Scandit.DataCapture.Core.UI.Maui;
using Syncfusion.Maui.Core.Hosting;

#if MAUI_DEVFLOW
using Microsoft.Maui.DevFlow.Agent;
#endif

namespace POMuswick
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
#if MAUI_DEVFLOW
            builder.AddMauiDevFlowAgent();
#endif
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .UseFFImageLoading()
                .UseBarcodeScanning()
                .UseScanditCore()
                .UseScanditBarcode(configure =>
                {
                    configure.AddSparkScanView();
                    configure.AddBarcodePickView();
                    configure.AddBarcodeFindView();
                    configure.AddBarcodeCountView();
                    configure.AddBarcodeArView();

                })
                .ConfigureMauiHandlers(handlers =>
                {
                    // Explicitly register the Scandit DataCaptureView handler
                    handlers.AddHandler(typeof(DataCaptureView), typeof(DataCaptureViewHandler));
                })

                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Font Awesome 5 Brands-Regular-400.otf", "FontAwesomeBrandsReg");
                    fonts.AddFont("Font Awesome 5 Free-Regular-400.otf", "FontAwesomeFreeReg");
                    fonts.AddFont("Font Awesome 5 Free-Solid-900.otf", "FontAwesomeFreeSolid");
                    fonts.AddFont("Font Awesome 6 Pro-Light-300.otf", "FontAwesomePro6Light");
                    fonts.AddFont("Font Awesome 6 Pro-Regular-400.otf", "FontAwesomePro6Regular");
                    fonts.AddFont("Font Awesome 6 Pro-Solid-900.otf", "FontAwesomePro6Solid");
                    fonts.AddFont("Font Awesome 6 Pro-Thin-100.otf", "FontAwesomePro6Thin");
                });
#if DEBUG
            builder.Logging.AddDebug();
#endif
            // ✅ REQUIRED
            builder.ConfigureDeviceIdProvider();
            builder.Services.AddSingleton<ISoapService>(sp =>
            {
                var httpClient = new HttpClient();
                return new SoapService(httpClient);
            });

            // Optional manager
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<AppState>();
            builder.Services.AddSingleton<CommManager>();
            builder.Services.AddSingleton<Database>();

            builder.Services.AddTransient<BannerParser>();
            builder.Services.AddTransient<CatNSubCatParser>();
            builder.Services.AddTransient<ItemParser>();
            builder.Services.AddTransient<ItemQQHParser>();
            builder.Services.AddTransient<LoginParser>();
            builder.Services.AddTransient<OrderHistoryParser>();
            builder.Services.AddTransient<PDFFilesParser>();
            builder.Services.AddTransient<SalesPersonCustomersParser>();
            builder.Services.AddTransient<SettingParser>();
            builder.Services.AddTransient<SubmitOrderParser>();

            builder.Services.AddSingleton<IBannerRepository, BannerRepository>();
            builder.Services.AddSingleton<ICartRepository, CartRepository>();
            builder.Services.AddSingleton<ISettingRepository, SettingRepository>();
            builder.Services.AddSingleton<ICartRepository, CartRepository>();
            builder.Services.AddSingleton<ICatNSubcatRepository, CatNSubcatRepository>();
            builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
            builder.Services.AddSingleton<ILocationRepository, LocationRepository>();
            builder.Services.AddSingleton<IItemQQHRepository, ItemQQHRepository>();
            builder.Services.AddSingleton<IItemRepository, ItemRepository>();
            builder.Services.AddSingleton<IOrderHistoryRepository, OrderHistoryRepository>();
            builder.Services.AddSingleton<IPDFFilesRepository, PDFFilesRepository>();
            builder.Services.AddSingleton<ISalesPersonCustomersRepository, SalesPersonCustomersRepository>();


            builder.Services.AddSingleton<IAppServices, AppServices>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<ISettingService, SettingService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<IBannerService, BannerService>();
            builder.Services.AddSingleton<IAppSyncService, AppSyncService>();
            builder.Services.AddSingleton<ICartService, CartService>();
            builder.Services.AddSingleton<ICatNSubCatService, CatNSubCatService>();
            builder.Services.AddSingleton<ICustomerService, CustomerService>();
            builder.Services.AddSingleton<IItemQQHService, ItemQQHService>();
            builder.Services.AddSingleton<IItemService, ItemService>();
            builder.Services.AddSingleton<ILocationService, LocationService>();
            builder.Services.AddSingleton<ILoginService, LoginService>();
            builder.Services.AddSingleton<IOrderHistoryService, OrderHistoryService>();
            builder.Services.AddSingleton<IPDFFilesService, PDFFilesService>();
            builder.Services.AddSingleton<ISalesPersonCustomersService, SalesPersonCustomersService>();
            builder.Services.AddSingleton<ISubmitOrderService, SubmitOrderService>();



            builder.Services.AddTransient<AboutPage>();
            builder.Services.AddTransient<CategoryPage>();
            builder.Services.AddTransient<CheckoutPage>();
            builder.Services.AddTransient<CustomerListPage>();
            builder.Services.AddTransient<ItemSearchPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<MyAccountPage>();
            builder.Services.AddTransient<PurchaseHistoryDetailPage>();
            builder.Services.AddTransient<PurchaseHistoryPage>();
            builder.Services.AddTransient<QuickEntryPage>();
            builder.Services.AddTransient<ReorderItemsPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<ShoppingCartPage>();
            builder.Services.AddTransient<SubcategoryPage>();
            builder.Services.AddTransient<SubmitOrderPage>();

            builder.Services.AddTransient<AboutViewModel>();
            builder.Services.AddTransient<AccountViewModel>();
            builder.Services.AddTransient<CategoryViewModel>();
            builder.Services.AddTransient<CheckoutViewModel>();
            builder.Services.AddTransient<CustomerListViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<ItemSearchViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<PurchaseHistoryDetailViewModel>();
            builder.Services.AddTransient<PurchaseHistoryViewModel>();
            builder.Services.AddTransient<QuickEntryViewModel>();
            builder.Services.AddTransient<ReorderViewModel>();
            builder.Services.AddTransient<SettingViewModel>();
            builder.Services.AddTransient<ShoppingCartViewModel>();
            builder.Services.AddTransient<SubCategoryViewModel>();
            builder.Services.AddTransient<SubmitOrderViewModel>();
            return builder.Build();
        }
    }
}
