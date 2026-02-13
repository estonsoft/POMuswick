using banditoth.MAUI.DeviceId.Interfaces;
using SQLite;

namespace POMuswick
{
    public class Customer
    {
        [PrimaryKey]
        public int CustId { get; set; }
        public string UniqueId { get; set; }
        public bool IsCodeVerified { get; set; }
        public string CustNo { get; set; }
        public string CompanyName { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string EmailVerify { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string CityStateZip { get; set; }
        public int Warehouse { get; set; }
        public int Delivery { get; set; }
        public int Pickup { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal ARBalance { get; set; }
        public string Status { get; set; }
        public string LastPaymentDate { get; set; }
        public string LastOrderDate { get; set; }
        public string TermsDesc { get; set; }
        public string User { get; set; }
        public bool RememberMe { get; set; }
        public string TextMessageCode { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal ShippingFee { get; set; }

        public Customer()
        {
            CustId = -1;
            IsCodeVerified = false;
            UniqueId = App.app_uniqueId;
            CustNo = "0";
            CompanyName = "";
            Contact = "";
            Email = "";
            EmailVerify = "";
            Phone = "";
            CellPhone = "";
            Address1 = "";
            Address2 = "";
            City = "";
            State = "";
            Zip = "";
            CityStateZip = "";
            Status = "";
            User = "";
            RememberMe = false;
            MinOrderAmount = 0;
            ShippingFee = 0;
        }

        private string ReplaceSpecialChars(string sIn)
        {
            string s = sIn;

            s = s.Replace("|", "");
            s = s.Replace("~", "");
            s = s.Replace("#", "");
            s = s.Replace("'", "");
            //s = s.Replace("&", "amp;");
            //s = s.Replace(",", "");
            //s = s.Replace("\"", "");
            //s = s.Replace("\\", "");
            //s = s.Replace("/", "");
            //s = s.Replace("<", "");
            //s = s.Replace(">", "");

            return s;
        }
    }
}
