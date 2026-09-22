using POMuswick.ViewModels;
namespace POMuswick.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly LoginViewModel _viewModel;
        public LoginPage(LoginViewModel vm)
        {
            BindingContext = _viewModel = vm;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.OnAppearingAsync();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await _viewModel.OnDisappearingAsync();
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}