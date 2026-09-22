using CommunityToolkit.Mvvm.ComponentModel;

namespace POMuswick.UIModels
{
    public partial class UIOrderDetail : ObservableObject
    {

        [ObservableProperty] public string _orderNo;
        [ObservableProperty] public int _itemNo;
        [ObservableProperty] public string _itemNoDisplay;
        [ObservableProperty] public int _lineNo;
        [ObservableProperty] public int _qtyOrdered;
        [ObservableProperty] public int _qtyShipped;
        [ObservableProperty] public string _description;
        [ObservableProperty] public decimal _price;
        [ObservableProperty] public string _priceDisplay;
        [ObservableProperty] public string _uOM;
        [ObservableProperty] public string _size;
        [ObservableProperty] public string _form;
        [ObservableProperty] public string _categoryCode;
        [ObservableProperty] public string _categoryDesc;
        [ObservableProperty] public string _subcategoryCode;
        [ObservableProperty] public string _subcategoryDesc;
        [ObservableProperty] public string _vendorId;
        [ObservableProperty] public string _vendorName;
        [ObservableProperty] public string _sellUnitsInPurch;
        [ObservableProperty] public string _sizeDisplay;
        [ObservableProperty] public string _uPC;
        [ObservableProperty] public string _itemNoDisplayUPC;
        [ObservableProperty] public string _imageURL;
        [ObservableProperty] public string _imageBase64;
        [ObservableProperty] public Boolean _isLoggedIn;
        [ObservableProperty] public int _rowHeight;
        [ObservableProperty] public Boolean _isStepperVisible;
        [ObservableProperty] public Boolean _isAddToOrderVisible;
        [ObservableProperty] public int _qtyOrder;
        [ObservableProperty] public string _sizeUOM;
        [ObservableProperty] public string _status;
        [ObservableProperty] public int _qOH;
        [ObservableProperty] public Boolean _isAvailable;
        [ObservableProperty] public Boolean _isQOHVisible;
        [ObservableProperty] public Boolean _isInStockVisible;
        [ObservableProperty] public Boolean _isOutOfStockVisible;
        [ObservableProperty] public Boolean _isStockRowVisible;
        [ObservableProperty] public Boolean _isQOHRedVisible;
        [ObservableProperty] public Boolean _isQOHBlackVisible;
        [ObservableProperty] public int _maxOrderQty;
        [ObservableProperty] public Boolean _isMaxOrderQtyVisible;
        [ObservableProperty] public string _maxOrderQtyDisplay;
    }
}
