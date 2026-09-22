using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Common;
using POMuswick.Services;

namespace POMuswick.ViewModels
{
    public partial class CustomerListViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly ISalesPersonCustomersService _salesPersonCustomersService;
        private readonly ICustomerService _customerService;
        private readonly ICatNSubCatService _catNSubCatService;
        private readonly IOrderHistoryService _orderHistoryService;
        private readonly ICartService _cartService;
        private readonly IAppSyncService _appSyncService;

        [ObservableProperty]
        public List<SalesCustomer> _customers;

        [ObservableProperty]
        public string _customerSearchText;


        public CustomerListViewModel(IAppServices appServices) : base(appServices)
        {
            Title = PageTitles.Customers;
            _dialogService = appServices._dialogService;
            _navigationService = appServices._navigationService;
            _customerService = appServices._customerService;
            _catNSubCatService = appServices._catNSubCatService;
            _orderHistoryService = appServices._orderHistoryService;
            _cartService = appServices._cartService;
            _salesPersonCustomersService = appServices._salesPersonCustomersService;
            _appSyncService = appServices._appSyncService;
        }

        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            await RefreshListAsync();
        }

        [RelayCommand]
        public async Task RefreshListAsync()
        {
            IsBusy = true;
            Customers = await _salesPersonCustomersService.GetSalesCustomer("");
            if(Customers == null || Customers.Count == 0)
            {
                var customersResult = await _salesPersonCustomersService.FetchSalesPersonCustomersAsync("");
                Customers = customersResult.salesCustomers;
            }
            IsBusy = false;
        }

        [RelayCommand]
        public async Task SearchCustomerAsync()
        {
            IsBusy = true;
            Customers = await _salesPersonCustomersService.GetSalesCustomer(CustomerSearchText);
            IsBusy = false;
        }

        [RelayCommand]
        private async Task SelectedCustomerAsync(SalesCustomer customer)
        {
            IsBusy = true;
            var currentCustomer = await _customerService.GetCustomerAsync();
            string OldCustNo = currentCustomer.CustNo;

            SalesCustomer salesCustomer = await _salesPersonCustomersService.FindSalesCustomer(customer.CustNo);


            if ((salesCustomer.CustNo != null) && (salesCustomer.CustNo != "") && (salesCustomer.CustNo != "0"))
            {
                await _appSyncService.SyncApp(salesCustomer.CustNo);
            }
            Customer newCustomer = BuildCustomer(salesCustomer);
            await _customerService.SaveCustomerAsync(newCustomer);
            await _cartService.SuspendCartItems(OldCustNo);
            await _cartService.ClearCartItems();
            await _orderHistoryService.ClearOrderHistory();
            await _cartService.RestoreCart();
            await _navigationService.GoBackAsync();
            IsBusy = false;
        }

        private Customer BuildCustomer(SalesCustomer cust)
        {
            return new Customer()
            {
                CustId = -1,
                CustNo = cust.CustNo,
                CompanyName = cust.CompanyName,
                Address1 = cust.Address1,
                Address2 = cust.Address2,
                City = cust.City,
                State = cust.State,
                Zip = cust.Zip,
                CityStateZip = cust.CityStateZip,
                Phone = cust.Phone,
                Contact = cust.Contact,
                Email = cust.Email,
                TermsDesc = cust.TermsDesc,
                ARBalance = cust.ARBalance,
                CreditLimit = cust.CreditLimit,
                LastPaymentDate = cust.LastPaymentDate,
                LastOrderDate = cust.LastOrderDate,
                Delivery = 1,
                Warehouse = 1,
                MinOrderAmount = cust.MinOrderAmount,
                ShippingFee = cust.ShippingFee
            };
        }
    }
}