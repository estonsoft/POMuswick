using POMuswick.Controls;
using POMuswick.ViewModels;

namespace POMuswick.Views
{
    public partial class PurchaseHistoryPage : ContentPage
    {
        private readonly PurchaseHistoryViewModel _viewModel;
        public PurchaseHistoryPage(PurchaseHistoryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            // Load your data or items here
            await _viewModel.OnAppearingAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
