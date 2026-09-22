using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Services;

namespace POMuswick.ViewModels
{
    public partial class AboutViewModel : BaseViewModel
    {
        public AboutViewModel(IAppServices appServices) : base(appServices)
        {
            Title = "About";
        }
        
        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
        }

        [RelayCommand]
        public async Task OpenWebAsync()
        {
            await Browser.OpenAsync("https://aka.ms/xamarin-quickstart");
        }
    }
}