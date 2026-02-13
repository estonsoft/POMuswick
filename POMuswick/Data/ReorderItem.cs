using SQLite;

namespace POMuswick
{
    public class ReorderItem
    {
        [PrimaryKey]
        public int ItemNo { get; set; }
        public string ItemNoDisplay { get; set; }
        public DateTime LastPurchDate { get; set; }
        public string LastPurchDateDisplay { get; set; }
        public int QtyLastOrder { get; set; }
        public string QtyOrderDisplay { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PriceDisplay { get; set; }
        public string UOM { get; set; }
        public string Size { get; set; }
        public string Form { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryDesc { get; set; }
        public string SubcategoryCode { get; set; }
        public string SubcategoryDesc { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string SellUnitsInPurch { get; set; }
        public string SizeDisplay { get; set; }
        public string UPC { get; set; }
        public string ItemNoDisplayUPC { get; set; }
        public string ImageURL { get; set; }
        public string ImageBase64 { get; set; }
        public bool IsLoggedIn { get; set; }
        public int RowHeight { get; set; }
        public Boolean IsStepperVisible { get; set; }
        public Boolean IsAddToOrderVisible { get; set; }
        public int QtyOrder { get; set; }
        public string SizeUOM { get; set; }
        public string UnitPriceDisplay { get; set; }
        public string UnitPriceStandardSell { get; set; }
        public string Status { get; set; }
        public int QOH { get; set; }
        public Boolean IsQOHVisible { get; set; }
        public Boolean IsInStockVisible { get; set; }
        public Boolean IsOutOfStockVisible { get; set; }
        public Boolean IsStockRowVisible { get; set; }
        public Boolean IsQOHRedVisible { get; set; }
        public Boolean IsQOHBlackVisible { get; set; }
        public int MaxOrderQty { get; set; }
        public Boolean IsMaxOrderQtyVisible { get; set; }
        public string MaxOrderQtyDisplay { get; set; }
    }
}
