using POMuswick.ViewModels;

namespace POMuswick.Views
{
    public partial class SubcategoryPage : ContentPage
    {
        private readonly SubCategoryViewModel _viewModel;
        public SubcategoryPage(SubCategoryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
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