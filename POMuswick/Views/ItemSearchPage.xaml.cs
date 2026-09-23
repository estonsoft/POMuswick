using POMuswick.Models;
using POMuswick.ViewModels;

namespace POMuswick.Views
{
    public partial class ItemSearchPage : ContentPage
    {
        private readonly ItemSearchViewModel _viewModel;
        public ItemSearchPage(ItemSearchViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
            NavigationPage.SetHasNavigationBar(this, false);
            Shell.SetNavBarIsVisible(this, false);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.OnAppearingAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void OnTappedSearch(object sender, EventArgs e)
        {
            await _viewModel.RefreshList();
        }

        private async void OnInStockChanged(object sender, CheckedChangedEventArgs e)
        {
            await _viewModel.RefreshList();
        }
    }
}

