using POMuswick.ViewModels;

namespace POMuswick.Views
{
    public partial class PurchaseHistoryDetailPage : ContentPage
    {
        private readonly PurchaseHistoryDetailViewModel _purchaseHistoryDetailViewModel;
        public PurchaseHistoryDetailPage(PurchaseHistoryDetailViewModel purchaseHistoryDetailViewModel)
        {
            InitializeComponent();
            BindingContext = _purchaseHistoryDetailViewModel=purchaseHistoryDetailViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _purchaseHistoryDetailViewModel.OnAppearingAsync();
        }

       

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}

