namespace POMuswick.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = this;

            App.g_CurrentPage = "Settings";
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (App.g_ServerURL.IndexOf("www.turningpointsystems.com") == -1)
            {
                ServerURL.Text = App.g_ServerURL;
            }
            else
            {
                ServerURL.Text = "";
            }
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            string sURL = ServerURL.Text.Trim();

            if (sURL != "")
            {
                if (!Uri.IsWellFormedUriString(sURL, UriKind.Absolute))
                {
                    await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Invalid Server URL", "Ok");
                    return;
                }

                if (App.g_ServerURL != sURL)
                {
                    App.g_ServerURL = sURL;
                    App.UpdateServerLinks();

                    await App.RefreshAll();
                    await App.RefreshOrderHistory();
                }
            }

            //Database db = new Database();
            App.g_db.SaveSetting("ServerURL", App.g_ServerURL);

            OnBackButtonPressed();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}