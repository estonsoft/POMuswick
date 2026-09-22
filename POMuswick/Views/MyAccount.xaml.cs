using POMuswick.ViewModels;

namespace POMuswick.Views
{
    public partial class MyAccountPage : ContentPage
    {
        private readonly AccountViewModel _accountViewModel;
        public MyAccountPage(AccountViewModel accountViewModel)
        {
            InitializeComponent();
            BindingContext = _accountViewModel = accountViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _accountViewModel.OnAppearingAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}