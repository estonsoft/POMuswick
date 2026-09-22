using POMuswick.ViewModels;

namespace POMuswick.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private readonly SettingViewModel _settingViewModel;
        public SettingsPage(SettingViewModel settingViewModel)
        {
            InitializeComponent();
            BindingContext = _settingViewModel = settingViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _settingViewModel.OnAppearingAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}