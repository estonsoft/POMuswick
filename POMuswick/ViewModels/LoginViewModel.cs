namespace POMuswick.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }
        public String User { get; set; }
        public String Password { get; set; }
        public bool RememberMe { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);

            try
            {
                RememberMe = App.g_Customer.RememberMe;
            }
            catch
            {
                RememberMe = false;
            }
        }

        private void OnLoginClicked(object obj)
        {
            App.g_LoginPage.ShowAnimation();
            Task.Run(async () =>
            {
                if (User.ToLower() == "app_test")
                {
                    App.g_ServerURL = "https://store.qwikpoint.net";
                }
                else
                {
                    App.g_ServerURL = "https://muswicksales.ddns.net";    // g_db.GetSetting("ServerURL");
                }

                App.UpdateServerLinks();

                App.g_IsLoggedIn = true;
                App.g_UserName = User;

                App.g_Customer.User = User;
                App.g_Customer.RememberMe = RememberMe;

                await App.g_db.SaveCustomer(App.g_Customer);

                await App.CommManager.ValidateLogin(User, Password, App.g_Customer.UniqueId);

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    App.g_LoginPage.HideAnimation();
                });
            });
        }
    }
}
