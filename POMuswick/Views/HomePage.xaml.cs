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
