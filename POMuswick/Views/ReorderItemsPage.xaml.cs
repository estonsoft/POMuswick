namespace POMuswick.Views
{
    public partial class ReorderItemsPage : ContentPage
    {
        public ReorderItemsPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "ReorderItemsPage";

            RefreshList();
        }

        public async void RefreshList()
        {
            ReorderItemsList.ItemsSource = null;
            ReorderItemsList.ItemsSource = App.g_ReorderItemList;

            foreach (Item ri in (List<Item>)ReorderItemsList.ItemsSource)
            {
                ri.IsLoggedIn = App.g_IsLoggedIn;

                foreach (Item i in App.g_ItemList)
                {
                    if (ri.ItemNo == i.ItemNo)
                    {
                        ri.QtyOrder = i.QtyOrder;
                        ri.MaxOrderQty = i.MaxOrderQty;
                        ri.IsMaxOrderQtyVisible = i.IsMaxOrderQtyVisible;
                        ri.MaxOrderQtyDisplay = i.MaxOrderQtyDisplay;
                        break;
                    }
                }

                if (ri.QtyOrder == 0)
                {
                    ri.IsStepperVisible = false;
                    ri.IsAddToOrderVisible = true;
                }
                else
                {
                    ri.IsStepperVisible = true;
                    ri.IsAddToOrderVisible = false;
                }

                ri.IsQOHRedVisible = false;
                ri.IsQOHBlackVisible = false;
                if (App.g_QOHDisplay == "Q")
                {
                    ri.IsQOHVisible = true;
                    ri.IsInStockVisible = false;
                    ri.IsOutOfStockVisible = false;
                    if (ri.QOH > 0)
                    {
                        ri.IsQOHBlackVisible = true;
                    }
                    else
                    {
                        ri.IsQOHRedVisible = true;
                    }
                }
                else if (App.g_QOHDisplay == "I")
                {
                    ri.IsQOHVisible = false;
                    if (ri.QOH > 0)
                    {
                        ri.IsInStockVisible = true;
                        ri.IsOutOfStockVisible = false;
                    }
                    else
                    {
                        ri.IsInStockVisible = false;
                        ri.IsOutOfStockVisible = true;
                    }
                }
                else
                {
                    ri.IsQOHVisible = false;
                    ri.IsInStockVisible = false;
                    ri.IsOutOfStockVisible = false;
                }
                if (ri.IsQOHVisible || ri.IsInStockVisible || ri.IsOutOfStockVisible)
                {
                    ri.IsStockRowVisible = true;
                }
                else
                {
                    ri.IsStockRowVisible = false;
                }

                if (App.g_BlockItemsNoQOH)
                {
                    if (ri.QOH == 0)
                    {
                        ri.IsStepperVisible = false;
                        ri.IsAddToOrderVisible = false;
                    }
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
