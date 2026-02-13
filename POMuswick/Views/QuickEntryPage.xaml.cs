using POMuswick.ViewModels;


namespace POMuswick.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuickEntryPage : ContentPage
    {
        int iQty = 1;
        string sQty = "";
        int iItemNo = 0;
        int iMaxQty = 0;

        public ScanditViewModelBase viewModel = null;

        public string Qty
        {
            get { return iQty.ToString(); }
            set
            {
                sQty = value;
                OnPropertyChanged();
            }
        }

        public QuickEntryPage()
        {
            try
            {
                this.InitializeComponent();
            }
            catch
            {
            }

            if (App.g_ScanditViewModel == null)
            {
                App.g_ScanditViewModel = new ScanditViewModelBase();
            }

            this.viewModel = App.g_ScanditViewModel;
            BindingContext = viewModel;
            viewModel.ScannerPage = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "QuickEntryPage";

            ClearItemInfo();

            if (App.g_QOHDisplay == "Q")
            {
                StockRow.IsVisible = true;
                QOHLabel.IsVisible = true;
                QOHRed.IsVisible = false;
                QOHBlack.IsVisible = false;
                QOHInStock.IsVisible = false;
                QOHOutOfStock.IsVisible = false;
            }
            else if (App.g_QOHDisplay == "I")
            {
                StockRow.IsVisible = true;
                QOHLabel.IsVisible = true;
                QOHRed.IsVisible = false;
                QOHBlack.IsVisible = false;
                QOHInStock.IsVisible = false;
                QOHOutOfStock.IsVisible = false;
            }
            else
            {
                StockRow.IsVisible = false;
                QOHLabel.IsVisible = false;
                QOHRed.IsVisible = false;
                QOHBlack.IsVisible = false;
                QOHInStock.IsVisible = false;
                QOHOutOfStock.IsVisible = false;
            }

            await Task.Delay(100);
            EntryFocus();

            _ = this.viewModel.OnResumeAsync();
        }

        protected override void OnDisappearing()
        {
            try
            {
                _ = this.viewModel.OnSleep();
                base.OnDisappearing();
                Content = null;
            }
            catch
            {
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void MinusButton_Clicked(object sender, EventArgs e)
        {
            if (iQty > 1)
            {
                iQty--;
                QtyStepper.Text = iQty.ToString();
            }
        }

        private void PlusButton_Clicked(object sender, EventArgs e)
        {
            if (iQty == 999)
            {
                return;
            }

            if (iQty + 1 > iMaxQty)
            {
                return;
            }

            if (iQty < 999)
            {
                iQty++;
                QtyStepper.Text = iQty.ToString();
            }
        }

        private void AddToOrderButton_Clicked(object sender, EventArgs e)
        {
            //Database db = new Database();

            if (AddToOrderButton.IsVisible)
            {
                SetMessage("Item Added To Shopping Cart");
            }
            else
            {
                SetMessage("Shopping Cart Qty Updated");
            }

            App.g_db.UpdateItemQtySet(iItemNo, iQty);

            //ClearItemInfo();

            EntryFocus();
        }

        private void EntryFocus()
        {
            //ScanItem.Text = "";

            //Task.Delay(600);
            //ScanItem.Focus();
        }

        private void ShowItemInfo(Item item)
        {
            ScanItem.Text = "";

            ImageURL.Source = item.ImageURL;
            try
            {
                if (item.LongDescription != "")
                {
                    Description.Text = item.LongDescription;
                }
                else
                {
                    Description.Text = item.Description;
                }
            }
            catch
            {
                Description.Text = item.Description;
            }
            ItemNoDisplay.Text = item.ItemNoDisplay;
            iItemNo = item.ItemNo;
            ItemNoDisplayUPC.Text = item.ItemNoDisplayUPC;
            SizeUOM.Text = item.SizeUOM;
            PriceDisplay.Text = item.PriceDisplay;
            Message.IsVisible = false;

            QtyStepper.IsVisible = true;
            MinusButton.IsVisible = true;
            PlusButton.IsVisible = true;
            QuickEntryStepper.IsVisible = true;

            if (item.QtyOrder == 0)
            {
                iQty = 1;
                QtyStepper.Text = iQty.ToString();
                AddToOrderButton.IsVisible = true;
                UpdateOrderButton.IsVisible = false;
            }
            else
            {
                iQty = item.QtyOrder;
                QtyStepper.Text = item.QtyOrder.ToString();
                AddToOrderButton.IsVisible = false;
                UpdateOrderButton.IsVisible = true;
            }

            QOHBlack.IsVisible = false;
            QOHRed.IsVisible = false;
            QOHLabel.IsVisible = false;
            QOHInStock.IsVisible = false;
            QOHOutOfStock.IsVisible = false;
            if (App.g_QOHDisplay == "Q")
            {
                QOHLabel.Text = "QOH:";
                if (item.QOH > 0)
                {
                    QOHLabel.IsVisible = true;
                    QOHBlack.IsVisible = true;
                    QOHBlack.Text = item.QOH.ToString();
                }
                else
                {
                    QOHLabel.IsVisible = true;
                    QOHRed.IsVisible = true;
                    QOHRed.Text = "0";
                }
            }
            else if (App.g_QOHDisplay == "I")
            {
                QOHLabel.Text = "QOH:";
                if (item.QOH > 0)
                {
                    QOHInStock.IsVisible = true;
                }
                else
                {
                    QOHOutOfStock.IsVisible = true;
                }
            }

            if (App.g_BlockItemsNoQOH)
            {
                if (item.QOH <= 0)
                {
                    QuickEntryStepper.IsVisible = false;
                    AddToOrderButton.IsVisible = false;
                    UpdateOrderButton.IsVisible = false;
                }
            }

            iMaxQty = item.MaxOrderQty;
            if ((item.MaxOrderQty > 0) && (item.MaxOrderQty < 9999))
            {
                MaxOrderQty.IsVisible = true;
                MaxOrderQty.Text = "Max " + item.MaxOrderQty.ToString();
            }
        }

        private void ClearItemInfo()
        {
            ScanItem.Text = "";

            ImageURL.Source = "";
            //Description.Text = "";
            ItemNoDisplay.Text = "";
            //iItemNo = 0;
            ItemNoDisplayUPC.Text = "";
            SizeUOM.Text = "";
            PriceDisplay.Text = "";
            QuickEntryStepper.IsVisible = false;
            AddToOrderButton.IsVisible = false;
            Message.IsVisible = false;

            QtyStepper.IsVisible = false;
            MinusButton.IsVisible = false;
            PlusButton.IsVisible = false;
            QuickEntryStepper.IsVisible = false;
            AddToOrderButton.IsVisible = false;
            UpdateOrderButton.IsVisible = false;

            QOHLabel.IsVisible = false;
            QOHRed.IsVisible = false;
            QOHBlack.IsVisible = false;
            QOHInStock.IsVisible = false;
            QOHOutOfStock.IsVisible = false;
            MaxOrderQty.IsVisible = false;
        }

        private void SetMessage(string sMessage)
        {
            ClearItemInfo();
            Message.Text = sMessage;
            Message.IsVisible = true;
        }

        public void ScanComplete()
        {
            viewModel.OnSleep();

            TapToScan.IsVisible = true;

            Item item = FindItem();

            if (item == null)
            {
                ClearItemInfo();
                Description.Text = "";
                SetMessage("Item Not Found " + ScanItem.Text);
                ScanItem.Text = "";
                EntryFocus();
                return;
            }

            if (App.g_db.GetItemQty(item.ItemNo) > 0)
            {
                SetMessage("Item Already In Shopping Cart");
            }

            ShowItemInfo(item);
        }

        private void ScanItem_Completed(object sender, EventArgs e)
        {
            ScanComplete();
        }

        public void SetScanItem(string barcode)
        {
            ScanItem.Text = barcode;
        }

        private void EnterButton_Clicked(object sender, EventArgs e)
        {
            Message.Text = "";
            ScanComplete();
        }

        private void ScanItem_CompletedOld(object sender, EventArgs e)
        {
            Item item = FindItem();

            if (item == null)
            {
                ClearItemInfo();
                Description.Text = "";
                SetMessage("Item Not Found " + ScanItem.Text);
                ScanItem.Text = "";
                EntryFocus();
                return;
            }

            //Database db = new Database();

            if (App.g_db.GetItemQty(item.ItemNo) > 0)
            {
                ClearItemInfo();
                //Description.Text = item.LongDescription;
                SetMessage("Item Already In Shopping Cart");
                EntryFocus();
                return;
            }

            ShowItemInfo(item);
        }

        private Item FindItem()
        {
            string ScanText = ScanItem.Text.Trim();

            if (ScanText == "")
            {
                return null;
            }

            Item item = null;
            List<Item> items = new List<Item>();
            int ItemNo = 0;
            int.TryParse(ScanItem.Text, out ItemNo);

            if (ItemNo > 0)
            {
                item = App.g_db.FindItem(ItemNo);
            }

            if (item == null)
            {
                items = App.g_db.SearchItemsQuickEntry(ScanText);

                if (items.Count >= 1)
                {
                    item = items[0];
                }
            }

            return item;
        }

        async void OnScannerEnable(object sender, EventArgs e)
        {
            ClearItemInfo();
            TapToScan.IsVisible = false;
            _ = this.viewModel.OnResumeAsync();
            ScanItem.Text = "";
            Description.Text = "";
            Message.Text = "";
        }

        async void OnShowImage(object sender, EventArgs e)
        {
            ClearItemInfo();
            TapToScan.IsVisible = false;
            _ = this.viewModel.OnResumeAsync();
            ScanItem.Text = "";
            Description.Text = "";
            Message.Text = "";
        }

        void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            // Handle the pinch
        }
    }
}
