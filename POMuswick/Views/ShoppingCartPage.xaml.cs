using CommunityToolkit.Mvvm.Messaging;
using POMuswick.Data;
using POMuswick.ViewModels;

namespace POMuswick.Views
{
    public partial class ShoppingCartPage : ContentPage
    {
        
        private readonly ShoppingCartViewModel _viewModel;
        public ShoppingCartPage(ShoppingCartViewModel shoppingCartViewModel)
        {
            InitializeComponent();
            _viewModel = shoppingCartViewModel;
            BindingContext = _viewModel ;
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
    }
}