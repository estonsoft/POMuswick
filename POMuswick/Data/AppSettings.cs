namespace POMuswick.Models
{
    public class AppSettings
    {
        public bool IsLoggedIn { get; set; }
        public bool IsCredits { get; set; }
        public bool IsSalesUser { get; set; }
        public bool HoldForReview { get; set; }
        public bool ForceSubmit { get; set; }
        public string QOHDisplay { get; set; } = "X";
        public string UserName { get; set; } = "";
        public string LastUserName { get; set; } = "";
        public string CustomerNo { get; set; } = "";
        public string LastCategoryUpdate { get; set; } = "";
        public string LastItemUpdate { get; set; } = "";
        public bool BlockItemsNoQOH { get; set; }
        public string BaseUrl { get; set; } = "https://muswicksales.ddns.net";
        public bool IsOrderSubmiting { get; set; }

        public string scanBarcode { get; set; } = "";
        public bool IsDeliveryHighlighted { get; internal set; }

        public void Reset()
        {
            IsLoggedIn = false;
            IsCredits = false;
            IsSalesUser = false;
            UserName = "";
            HoldForReview = false;
            ForceSubmit = false;
            QOHDisplay = "X";
            IsOrderSubmiting = false;
            BlockItemsNoQOH = false;
        }
    }
}