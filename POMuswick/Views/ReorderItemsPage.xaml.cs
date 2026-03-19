using System.Diagnostics;

namespace POMuswick.Views
{
    public partial class ReorderItemsPage : ContentPage
    {
        public ReorderItemsPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "ReorderItemsPage";

            Task.Run(async () =>
            {
                await Task.Delay(100); // Wait for page transition to finish
                await RefreshListAsync();
            });
        }

        public async Task RefreshListAsync()
        {
            try
            {
                // 1. PREPARATION (Fastest possible lookups)
                // Convert the global list to a Dictionary once. 
                // This prevents the nested loop lag (N*M becomes N+M).
                var itemLookup = App.g_ItemList.ToDictionary(i => i.ItemNo);
                var itemsToProcess = App.g_ReorderItemList.ToList();

                // Cache global flags to local variables so threads don't "fight" over App object access
                string qohDisplay = App.g_QOHDisplay;
                bool isLoggedIn = App.g_IsLoggedIn;
                bool blockNoQoh = App.g_BlockItemsNoQOH;

                // 2. PARALLEL PROCESSING
                // This utilizes all CPU cores to process the list simultaneously
                Parallel.ForEach(itemsToProcess, ri =>
                {
                    ri.IsLoggedIn = isLoggedIn;

                    // Instant lookup via Dictionary
                    if (itemLookup.TryGetValue(ri.ItemNo, out var matchingItem))
                    {
                        ri.QtyOrder = matchingItem.QtyOrder;
                        ri.MaxOrderQty = matchingItem.MaxOrderQty;
                        ri.IsMaxOrderQtyVisible = matchingItem.IsMaxOrderQtyVisible;
                        ri.MaxOrderQtyDisplay = matchingItem.MaxOrderQtyDisplay;
                    }

                    // Visibility Logic
                    ri.IsStepperVisible = ri.QtyOrder != 0;
                    ri.IsAddToOrderVisible = ri.QtyOrder == 0;

                    // Optimized Stock Logic
                    ProcessStockLogic(ri, qohDisplay);

                    // Global restriction check
                    if (blockNoQoh && ri.QOH == 0)
                    {
                        ri.IsStepperVisible = false;
                        ri.IsAddToOrderVisible = false;
                    }
                });

                // 3. UI UPDATE (Main Thread)
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Assigning the fully processed list to the UI
                    ReorderItemsList.ItemsSource = itemsToProcess;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Critical Error in RefreshList: {ex.Message}");
            }
        }

        private void ProcessStockLogic(Item ri, string qohDisplay)
        {
            // Reset all visibility flags efficiently
            ri.IsQOHRedVisible = false;
            ri.IsQOHBlackVisible = false;
            ri.IsQOHVisible = false;
            ri.IsInStockVisible = false;
            ri.IsOutOfStockVisible = false;

            if (qohDisplay == "Q")
            {
                ri.IsQOHVisible = true;
                if (ri.QOH > 0) ri.IsQOHBlackVisible = true;
                else ri.IsQOHRedVisible = true;
            }
            else if (qohDisplay == "I")
            {
                if (ri.QOH > 0) ri.IsInStockVisible = true;
                else ri.IsOutOfStockVisible = true;
            }

            ri.IsStockRowVisible = ri.IsQOHVisible || ri.IsInStockVisible || ri.IsOutOfStockVisible;
        }

        protected override bool OnBackButtonPressed() => true;
    }
}
