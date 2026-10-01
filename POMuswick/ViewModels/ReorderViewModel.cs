using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Common;
using POMuswick.Models;
using POMuswick.Services;
using POMuswick.UIModels;

namespace POMuswick.ViewModels
{
    public partial class ReorderViewModel : BaseViewModel
    {
        private readonly IItemService _itemService;
        private readonly ISettingService _settingService;
        [ObservableProperty]
        List<UIItems> _reorderList = new();

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
            var sourceItems = (await _itemService.FetchReorderItemsAsync()).items;
            var itemLookup = sourceItems.ToDictionary(i => i.ItemNo);

            // Cache global flags to local variables so threads don't "fight" over App object access
            AppSettings appSettings = await _settingService.LoadSetting();
            string qohDisplay = appSettings.QOHDisplay;
            bool isLoggedIn = appSettings.IsLoggedIn;
            bool blockNoQoh = appSettings.BlockItemsNoQOH;

            var reorderItems = new List<UIItems>(sourceItems.Count);
            foreach (var sourceItem in sourceItems)
            {
                var ri = sourceItem.ToUI();
                ri.IsLoggedIn = isLoggedIn;

                // Instant lookup via Dictionary
                if (itemLookup.TryGetValue(ri.ItemNo, out var matchingItem))
                {
                    ri.ImageURL = matchingItem.ImageURL;
                    ri.QtyOrder = matchingItem.QtyOrder;
                    ri.MaxOrderQty = matchingItem.MaxOrderQty;
                    ri.IsMaxOrderQtyVisible = matchingItem.IsMaxOrderQtyVisible;
                    ri.MaxOrderQtyDisplay = matchingItem.MaxOrderQtyDisplay;
                }

                if (string.IsNullOrWhiteSpace(ri.ImageURL))
                    ri.ImageURL = $"{Constants.ItemImageUrl}{ri.ItemNo}.jpg";

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
                reorderItems.Add(ri);
            }

            ReorderList = reorderItems;
        }

        private void ProcessStockLogic(UIItems ri, string qohDisplay)
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
        private async Task IncreaseQtyAsync(UIItems item)
        {
            if (item == null || item.QtyOrder >= 999)
                return;

            if (item.MaxOrderQty > 0 && item.QtyOrder >= item.MaxOrderQty)
                return;

            var newQuantity = item.QtyOrder + 1;
            await _itemService.UpdateItemQtySet(item.ItemNo, newQuantity);
            item.QtyOrder = newQuantity;
            item.IsStepperVisible = true;
            item.IsAddToOrderVisible = false;
        }

        [RelayCommand]
        private async Task DecreaseQtyAsync(UIItems item)
        {
            if (item == null || item.QtyOrder <= 0)
                return;

            var newQuantity = item.QtyOrder - 1;
            await _itemService.UpdateItemQtySet(item.ItemNo, newQuantity);
            item.QtyOrder = newQuantity;

            if (item.QtyOrder == 0)
            {
                item.IsStepperVisible = false;
                item.IsAddToOrderVisible = true;
            }
        }
    }
}