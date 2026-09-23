using POMuswick.Data;
using POMuswick.Models;
using POMuswick.Repository;
using POMuswick.Services;

namespace POMuswick
{
    public class AppState
    {
        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;
        private readonly INavigationService _navigationService;
        private readonly ILoginService _loginService;
        private static AppSettings appSettings = new();

        public AppState(ISettingService settingService, ICustomerService customerService, INavigationService navigationService, ILoginService loginService)
        {
            _settingService = settingService;
            _customerService = customerService;
            _navigationService = navigationService;
            _loginService = loginService;

            appSettings = _settingService.LoadSetting().Result;
        }

        public static string GetbaseUrl()
        {
            return appSettings.BaseUrl;
        }

        public async void AppResume()
        {
            try
            {
                if (appSettings.IsLoggedIn &&
                    Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    var result = await _loginService.ValidateUserAsync();
                    if (result.Status == "1" && Shell.Current.CurrentState.Location.ToString() != $"//{AppRoutes.Home}")
                    {
                        await _navigationService.GoToRootAsync(AppRoutes.Home);
                    }
                }
                appSettings.IsOrderSubmiting = false;
            }
            catch (HttpRequestException)
            {
                System.Diagnostics.Debug.WriteLine("App resume skipped because the network is unavailable.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"App resume failed: {ex}");
            }
        }
    }
}