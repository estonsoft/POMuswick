using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Models;
using POMuswick.Services;

namespace POMuswick.ViewModels
{
    public partial class QuickEntryViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ISettingService _settingService;
        private readonly IItemService _itemService;
        private AppSettings appSetting;
        [ObservableProperty]
        int iQty = 1;
        [ObservableProperty]
        string sQty = "";

        [ObservableProperty]
        int iItemNo = 0;
        [ObservableProperty]
        int iMaxQty = 0;

        [ObservableProperty]
        bool _stockRowIsVisible;
        [ObservableProperty]
        bool _qOHLabelIsVisible;
        [ObservableProperty]
        bool _qOHRedIsVisible;
        [ObservableProperty]
        bool _qOHBlackIsVisible;
        [ObservableProperty]
        bool _qOHInStockIsVisible;
        [ObservableProperty]
        bool _qOHOutOfStockIsVisible;

        [ObservableProperty]
        string _descriptionText;
        [ObservableProperty]
        string _messageText;
        [ObservableProperty]
        string _qOHLabelText;
        [ObservableProperty]
        int _qOH;
        [ObservableProperty]
        string _qtyStepperText;
        [ObservableProperty]
        string _maxOrderQtyText;

        [ObservableProperty]
        string _scanItemText;
        [ObservableProperty]
        ImageSource _imageURLSource;
        [ObservableProperty]
        string _itemNoDisplayText;
        [ObservableProperty]
        string _itemNoDisplayUPCText;
        [ObservableProperty]
        string _categoryDesc;
        [ObservableProperty]
        string _sizeUOMText;
        [ObservableProperty]
        string _priceDisplayText;
        [ObservableProperty]
        bool _qtyStepperIsVisible;
        [ObservableProperty]
        bool _minusButtonIsVisible;
        [ObservableProperty]
        bool _plusButtonIsVisible;
        [ObservableProperty]
        bool _quickEntryStepperIsVisible;
        [ObservableProperty]
        bool _addToOrderButtonIsVisible;
        [ObservableProperty]
        bool _updateOrderButtonIsVisible;
        [ObservableProperty]
        bool _messageIsVisible;
        [ObservableProperty]
        bool _tapToScanIsVisible;

        [ObservableProperty]
        bool _maxOrderQtyIsVisible = false;

        public QuickEntryViewModel(IAppServices appServices) : base(appServices)
        {
            Title = "Scan";
            _navigationService = appServices._navigationService;
            _settingService = appServices._settingService;
            _itemService = appServices._itemService;
        }

        public async override Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            appSetting = await _settingService.LoadSetting();
            await RequestCameraPermission();
            ClearItemInfo();
            await SetInfo();
        }

        private async Task SetInfo()
        {
            if (appSetting.QOHDisplay == "Q")
            {
                StockRowIsVisible = true;
                QOHLabelIsVisible = true;
                QOHRedIsVisible = false;
                QOHBlackIsVisible = false;
                QOHInStockIsVisible = false;
                QOHOutOfStockIsVisible = false;
            }
            else if (appSetting.QOHDisplay == "I")
            {
                StockRowIsVisible = true;
                QOHLabelIsVisible = true;
                QOHRedIsVisible = false;
                QOHBlackIsVisible = false;
                QOHInStockIsVisible = false;
                QOHOutOfStockIsVisible = false;
            }
            else
            {
                StockRowIsVisible = false;
                QOHLabelIsVisible = false;
                QOHRedIsVisible = false;
                QOHBlackIsVisible = false;
                QOHInStockIsVisible = false;
                QOHOutOfStockIsVisible = false;
            }
        }
        private void ClearItemInfo()
        {
            ScanItemText = "";
            DescriptionText = string.Empty;
            MessageText = string.Empty;
            QtyStepperText = string.Empty;
            MaxOrderQtyText = string.Empty;
            QOHLabelText = string.Empty;
            QOH = 0;

            ImageURLSource = null;
            ItemNoDisplayText = "";
            ItemNoDisplayUPCText = "";
            SizeUOMText = "";
            PriceDisplayText = "";
            QuickEntryStepperIsVisible = false;
            AddToOrderButtonIsVisible = false;
            MessageIsVisible = false;

            QtyStepperIsVisible = false;
            MinusButtonIsVisible = false;
            PlusButtonIsVisible = false;
            QuickEntryStepperIsVisible = false;
            AddToOrderButtonIsVisible = false;
            UpdateOrderButtonIsVisible = false;

            QOHLabelIsVisible = false;
            QOHRedIsVisible = false;
            QOHBlackIsVisible = false;
            QOHInStockIsVisible = false;
            QOHOutOfStockIsVisible = false;
            MaxOrderQtyIsVisible = false;
        }

        private void ShowItemInfo(Item item)
        {
            ScanItemText = "";
            ImageURLSource = item.ImageURL;
            try
            {
                if (item.LongDescription != "")
                {
                    DescriptionText = item.LongDescription;
                }
                else
                {
                    DescriptionText = item.Description;
                }
            }
            catch
            {
                DescriptionText = item.Description;
            }
            ItemNoDisplayText = item.ItemNoDisplay;
            IItemNo = item.ItemNo;
            ItemNoDisplayUPCText = item.ItemNoDisplayUPC;
            CategoryDesc = item.CategoryDesc;
            SizeUOMText = item.SizeUOM;
            PriceDisplayText = item.PriceDisplay;
            MessageIsVisible = false;

            QtyStepperIsVisible = true;
            MinusButtonIsVisible = true;
            PlusButtonIsVisible = true;
            QuickEntryStepperIsVisible = true;

            if (item.QtyOrder == 0)
            {
                IQty = 1;
                QtyStepperText = IQty.ToString();
                AddToOrderButtonIsVisible = true;
                UpdateOrderButtonIsVisible = false;
            }
            else
            {
                IQty = item.QtyOrder;
                QtyStepperText = item.QtyOrder.ToString();
                AddToOrderButtonIsVisible = false;
                UpdateOrderButtonIsVisible = true;
            }

            QOHBlackIsVisible = false;
            QOHRedIsVisible = false;
            QOHLabelIsVisible = false;
            QOHInStockIsVisible = false;
            QOHOutOfStockIsVisible = false;
            QOH = item.QOH;
            if (appSetting.QOHDisplay == "Q")
            {
                QOHLabelText = "QOH:";
                if (item.QOH > 0)
                {
                    QOHLabelIsVisible = true;
                    QOHBlackIsVisible = true;
                }
                else
                {
                    QOHLabelIsVisible = true;
                    QOHRedIsVisible = true;
                }
            }
            else if (appSetting.QOHDisplay == "I")
            {
                QOHLabelText = "QOH:";
                if (item.QOH > 0)
                {
                    QOHInStockIsVisible = true;
                }
                else
                {
                    QOHOutOfStockIsVisible = true;
                }
            }

            if (appSetting.BlockItemsNoQOH)
            {
                if (item.QOH <= 0)
                {
                    QuickEntryStepperIsVisible = false;
                    AddToOrderButtonIsVisible = false;
                    UpdateOrderButtonIsVisible = false;
                }
            }

            IMaxQty = item.MaxOrderQty;
            if ((item.MaxOrderQty > 0) && (item.MaxOrderQty < 9999))
            {
                MaxOrderQtyIsVisible = true;
                MaxOrderQtyText = "Max " + item.MaxOrderQty.ToString();
            }
        }

        [RelayCommand]
        private void MinusButtonAsync()
        {
            if (IQty > 1)
            {
                IQty--;
                QtyStepperText = IQty.ToString();
            }
        }

        [RelayCommand]
        private void PlusButtonAsync()
        {
            if (IQty == 999)
            {
                return;
            }

            if (IQty + 1 > IMaxQty)
            {
                return;
            }

            if (IQty < 999)
            {
                IQty++;
                QtyStepperText = IQty.ToString();
            }
        }

        [RelayCommand]
        private void AddToOrderButtonAsync()
        {
            if (AddToOrderButtonIsVisible)
            {
                SetMessage("Item Added To Shopping Cart");
            }
            else
            {
                SetMessage("Shopping Cart Qty Updated");
            }
            _itemService.UpdateItemQtySet(IItemNo, IQty);
        }

        private void SetMessage(string sMessage)
        {
            ClearItemInfo();
            MessageText = sMessage;
            MessageIsVisible = true;
        }

        public async Task ScanComplete()
        {
            TapToScanIsVisible = true;
            Item item = await FindItem();
            if (item == null)
            {
                ClearItemInfo();
                DescriptionText = "";
                SetMessage("Item Not Found " + ScanItemText);
                ScanItemText = "";
                return;
            }
            if (await _itemService.GetItemQty(item.ItemNo) > 0)
            {
                SetMessage("Item Already In Shopping Cart");
            }
            ShowItemInfo(item);
        }

        [RelayCommand]
        private async Task ScanItemCompletedAsync()
        {
            await ScanComplete();
        }

        public void SetScanItem(string barcode)
        {
            ScanItemText = barcode;
        }

        [RelayCommand]
        private async Task EnterButtonAsync()
        {
            MessageText = "";
            await ScanComplete();
        }

        private async Task<Item> FindItem()
        {
            string ScanText = ScanItemText.Trim();

            if (ScanText == "")
            {
                return null;
            }

            Item item = null;
            List<Item> items = new List<Item>();
            int ItemNo = 0;
            int.TryParse(ScanItemText, out ItemNo);

            if (ItemNo > 0)
            {
                item = await _itemService.GetItemByItemNo(ItemNo);
            }

            if (item == null)
            {
                items = await _itemService.SearchItemsQuickEntry(ScanText);

                if (items.Count >= 1)
                {
                    item = items[0];
                }
            }

            return item;
        }

        [RelayCommand]
        async Task OnScannerEnableAsync()
        {
            ClearItemInfo();
            TapToScanIsVisible = false;
            ScanItemText = "";
            DescriptionText = "";
            MessageText = "";
        }

        [RelayCommand]
        async Task ShowImageAsync()
        {
            ClearItemInfo();
            TapToScanIsVisible = false;
            ScanItemText = "";
            DescriptionText = "";
            MessageText = "";
        }
    }
}