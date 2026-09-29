namespace POMuswick.UIModels
{
    // Shared shape for models rendered by CustomListItem so its bindings can be compiled (x:DataType).
    public interface IListItemDisplay
    {
        int ItemNo { get; set; }
        string ItemNoDisplay { get; set; }
        string ItemNoDisplayUPC { get; set; }
        string Description { get; set; }
        string ImageURL { get; set; }
        string CategoryDesc { get; set; }
        string PriceDisplay { get; set; }
        string SizeUOM { get; set; }
        string UOM { get; set; }
        int QOH { get; set; }
        int QtyOrder { get; set; }
        int MaxOrderQty { get; set; }
        string MaxOrderQtyDisplay { get; set; }
        bool IsStockRowVisible { get; set; }
        bool IsQOHVisible { get; set; }
        bool IsQOHRedVisible { get; set; }
        bool IsQOHBlackVisible { get; set; }
        bool IsInStockVisible { get; set; }
        bool IsOutOfStockVisible { get; set; }
        bool IsMaxOrderQtyVisible { get; set; }
        bool IsPriceVisible { get; set; }
        bool IsStepperVisible { get; set; }
        bool IsAddToOrderVisible { get; set; }
    }
}
