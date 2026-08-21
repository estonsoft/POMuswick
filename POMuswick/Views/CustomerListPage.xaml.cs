using POMuswick.Controls;

namespace POMuswick.Views
{
    public partial class CustomerListPage : ContentPage
    {
        private List<SalesCustomer> customers = new List<SalesCustomer>();

        public CustomerListPage()
        {
            try
            {
                InitializeComponent();
                App.g_CustomerPage = this;
            }
            catch (Exception ex)
            {
                Console.WriteLine("InitializeComponent Error " + Environment.NewLine + ex.ToString() + Environment.NewLine + ex.StackTrace);
            }

            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "CustomerListPage";

            RefreshList();
        }

        public void UpdateSyncProgress(
            double current,
            string status)
        {
            int total = 100;

            var progress = (double)current / total;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingAlert.ProgressValue = progress;
                LoadingAlert.ProgressPercentage = (int)(progress * 100);
                LoadingAlert.SyncStatus = status;
            });
        }

        public async void RefreshList()
        {

            CustomerList.ItemsSource = null;

            await Task.Run(async () =>
            {
                customers = await App.g_db.GetSalesCustomers(CustomerSearch.Text);
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    CustomerList.ItemsSource = customers;
                });
            });
            //Database db = new Database();
            App.g_CurrentPage = "CustomerListPage";
        }

        async void OnTappedSearch(object sender, EventArgs args)
        {
            RefreshList();
        }

        async void OnTappedCustomer(object sender, EventArgs args)
        {
            LoadingAlert.IsVisible = true;
            await App.ResetProgressAsync();

            string OldCustNo = App.g_Customer.CustNo;

            var c = sender as CustomerStackLayout;

            SalesCustomer cust = await App.g_db.FindSalesCustomer(c.CustNo);

            App.g_Customer.CustId = -1;
            App.g_Customer.CustNo = cust.CustNo;
            App.g_Customer.CompanyName = cust.CompanyName;
            App.g_Customer.Address1 = cust.Address1;
            App.g_Customer.Address2 = cust.Address2;
            App.g_Customer.City = cust.City;
            App.g_Customer.State = cust.State;
            App.g_Customer.Zip = cust.Zip;
            App.g_Customer.CityStateZip = cust.CityStateZip;
            App.g_Customer.Phone = cust.Phone;
            App.g_Customer.Contact = cust.Contact;
            App.g_Customer.Email = cust.Email;
            App.g_Customer.Delivery = cust.Delivery;
            App.g_Customer.Warehouse = cust.Warehouse;
            App.g_Customer.TermsDesc = cust.TermsDesc;
            App.g_Customer.ARBalance = cust.ARBalance;
            App.g_Customer.CreditLimit = cust.CreditLimit;
            App.g_Customer.LastPaymentDate = cust.LastPaymentDate;
            App.g_Customer.LastOrderDate = cust.LastOrderDate;
            App.g_Customer.Delivery = 1;
            App.g_Customer.Warehouse = 1;
            App.g_Customer.MinOrderAmount = cust.MinOrderAmount;
            App.g_Customer.ShippingFee = cust.ShippingFee;

            await App.g_db.SaveCustomer(App.g_Customer);

            await App.g_db.SuspendCartItems(OldCustNo);
            await App.g_db.ClearCartItems();
            await App.g_db.DeleteOrderHistory();

            await App.g_db.RestoreCartItems(App.g_Customer.CustNo);

            await App.g_App.InitializeAppAfterLogin();

            LoadingAlert.IsVisible = false;
            await App.g_Shell.GoToHome();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void CustomerSearch_Completed(object sender, EventArgs e)
        {
            RefreshList();
        }
    }
}
