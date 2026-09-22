using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Services;
using POMuswick;
using POMuswick.ViewModels;
using POMuswick.Common;
public partial class LoginViewModel : BaseViewModel
{
    private readonly ILoginService _loginService;
    private readonly ICustomerService _customerService;
    private readonly INavigationService _navigationService;
    private readonly ISettingService _settingService;
    private readonly IAppSyncService _appSyncService;

    [ObservableProperty]
    private string _user = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _appVersion = string.Empty;

    [ObservableProperty]
    private bool _rememberMe;


    public LoginViewModel(IAppServices appServices) : base(appServices)
    {
         Title = PageTitles.Login;
        _navigationService = appServices._navigationService;
        _settingService = appServices._settingService;
        _loginService = appServices._loginService;
        _customerService = appServices._customerService;
        _appSyncService = appServices._appSyncService;
    }

    public override async Task OnAppearingAsync()
    {
        Customer customer = await _customerService.GetCustomerAsync();
        RememberMe = customer.RememberMe;
        AppVersion = Constants.Version;
        if (RememberMe)
            User = customer.User;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        try
        {
            IsBusy = true;

            var result = await _loginService.LoginAsync(
                User,
                Password);
            
            await _customerService.SaveCustomerAsync(new Customer() { RememberMe = this.RememberMe });
            
            if (result.Success)
            {
                await _appSyncService.SyncApp();
                await _navigationService.GoToRootAsync(AppRoutes.Home);
                return;
            }
            string message = result.Status switch
            {
                "P" => "Invalid password. Please try again.",
                "I" => "Inactive account. Please contact Customer Service.",
                "U" => "Account does not exist.",
                _ => "Error attempting to login."
            };

            await Shell.Current.DisplayAlertAsync(
                "Muswick Wholesale Grocers",
                message,
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SettingAsync()
    {
        await _navigationService.GoToAsync(AppRoutes.Setting);
    }
}