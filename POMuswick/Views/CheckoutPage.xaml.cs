using POMuswick.ViewModels;

namespace POMuswick.Views
{
    public partial class CheckoutPage : ContentPage
    {
        private readonly CheckoutViewModel _checkoutViewModel;
        public CheckoutPage(CheckoutViewModel checkoutViewModel)
        {
            InitializeComponent();
            BindingContext = _checkoutViewModel = checkoutViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _checkoutViewModel.OnAppearingAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

    }
}