using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Services;

namespace POMuswick.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    private readonly IAppServices _appServices;
    [ObservableProperty]
    public bool isBusy;

    [ObservableProperty]
    public string _title = string.Empty;

    [ObservableProperty]
    private string _syncStatus = string.Empty;

    [ObservableProperty]
    private int _progressPercentage;

    [ObservableProperty]
    private double _progressValue;

    public BaseViewModel(IAppServices appServices)
    {
        _appServices = appServices;
    }

    protected void ResetSyncProgress()
    {
        SyncStatus = "Starting sync";
        ProgressPercentage = 0;
        ProgressValue = 0;
    }

    protected IProgress<SyncProgress> CreateSyncProgress()
    {
        return new Progress<SyncProgress>(syncProgress =>
        {
            SyncStatus = syncProgress.Status;
            ProgressPercentage = syncProgress.Percentage;
            ProgressValue = syncProgress.Percentage / 100d;
        });
    }

    protected Task SyncAppAsync(string selectedCustomer = "0")
    {
        ResetSyncProgress();
        return _appServices._appSyncService.SyncApp(selectedCustomer, CreateSyncProgress());
    }

    public async Task RequestCameraPermission()
    {
        // Check current status
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

        if (status == PermissionStatus.Granted)
        {
            // Permission already granted, proceed with camera action
            // e.g., StartCamera();
        }
        else if (status == PermissionStatus.Denied && OperatingSystem.IsAndroid())
        {
            // Android specific: If denied, shouldShowRationale might tell you if you can ask again
            if (Permissions.ShouldShowRationale<Permissions.Camera>())
            {
                // Show an alert to explain why you need it, then request again
                IDialogService dialogService = _appServices._dialogService;
                if (await dialogService.ConfirmAsync("Permission Needed", "We need camera access to take photos. Allow access?", "OK", "Cancel"))
                {
                    await Permissions.RequestAsync<Permissions.Camera>();
                }
            }
        }
        else if (status != PermissionStatus.Granted) // For iOS/Others if not granted or just denied
        {
            // Request permission
            status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
                await RequestCameraPermission();
        }
    }

    public virtual Task OnAppearingAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task OnDisappearingAsync()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    public async Task BackPressedAsync()
    {
        INavigationService navigationService = _appServices._navigationService;
        await navigationService.GoBackAsync();
    }
    [RelayCommand]
    public async Task HomePressedAsync()
    {
        await _appServices._navigationService.GoToAsync(AppRoutes.Home);
    }
    [RelayCommand]
    public async Task CartPressedAsync()
    {
        List<Item> items = await _appServices._cartService.GetCartItems();
        if (items.Count == 0)
            await _appServices._dialogService.AlertAsync("Muswick Wholesale Grocers", "Your shopping cart is empty", "Ok");
        else
            await _appServices._navigationService.GoToAsync(AppRoutes.ShoppingCart);
    }
    [RelayCommand]
    public async Task ShopPressedAsync()
    {
        await _appServices._navigationService.GoToAsync(AppRoutes.Categories);
    }
    [RelayCommand]
    public async Task ScanPressedAsync()
    {
        await _appServices._navigationService.GoToAsync(AppRoutes.QuickEntry);
    }
    [RelayCommand]
    public async Task HistoryPressedAsync()
    {
        await _appServices._navigationService.GoToAsync(AppRoutes.PurchaseHistory);
    }
}
