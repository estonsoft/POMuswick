namespace POMuswick.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ValidateOrderPage : ContentPage
    {
        public ValidateOrderPage()
        {
            InitializeComponent();
            BindingContext = this;

            App.g_CurrentPage = "ValidateOrderPage";
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            List<Item> lstCartItems = await App.g_db.GetCartItems();
            String sOrderInfo = "";

            foreach (Item item in lstCartItems)
            {
                try
                {
                    sOrderInfo += item.ItemNo.ToString() + "|";
                    sOrderInfo += item.QtyOrder.ToString() + "|";
                    sOrderInfo += "0" + "~";
                }
                catch { }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}