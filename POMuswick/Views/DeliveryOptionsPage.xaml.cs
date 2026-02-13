using POMuswick.ViewModels;

namespace POMuswick.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveryOptionsPage : ContentPage
    {
        public DeliveryOptionsPage()
        {
            InitializeComponent();
            BindingContext = new DeliveryOptionsViewModel();

            App.g_CurrentPage = "DeliveryOptionsPage";
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}