using POMuswick.ViewModels;

namespace POMuswick.Views
{
    public partial class CustomerListPage : ContentPage
    {
        private readonly CustomerListViewModel _customerListViewModel;

        public CustomerListPage(CustomerListViewModel customerListViewModel)
        {
            BindingContext = _customerListViewModel= customerListViewModel;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _customerListViewModel.OnAppearingAsync();
        }
        
        protected override bool OnBackButtonPressed()
        {
            return true;
        }      
    }
}
