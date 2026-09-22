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
            CheckURL(user);
            var deviceID = _deviceIdProvider.GetDeviceId();
            var response = await _comm.ValidateLogin(user, password,deviceID);

            var result = _parser.Parse(response);
            result.Settings.UserName = user;
            if (result.Success)
            {
                // _customerRepository.Save(result.Customer);

                await _settingService.SaveSetting(result.Settings);

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

        private void CheckURL(string user)
        {
            AppSettings appSettings = new AppSettings();
            if (user.ToLower() == "app_test")
            {
                appSettings.BaseUrl = "https://store.qwikpoint.net";
            }
            else
            {
                appSettings.BaseUrl = "https://muswicksales.ddns.net";
            }
            appSettings.UpdateServerLinks(appSettings.BaseUrl);
            _settingService.SaveSetting(appSettings);
        }
    }

    public interface ILoginService
    {
        public Task<LoginResult> LoginAsync(string user, string password);

        public Task<LoginResult> ValidateUserAsync();

    }
}