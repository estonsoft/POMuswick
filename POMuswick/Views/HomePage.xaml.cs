using POMuswick.ViewModels;

namespace POMuswick.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly HomeViewModel _viewModel;
        public HomePage(HomeViewModel homeViewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = homeViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                await _viewModel.OnAppearingAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Home page load failed: {ex}");
            }
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            try
            {
                await _viewModel.OnDisappearingAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Home page unload failed: {ex}");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
