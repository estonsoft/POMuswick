using banditoth.MAUI.DeviceId.Interfaces;
using POMuswick.Data;
using POMuswick.Models;
using POMuswick.Parsers;
using POMuswick.Repository;
namespace POMuswick.Services
{
    public class LoginService : ILoginService
    {
        private readonly CommManager _comm;
        private readonly LoginParser _parser;
        private readonly ICustomerService _customerService;
        private readonly ISettingService _settingService;
        private readonly ILocationService _locationService;

        private readonly IDeviceIdProvider _deviceIdProvider;
        public LoginService(
            CommManager comm,
            LoginParser parser,
            ICustomerService customerService,
            ISettingService settingService,
            ILocationService locationService,
            IDeviceIdProvider deviceIdProvider)
        {
            _comm = comm;
            _parser = parser;
            _customerService = customerService;
            _settingService = settingService;
            _locationService = locationService;
            _deviceIdProvider = deviceIdProvider;
        }

        public async Task<LoginResult> LoginAsync(string user, string password)
        {
            await CheckURL(user);
            var deviceID = _deviceIdProvider.GetDeviceId();
            var response = await _comm.ValidateLogin(user, password, deviceID);

            var result = _parser.Parse(response);
            result.Settings.UserName = user;
            if (result.Success)
            {
                await _customerService.SaveCustomerAsync(result.Customer);

                await _settingService.SaveChanges(new Dictionary<string, string>
                {
                    [nameof(AppSettings.UserName)] = user,
                    [nameof(AppSettings.CustomerNo)] = result.Settings.CustomerNo,
                    [nameof(AppSettings.IsLoggedIn)] = result.Settings.IsLoggedIn ? "1" : "0",
                    [nameof(AppSettings.IsCredits)] = result.Settings.IsCredits ? "1" : "0",
                    [nameof(AppSettings.IsSalesUser)] = result.Settings.IsSalesUser ? "1" : "0",
                    [nameof(AppSettings.HoldForReview)] = result.Settings.HoldForReview ? "1" : "0",
                    [nameof(AppSettings.ForceSubmit)] = result.Settings.ForceSubmit ? "1" : "0",
                    [nameof(AppSettings.QOHDisplay)] = result.Settings.QOHDisplay ?? "X",
                    [nameof(AppSettings.BlockItemsNoQOH)] = result.Settings.BlockItemsNoQOH ? "1" : "0"
                });

                await _locationService.SaveLocationAsync(result.Location);
            }
            return result;
        }

        public async Task<LoginResult> ValidateUserAsync()
        {
            var customer = await _customerService.GetCustomerAsync();
            var response = await _comm.ValidateUserActive(customer.User);
            LoginResult loginResult = _parser.ParseActiveUser(response);
            return loginResult;
        }

        private async Task CheckURL(string user)
        {
            AppSettings appSettings = await _settingService.LoadSetting();
            if (user.ToLower() == "app_test")
            {
                appSettings.BaseUrl = "https://store.qwikpoint.net";
            }
            else
            {
                appSettings.BaseUrl = "https://muswicksales.ddns.net";
            }
            Constants.BaseURL = appSettings.BaseUrl;

            await _settingService.SaveChanges(new Dictionary<string, string>
            {
                [nameof(AppSettings.BaseUrl)] = appSettings.BaseUrl
            });
        }
    }

    public interface ILoginService
    {
        public Task<LoginResult> LoginAsync(string user, string password);

        public Task<LoginResult> ValidateUserAsync();

    }
}