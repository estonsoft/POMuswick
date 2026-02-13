using CommunityToolkit.Mvvm.Messaging;

namespace POMuswick.Views
{
    public partial class ShoppingCartPage : ContentPage
    {
        int iCartItems = 0;
        int iCartPieces = 0;
        decimal dCartTotal = 0;

        string sCartItems;
        string sCartPieces;
        string sCartTotal;

        List<Item> lstItems = new List<Item>();

        public string CartItems
        {
            get { return iCartItems.ToString(); }
            set
            {
                sCartItems = value;
                OnPropertyChanged();
            }
        }

        public string CartPieces
        {
            get { return iCartPieces.ToString(); }
            set
            {
                sCartPieces = value;
                OnPropertyChanged();
            }
        }

        public string CartTotal
        {
            get { return string.Format("{0:C}", dCartTotal); }
            set
            {
                sCartTotal = value;
                OnPropertyChanged();
            }
        }

        public ShoppingCartPage()
        {
            InitializeComponent();

            //BindingContext = _viewModel = new ShoppingCartViewModel();
            BindingContext = this;

            App.g_ShoppingCartPage = this;

            RefreshList();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Database db = new Database();
            lstItems = App.g_db.GetCartItems();

            if (lstItems.Count > 0)
            {
                App.g_CurrentPage = "ShoppingCartPage";

                RefreshList();
            }
            else
            {
                Dispatcher.Dispatch(async () =>
                {
                    await Shell.Current.Navigation.PopToRootAsync();
                    await App.g_Shell.GoToHome();
                    await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Your shopping cart is empty", "Ok");
                });
            }

            if (App.g_IsLoggedIn)
            {
                btnCheckout.IsVisible = true;
                btnSignIn.IsVisible = false;
            }
            else
            {
                btnCheckout.IsVisible = false;
                btnSignIn.IsVisible = true;
            }
        }

        public void UpdateTotals()
        {
            iCartItems = 0;
            iCartPieces = 0;
            dCartTotal = 0;

            foreach (Item item in (List<Item>)ItemsListCart.ItemsSource)
            {
                try
                {
                    if (item.QtyOrder > 0)
                    {
                        item.PriceOrder = item.Price;

                        iCartItems += 1;
                        dCartTotal += (item.PriceOrder * item.QtyOrder);
                        iCartPieces += item.QtyOrder;
                    }
                }
                catch { }
            }

            CartItems = iCartItems.ToString();
            CartPieces = iCartPieces.ToString();
            CartTotal = dCartTotal.ToString("{0:C2}");
        }

        public async void RefreshList()
        {
            ItemsListCart.ItemsSource = null;

            lstItems = App.g_db.GetCartItems();

            foreach (Item i in lstItems)
            {
                i.IsLoggedIn = App.g_IsLoggedIn;

                if (i.QtyOrder == 0)
                {
                    i.IsStepperVisible = false;
                    i.IsAddToOrderVisible = true;
                }
                else if (i.QtyOrder < 0)
                {
                    i.IsStepperVisible = false;
                    i.IsAddToOrderVisible = false;
                }
                else
                {
                    i.IsStepperVisible = true;
                    i.IsAddToOrderVisible = false;
                }

                i.IsQOHBlackVisible = false;
                i.IsQOHRedVisible = false;
                if (App.g_QOHDisplay == "Q")
                {
                    i.IsQOHVisible = true;
                    i.IsInStockVisible = false;
                    i.IsOutOfStockVisible = false;
                    if (i.QOH > 0)
                    {
                        i.IsQOHBlackVisible = true;
                    }
                    else
                    {
                        i.IsQOHRedVisible = true;
                    }
                }
                else if (App.g_QOHDisplay == "I")
                {
                    i.IsQOHVisible = false;
                    if (i.QOH > 0)
                    {
                        i.IsInStockVisible = true;
                        i.IsOutOfStockVisible = false;
                    }
                    else
                    {
                        i.IsInStockVisible = false;
                        i.IsOutOfStockVisible = true;
                    }
                }
                else
                {
                    i.IsQOHVisible = false;
                    i.IsInStockVisible = false;
                    i.IsOutOfStockVisible = false;
                }
                if (i.IsQOHVisible || i.IsInStockVisible || i.IsOutOfStockVisible)
                {
                    i.IsStockRowVisible = true;
                }
                else
                {
                    i.IsStockRowVisible = false;
                }
            }

            ItemsListCart.ItemsSource = lstItems;

            UpdateTotals();
        }

        private async void btnCheckout_Clicked(object sender, EventArgs e)
        {
            await App.g_Shell.GoToCheckout();
        }

        private async void btnSignIn_Clicked(object sender, EventArgs e)
        {
            await App.g_Shell.GoToLogin();
        }

        private async void btnClearCart_Clicked(object sender, EventArgs e)
        {
            bool bClear = await DisplayAlertAsync("Profit Order", "Are you sure you wish to remove all the items from your shopping cart?", "Yes", "No");

            if (bClear)
            {
                App.g_db.ClearCartItems();
                await App.g_Shell.GoToHome();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}