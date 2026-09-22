using System.Diagnostics;
using POMuswick.ViewModels;

namespace POMuswick.Views
{
    public partial class ReorderItemsPage : ContentPage
    {
        private readonly ReorderViewModel _reorderViewModel;
        public ReorderItemsPage(ReorderViewModel reorderViewModel)
        {
            InitializeComponent();
            BindingContext = _reorderViewModel= reorderViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _reorderViewModel.OnAppearingAsync();
        }
        protected override bool OnBackButtonPressed() => true;
    }
}
