using POMuswick.ViewModels;

namespace POMuswick.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage(AboutViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}