namespace POMuswick.Views
{
    public partial class PurchaseHistoryDetailPage : ContentPage
    {
        OrderHeader _OrderHdr = new OrderHeader();

        public string OrderNo
        {
            get { return _OrderHdr.OrderNo; }
            set
            {
                _OrderHdr.OrderNo = value;
                OnPropertyChanged();
            }
        }
        public string OrderDateDisplay
        {
            get { return _OrderHdr.OrderDateDisplay; }
            set
            {
                _OrderHdr.OrderDateDisplay = value;
                OnPropertyChanged();
            }
        }
        public int Items
        {
            get { return _OrderHdr.Items; }
            set
            {
                _OrderHdr.Items = value;
                OnPropertyChanged();
            }
        }
        public int Pieces
        {
            get { return _OrderHdr.Pieces; }
            set
            {
                _OrderHdr.Pieces = value;
                OnPropertyChanged();
            }
        }
        public string TotalDisplay
        {
            get { return _OrderHdr.TotalDisplay; }
            set
            {
                _OrderHdr.TotalDisplay = value;
                OnPropertyChanged();
            }
        }

        public PurchaseHistoryDetailPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "PurchaseHistoryDetailPage";

            RefreshList();
        }

        public async void RefreshList()
        {
            OrderItemsList.ItemsSource = null;

            //Database db = new Database();
            _OrderHdr = App.g_db.GetOrderHeader(App.g_OrderNo);

            OrderNo = _OrderHdr.OrderNo;
            OrderDateDisplay = _OrderHdr.OrderDateDisplay;
            Items = _OrderHdr.Items;
            Pieces = _OrderHdr.Pieces;
            TotalDisplay = _OrderHdr.TotalDisplay;

            List<Item> lstItem = App.g_db.GetItems();

            OrderItemsList.ItemsSource = App.g_db.GetOrderDetail(App.g_OrderNo);

            foreach (OrderDetail d in (List<OrderDetail>)OrderItemsList.ItemsSource)
            {
                d.IsLoggedIn = App.g_IsLoggedIn;

                foreach (Item i in lstItem)
                {
                    if (d.ItemNo == i.ItemNo)
                    {
                        d.QtyOrder = i.QtyOrder;
                        d.MaxOrderQty = i.MaxOrderQty;
                        d.IsMaxOrderQtyVisible = i.IsMaxOrderQtyVisible;
                        d.MaxOrderQtyDisplay = i.MaxOrderQtyDisplay;
                        break;
                    }
                }

                if (d.QtyOrder == 0)
                {
                    d.IsStepperVisible = false;
                    d.IsAddToOrderVisible = true;
                }
                else
                {
                    d.IsStepperVisible = true;
                    d.IsAddToOrderVisible = false;
                }

                d.IsQOHRedVisible = false;
                d.IsQOHBlackVisible = false;
                if (App.g_QOHDisplay == "Q")
                {
                    d.IsQOHVisible = true;
                    d.IsInStockVisible = false;
                    d.IsOutOfStockVisible = false;
                    if (d.QOH > 0)
                    {
                        d.IsQOHBlackVisible = true;
                    }
                    else
                    {
                        d.IsQOHRedVisible = true;
                    }
                }
                else if (App.g_QOHDisplay == "I")
                {
                    d.IsQOHVisible = false;
                    if (d.QOH > 0)
                    {
                        d.IsInStockVisible = true;
                        d.IsOutOfStockVisible = false;
                    }
                    else
                    {
                        d.IsInStockVisible = false;
                        d.IsOutOfStockVisible = true;
                    }
                }
                else
                {
                    d.IsQOHVisible = false;
                    d.IsInStockVisible = false;
                    d.IsOutOfStockVisible = false;
                }
                if (d.IsQOHVisible || d.IsInStockVisible || d.IsOutOfStockVisible)
                {
                    d.IsStockRowVisible = true;
                }
                else
                {
                    d.IsStockRowVisible = false;
                }

                if (App.g_BlockItemsNoQOH)
                {
                    if (d.QOH == 0)
                    {
                        d.IsStepperVisible = false;
                        d.IsAddToOrderVisible = false;
                    }
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void OrderItemsList_ItemAppearing(object sender, Syncfusion.Maui.ListView.ItemAppearingEventArgs e)
        {
            OrderDetail item = (OrderDetail)e.DataItem;

            if (item.QtyOrder > 0)
            {
                item.IsStepperVisible = true;
                item.IsAddToOrderVisible = false;
            }
            else
            {
                item.IsStepperVisible = false;
                item.IsAddToOrderVisible = true;
            }
        }
    }
}

