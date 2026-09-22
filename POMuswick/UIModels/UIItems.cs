using CommunityToolkit.Mvvm.ComponentModel;

namespace POMuswick.UIModels
{
    public partial class UIItems : ObservableObject
    {
        
        [ObservableProperty] public int _itemNo ;
        [ObservableProperty] public string _itemNoDisplay ;
        [ObservableProperty] public string _itemNoDisplayUPC ;
        [ObservableProperty] public int _qty ;
        [ObservableProperty] public string _qtyDisplay ;
        [ObservableProperty] public string _description ;
        [ObservableProperty] public string _imageURL ;
        [ObservableProperty] public string _imageBase64 ;
        [ObservableProperty] public string _categoryCode ;
        [ObservableProperty] public string _categoryDesc ;
        [ObservableProperty] public string _subcategoryCode ;
        [ObservableProperty] public string _subcategoryDesc ;
        [ObservableProperty] public string _vendorCode ;
        [ObservableProperty] public string _vendorName ;
        [ObservableProperty] public string _uOM ;
        [ObservableProperty] public int _size ;
        [ObservableProperty] public string _sizeDisplay ;
        [ObservableProperty] public string _form ;
        [ObservableProperty] public string _retailUOM ;
        [ObservableProperty] public string _retailSize ;
        [ObservableProperty] public string _packSize ;
        [ObservableProperty] public int _sellUnitsInPurchaseUnit ;
        [ObservableProperty] public decimal _price ;
        [ObservableProperty] public string _priceDisplay ;
        [ObservableProperty] public decimal _tax ;
        [ObservableProperty] public string _taxDisplay ;
        [ObservableProperty] public decimal _retailPrice ;
        [ObservableProperty] public string _retailPriceDisplay ;
        [ObservableProperty] public String _sizeUOM ;  // 12/14oz&#10;Unit: $2.29
        [ObservableProperty] public int _rowHeight ;
        [ObservableProperty] public String _uPC_1 ;
        [ObservableProperty] public String _uPC_2 ;
        [ObservableProperty] public String _uPC_3 ;
        [ObservableProperty] public String _uPC_4 ;
        [ObservableProperty] public String _status ;
        [ObservableProperty] public int _qtyOrder ;
        [ObservableProperty] public decimal _priceOrder ;
        [ObservableProperty] public decimal _extPriceOrder ;
        [ObservableProperty] public String _priceOrderDisplay ;
        [ObservableProperty] public Boolean _isCart ;
        [ObservableProperty] public Boolean _isCheckout ;
        [ObservableProperty] public Boolean _isLoggedIn ;
        [ObservableProperty] public int _categoryRank ;
        [ObservableProperty] public Boolean _isStepperVisible ;
        [ObservableProperty] public Boolean _isAddToOrderVisible ;
        [ObservableProperty] public int _qOH ;
        [ObservableProperty] public Boolean _isQOHVisible ;
        [ObservableProperty] public Boolean _isInStockVisible ;
        [ObservableProperty] public Boolean _isOutOfStockVisible ;
        [ObservableProperty] public Boolean _isStockRowVisible ;
        [ObservableProperty] public Boolean _isQOHRedVisible ;
        [ObservableProperty] public Boolean _isQOHBlackVisible ;
        [ObservableProperty] public Boolean _isOutOfStock ;
        [ObservableProperty] public string _newItem ;
        [ObservableProperty] public DateTime _dateAdded ;
        [ObservableProperty] public string _dateAddedDisplay ;
        [ObservableProperty] public DateTime _lastPurchDate ;
        [ObservableProperty] public string _lastPurchDateDisplay ;
        [ObservableProperty] public int _qtyLastOrder ;
        [ObservableProperty] public string _qtyLastOrderDisplay ;
        [ObservableProperty] public int _maxOrderQty ;
        [ObservableProperty] public Boolean _isMaxOrderQtyVisible ;
        [ObservableProperty] public string _maxOrderQtyDisplay ;
        [ObservableProperty] public string _keyword1 ;
        [ObservableProperty] public string _keyword2 ;
        [ObservableProperty] public string _keyword3 ;
        [ObservableProperty] public string _longDescription ;
        [ObservableProperty] public string _searchDescription ;
    }
}
