using CommunityToolkit.Mvvm.Messaging;
using POMuswick.Data;

namespace POMuswick.Views
{
    public partial class ShoppingCartPage : ContentPage
    {
        int iCartItems = 0;
        int iCartPieces = 0;
        decimal dCartTotal = 0;

        public bool _IsDeliveryHighlighted;
        public bool _IsPickupHighlighted;

        public Location _Location = new Location();

        string sCartItems;
        string sCartPieces;
        string sCartTotal;

        List<Item> lstItems = new List<Item>();

        public Boolean IsDeliveryHighlighted
        {
            get { return _IsDeliveryHighlighted; }
            set
            {
                _IsDeliveryHighlighted = value;
                OnPropertyChanged();
            }
        }

        public Boolean IsPickupHighlighted
        {
            get { return _IsPickupHighlighted; }
            set
            {
                _IsPickupHighlighted = value;
                OnPropertyChanged();
            }
        }

        public String CompanyName
        {
            get { return App.g_Customer.CompanyName; }
            set
            {
                App.g_Customer.CompanyName = value;
                OnPropertyChanged();
            }
        }

        public String CompanyAddress
        {
            get { return App.g_Customer.Address1; }
            set
            {
                App.g_Customer.Address1 = value;
                OnPropertyChanged();
            }
        }

        public String CompanyCityStateZip
        {
            get { return App.g_Customer.CityStateZip; }
            set
            {
                App.g_Customer.CityStateZip = value;
                OnPropertyChanged();
            }
        }

        public String LocationName
        {
            get { return _Location.Name; }
            set
            {
                _Location.Name = value;
                OnPropertyChanged();
            }
        }

        public String LocationAddress
        {
            get { return _Location.Address; }
            set
            {
                _Location.Address = value;
                OnPropertyChanged();
            }
        }

        public String LocationCityStateZip
        {
            get { return _Location.CityStateZip; }
            set
            {
                _Location.CityStateZip = value;
                OnPropertyChanged();
            }
        }

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

            DPStatus();

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

        private async void DPStatus()
        {
            if (App.g_Customer.Delivery != 1)
            {
                App.g_Customer = App.g_db.GetCustomer();

            }
            
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (App.g_Customer.Delivery == 1)
                {
                    IsDeliveryHighlighted = true;
                    IsPickupHighlighted = false;

                    CompanyName = App.g_Customer.CompanyName;
                    CompanyAddress = App.g_Customer.Address1;
                    CompanyCityStateZip = App.g_Customer.CityStateZip;
                }
                else
                {
                    IsDeliveryHighlighted = false;
                    IsPickupHighlighted = true;

                    CompanyName = App.g_Customer.CompanyName;
                    CompanyAddress = "Delivery Not Available";
                    CompanyCityStateZip = "";
                }
            }); 
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
            validate();            
        }

        private async void validate()
        {
            //await App.g_Shell.GoToCheckout();
            LoadingAlert.IsVisible = true;
            LoadingAlert.IsLoading = true;

            await Task.Delay(1000).ContinueWith(t =>
            {
                List<Item> lstCartItems = App.g_db.GetCartItems();
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

                ValidateResponse response = App.CommManager.ValidateOrderQOH(App.g_Customer.CustNo, sOrderInfo).Result;
                MainThread.BeginInvokeOnMainThread(async () => {
                    if (response.IsValid)
                    {
                        LoadingAlert.IsVisible = false;
                        if ((dCartTotal < App.g_Customer.MinOrderAmount) && (IsDeliveryHighlighted))
                        {
                            bool bContinue =  await DisplayAlertAsync("Muswick Wholesale Grocers", "Your order total must be at least " + string.Format("{0:C}", App.g_Customer.MinOrderAmount) + " to avoid a " + string.Format("{0:C}", App.g_Customer.ShippingFee) + " shipping fee.  Do you wish to continue?  Yes to continue and place order.  No to go back and add more items to your order.", "YES", "NO");

                            if (bContinue)
                            {
                                await App.g_Shell.GoToCheckout();
                                return;
                            }
                            else 
                            {
                                await App.g_Shell.GoToShoppingCart();
                            }
                        }
                        else { await App.g_Shell.GoToCheckout(); }
                    }
                    else
                    {
                        LoadingAlert.IsVisible = false;
                        await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", response.Message, "Ok");
                    }
                });
            });
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

        async void OnDelivery(object sender, EventArgs e)
        {
            if (App.g_Customer.Delivery == 1)
            {
                IsDeliveryHighlighted = true;
                IsPickupHighlighted = false;
            }
        }

        async void OnPickup(object sender, EventArgs e)
        {
            IsDeliveryHighlighted = false;
            IsPickupHighlighted = true;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}