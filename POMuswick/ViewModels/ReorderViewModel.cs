using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Common;
using POMuswick.Models;
using POMuswick.Services;

namespace POMuswick.ViewModels
{
    public partial class ReorderViewModel : BaseViewModel
    {
        private readonly IItemService _itemService;
        private readonly ISettingService _settingService;
        [ObservableProperty]
        List<Item> _reorderList;

        public ReorderViewModel(IAppServices appServices) : base(appServices)
        {
            Title = PageTitles.ReorderItems;
            _itemService = appServices._itemService;
            _settingService = appServices._settingService;
        }

        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            await RefreshListAsync();
        }

        public async Task RefreshListAsync()
        {
            var itemResult = await _itemService.FetchReorderItemsAsync();
            var itemLookup = itemResult.items.ToDictionary(i => i.ItemNo);
            var itemsToProcess = await _itemService.FetchReorderItemsAsync();

            // Cache global flags to local variables so threads don't "fight" over App object access
            AppSettings appSettings = await _settingService.LoadSetting();
            string qohDisplay = appSettings.QOHDisplay;
            bool isLoggedIn = appSettings.IsLoggedIn;
            bool blockNoQoh = appSettings.BlockItemsNoQOH;

            // 2. PARALLEL PROCESSING
            // This utilizes all CPU cores to process the list simultaneously
            Parallel.ForEach(itemsToProcess.items, ri =>
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
            ReorderList = itemsToProcess.items;
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

        [RelayCommand]
        private async Task IncreaseQtyAsync(Item item)
        {
            if (item == null || item.QtyOrder >= 999)
                return;

            if (item.MaxOrderQty > 0 && item.QtyOrder >= item.MaxOrderQty)
                return;

            await _itemService.UpdateItemQtySet(item.ItemNo, 1);
            item.QtyOrder++;
            item.IsStepperVisible = true;
            item.IsAddToOrderVisible = false;
        }

        [RelayCommand]
        private async Task DecreaseQtyAsync(Item item)
        {
            if (item == null || item.QtyOrder <= 0)
                return;

            await _itemService.UpdateItemQtySet(item.ItemNo, -1);
            item.QtyOrder--;

            if (item.QtyOrder == 0)
            {
                item.IsStepperVisible = false;
                item.IsAddToOrderVisible = true;
            }
        }
    }
}