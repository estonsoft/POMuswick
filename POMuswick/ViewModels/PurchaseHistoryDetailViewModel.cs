using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Common;
using POMuswick.Models;
using POMuswick.Services;
using POMuswick.UIModels;

namespace POMuswick.ViewModels
{
    public partial class PurchaseHistoryDetailViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IItemService _itemService;
        private readonly ISettingService _settingService;
        private readonly IOrderHistoryService _orderHistoryService;

        [ObservableProperty]
        public OrderHeader _ordHeader;
        [ObservableProperty]
        public List<UIOrderDetail> _orderDetailList;

        [ObservableProperty]
        public string _orderNo;
        [ObservableProperty]
        public string _orderDateDisplay;
        [ObservableProperty]
        public int _items;
        [ObservableProperty]
        public int _pieces;
        [ObservableProperty]
        public string _totalDisplay;

        [ObservableProperty]
        public ImageSource _selectedImage;
        [ObservableProperty]
        public bool _showImage;
        public PurchaseHistoryDetailViewModel(IAppServices appServices) : base(appServices)
        {
            Title = PageTitles.OrderDetail;
            _dialogService = appServices._dialogService;
            _navigationService = appServices._navigationService;
            _itemService = appServices._itemService;
            _settingService = appServices._settingService;
            _orderHistoryService = appServices._orderHistoryService;
        }

        public async override Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("OrderNo", out var searchValue))
            {
                OrderNo = (string)searchValue;
            }
            RefreshList();
        }

        public async void RefreshList()
        {
            OrdHeader = await _orderHistoryService.FetchOrderHeaderAsync(OrderNo);
            OrderNo = OrdHeader.OrderNo;
            OrderDateDisplay = OrdHeader.OrderDateDisplay;
            Items = OrdHeader.Items;
            Pieces = OrdHeader.Pieces;
            TotalDisplay = OrdHeader.TotalDisplay;

            var itemResult = await _itemService.FetchNewItemAsync(false);
            List<Item> lstItem = itemResult.items;

            var orderDetails = await _orderHistoryService.FetchOrderDetailsAsync(OrderNo);
            List<UIOrderDetail> uiList = orderDetails
                .Select(x => x.ToUI())
                .ToList();
            OrderDetailList = uiList;

            AppSettings appSettings = await _settingService.LoadSetting();
            var itemLookup = lstItem.ToDictionary(x => x.ItemNo);

            foreach (var d in OrderDetailList)
            {
                d.IsLoggedIn = appSettings.IsLoggedIn;

                // Sync Item information
                if (itemLookup.TryGetValue(d.ItemNo, out var item))
                {
                    d.QtyOrder = item.QtyOrder;
                    d.MaxOrderQty = item.MaxOrderQty;
                    d.IsMaxOrderQtyVisible = item.IsMaxOrderQtyVisible;
                    d.MaxOrderQtyDisplay = item.MaxOrderQtyDisplay;
                }

                // Order visibility
                d.IsStepperVisible = d.QtyOrder > 0;
                d.IsAddToOrderVisible = d.QtyOrder == 0;

                // Reset stock indicators
                d.IsQOHVisible = false;
                d.IsQOHBlackVisible = false;
                d.IsQOHRedVisible = false;
                d.IsInStockVisible = false;
                d.IsOutOfStockVisible = false;

                switch (appSettings.QOHDisplay)
                {
                    case "Q":
                        d.IsQOHVisible = true;

                        if (d.QOH > 0)
                            d.IsQOHBlackVisible = true;
                        else
                            d.IsQOHRedVisible = true;

                        break;

                    case "I":
                        if (d.QOH > 0)
                            d.IsInStockVisible = true;
                        else
                            d.IsOutOfStockVisible = true;

                        break;
                }

                d.IsStockRowVisible =
                    d.IsQOHVisible ||
                    d.IsInStockVisible ||
                    d.IsOutOfStockVisible;

                // Hide ordering if no stock
                if (appSettings.BlockItemsNoQOH && d.QOH == 0)
                {
                    d.IsStepperVisible = false;
                    d.IsAddToOrderVisible = false;
                }
            }
        }
        [RelayCommand]
        private async Task ShowImageAsync(ImageSource cachedImage)
        {
            SelectedImage = cachedImage;
            ShowImage = true;
        }

        [RelayCommand]
        public async Task IncreaseQtyAsync(UIOrderDetail item)
        {
            if (item == null)
                return;

            if (item.QtyOrder >= 999)
                return;

            if (item.MaxOrderQty > 0 &&
                item.QtyOrder >= item.MaxOrderQty)
                return;

            await _itemService.UpdateItemQtySet(item.ItemNo, 1);

            item.QtyOrder++;

            item.IsStepperVisible = true;
            item.IsAddToOrderVisible = false;
            await Task.CompletedTask;
        }

        [RelayCommand]
        public async Task DecreaseQtyAsync(UIOrderDetail item)
        {
            if (item == null)
                return;

            if (item.QtyOrder <= 0)
                return;

            await _itemService.UpdateItemQtySet(item.ItemNo, -1);

            item.QtyOrder--;

            if (item.QtyOrder == 0)
            {
                item.IsStepperVisible = false;
                item.IsAddToOrderVisible = true;
            }
            await Task.CompletedTask;
        }

        [RelayCommand]
        public async Task CloseImageAsync()
        {
            ShowImage = false;
        }

        // private void OrderItemsList_ItemAppearing(object sender, Syncfusion.Maui.ListView.ItemAppearingEventArgs e)
        // {
        //     OrderDetail item = (OrderDetail)e.DataItem;

        //     if (item.QtyOrder > 0)
        //     {
        //         item.IsStepperVisible = true;
        //         item.IsAddToOrderVisible = false;
        //     }
        //     else
        //     {
        //         item.IsStepperVisible = false;
        //         item.IsAddToOrderVisible = true;
        //     }
        // }

    }
    public static class OrderDetailMapper
    {
        public static UIOrderDetail ToUI(this OrderDetail item)
        {
            return new UIOrderDetail
            {
                OrderNo = item.OrderNo,
                ItemNo = item.ItemNo,
                ItemNoDisplay = item.ItemNoDisplay,
                LineNo = item.LineNo,
                QtyOrdered = item.QtyOrdered,
                QtyShipped = item.QtyShipped,
                Description = item.Description,
                Price = item.Price,
                PriceDisplay = item.PriceDisplay,
                UOM = item.UOM,
                Size = item.Size,
                Form = item.Form,
                CategoryCode = item.CategoryCode,
                CategoryDesc = item.CategoryDesc,
                SubcategoryCode = item.SubcategoryCode,
                SubcategoryDesc = item.SubcategoryDesc,
                VendorId = item.VendorId,
                VendorName = item.VendorName,
                SellUnitsInPurch = item.SellUnitsInPurch,
                SizeDisplay = item.SizeDisplay,
                UPC = item.UPC,
                ItemNoDisplayUPC = item.ItemNoDisplayUPC,
                ImageURL = item.ImageURL,
                ImageBase64 = item.ImageBase64,
                IsLoggedIn = item.IsLoggedIn,
                RowHeight = item.RowHeight,
                IsStepperVisible = item.IsStepperVisible,
                IsAddToOrderVisible = item.IsAddToOrderVisible,
                QtyOrder = item.QtyOrder,
                SizeUOM = item.SizeUOM,
                Status = item.Status,
                QOH = item.QOH,
                IsAvailable = item.IsAvailable,
                IsQOHVisible = item.IsQOHVisible,
                IsInStockVisible = item.IsInStockVisible,
                IsOutOfStockVisible = item.IsOutOfStockVisible,
                IsStockRowVisible = item.IsStockRowVisible,
                IsQOHRedVisible = item.IsQOHRedVisible,
                IsQOHBlackVisible = item.IsQOHBlackVisible,
                MaxOrderQty = item.MaxOrderQty,
                IsMaxOrderQtyVisible = item.IsMaxOrderQtyVisible,
                MaxOrderQtyDisplay = item.MaxOrderQtyDisplay
            };
        }
    }
}

