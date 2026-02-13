using POMuswick.ViewModels;



namespace POMuswick.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = new LoginViewModel();

            App.g_LoginPage = this;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            User.IsEnabled = false;
            User.IsEnabled = true;
            Password.IsEnabled = false;
            Password.IsEnabled = true;
            RememberMe.IsEnabled = false;
            RememberMe.IsEnabled = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "LoginPage";

            AppVersion.Text = Constants.Version;

            if (App.g_Customer.RememberMe)
            {
                User.Text = App.g_Customer.User;
            }
            else
            {
            }
        }

        public void HideAnimation()
        {
            WaitImage.IsVisible = false;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void Settings_Clicked(object sender, EventArgs e)
        {
            App.g_HeaderTitle = "Settings";
            await Navigation.PushModalAsync(new SettingsPage());
        }
    }
}