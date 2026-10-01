using Realms;


namespace POMuswick
{
    public class SalesCustomer : Realms.RealmObject
    {
        [PrimaryKey]
        public string CustNo { get; set; }
        public string CompanyName { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public decimal ARBalance { get; set; }
        public string ARBalanceDisplay { get; set; }
        public decimal CreditLimit { get; set; }
        public string CreditLimitDisplay { get; set; }
        public string CityStateZip { get; set; }
        public int Warehouse { get; set; }
        public int Delivery { get; set; }
        public string TermsCode { get; set; }
        public string TermsDesc { get; set; }
        public string AmountDue { get; set; }
        public string LastPaymentDate { get; set; }
        public string LastOrderDate { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal ShippingFee { get; set; }

        public SalesCustomer CopyDetached()
        {
            return new SalesCustomer
            {
                CustNo = CustNo,
                CompanyName = CompanyName,
                Contact = Contact,
                Email = Email,
                Phone = Phone,
                Address1 = Address1,
                Address2 = Address2,
                City = City,
                State = State,
                Zip = Zip,
                ARBalance = ARBalance,
                ARBalanceDisplay = ARBalanceDisplay,
                CreditLimit = CreditLimit,
                CreditLimitDisplay = CreditLimitDisplay,
                CityStateZip = CityStateZip,
                Warehouse = Warehouse,
                Delivery = Delivery,
                TermsCode = TermsCode,
                TermsDesc = TermsDesc,
                AmountDue = AmountDue,
                LastPaymentDate = LastPaymentDate,
                LastOrderDate = LastOrderDate,
                MinOrderAmount = MinOrderAmount,
                ShippingFee = ShippingFee
            };
        }
    }
}
