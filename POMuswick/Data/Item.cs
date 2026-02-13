using SQLite;

namespace POMuswick
{
    public class Item
    {
        [PrimaryKey]
        public int ItemNo { get; set; }
        public string ItemNoDisplay { get; set; }
        public string ItemNoDisplayUPC { get; set; }
        public int Qty { get; set; }
        public string QtyDisplay { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public string ImageBase64 { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryDesc { get; set; }
        public string SubcategoryCode { get; set; }
        public string SubcategoryDesc { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string UOM { get; set; }
        public int Size { get; set; }
        public string SizeDisplay { get; set; }
        public string Form { get; set; }
        public string RetailUOM { get; set; }
        public string RetailSize { get; set; }
        public string PackSize { get; set; }
        public int SellUnitsInPurchaseUnit { get; set; }
        public decimal Price { get; set; }
        public string PriceDisplay { get; set; }
        public decimal Tax { get; set; }
        public string TaxDisplay { get; set; }
        public decimal RetailPrice { get; set; }
        public string RetailPriceDisplay { get; set; }
        public String SizeUOM { get; set; }  // 12/14oz&#10;Unit: $2.29
        public int RowHeight { get; set; }
        public String UPC_1 { get; set; }
        public String UPC_2 { get; set; }
        public String UPC_3 { get; set; }
        public String UPC_4 { get; set; }
        public String Status { get; set; }
        public int QtyOrder { get; set; }
        public decimal PriceOrder { get; set; }
        public decimal ExtPriceOrder { get; set; }
        public String PriceOrderDisplay { get; set; }
        public Boolean IsCart { get; set; }
        public Boolean IsCheckout { get; set; }
        public Boolean IsLoggedIn { get; set; }
        public int CategoryRank { get; set; }
        public Boolean IsStepperVisible { get; set; }
        public Boolean IsAddToOrderVisible { get; set; }
        public int QOH { get; set; }
        public Boolean IsQOHVisible { get; set; }
        public Boolean IsInStockVisible { get; set; }
        public Boolean IsOutOfStockVisible { get; set; }
        public Boolean IsStockRowVisible { get; set; }
        public Boolean IsQOHRedVisible { get; set; }
        public Boolean IsQOHBlackVisible { get; set; }
        public Boolean IsOutOfStock { get; set; }
        public string NewItem { get; set; }
        public DateTime DateAdded { get; set; }
        public string DateAddedDisplay { get; set; }
        public DateTime LastPurchDate { get; set; }
        public string LastPurchDateDisplay { get; set; }
        public int QtyLastOrder { get; set; }
        public string QtyLastOrderDisplay { get; set; }
        public int MaxOrderQty { get; set; }
        public Boolean IsMaxOrderQtyVisible { get; set; }
        public string MaxOrderQtyDisplay { get; set; }
        public string Keyword1 { get; set; }
        public string Keyword2 { get; set; }
        public string Keyword3 { get; set; }
        public string LongDescription { get; set; }
        public string SearchDescription { get; set; }
    }
}