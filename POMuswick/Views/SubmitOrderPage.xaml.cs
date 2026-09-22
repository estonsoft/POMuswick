using POMuswick.ViewModels;

namespace POMuswick.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubmitOrderPage : ContentPage
    {
        private readonly SubmitOrderViewModel _submitOrderViewModel;
        public SubmitOrderPage(SubmitOrderViewModel submitOrderViewModel)
        {
            InitializeComponent();
            BindingContext = _submitOrderViewModel = submitOrderViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _submitOrderViewModel.OnAppearingAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}