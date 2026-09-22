using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Common;
using POMuswick.Models;
using POMuswick.Services;

namespace POMuswick.ViewModels
{
    public partial class SettingViewModel : BaseViewModel
    {
        private readonly ISettingService _settingService;
        private readonly INavigationService _navigationService;
        private readonly IAppSyncService _appSyncService;

        [ObservableProperty]
        public string _serverURLText;
        private AppSettings appSettings;

        public SettingViewModel(IAppServices appServices) : base(appServices)
        {
            Title = PageTitles.Settings;
            _settingService = appServices._settingService;
            _navigationService = appServices._navigationService;
            _appSyncService = appServices._appSyncService;
        }

        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            appSettings = await _settingService.LoadSetting();
            if (appSettings.BaseUrl.IndexOf("www.turningpointsystems.com") == -1)
            {
                ServerURLText = appSettings.BaseUrl;
            }
            else
            {
                ServerURLText = "";
            }
        }

        [RelayCommand]
        private async Task SaveSettingAsync()
        {
            string sURL = ServerURLText.Trim();

            if (sURL != "")
            {
                if (!Uri.IsWellFormedUriString(sURL, UriKind.Absolute))
                {
                    await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Invalid Server URL", "Ok");
                    return;
                }

                if (appSettings.BaseUrl != sURL)
                {
                    appSettings.BaseUrl = sURL;
                    Constants.BaseURL = appSettings.BaseUrl;
                    await _appSyncService.SyncApp();
                }
            }
            await _settingService.SaveChanges(new Dictionary<string, string>
            {
                [nameof(AppSettings.BaseUrl)] = appSettings.BaseUrl
            });
            await _navigationService.GoBackAsync();
        }
    }
}