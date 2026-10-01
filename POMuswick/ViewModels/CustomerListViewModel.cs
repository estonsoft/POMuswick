using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Common;
using POMuswick.Models;
using POMuswick.Services;

namespace POMuswick.ViewModels
{
    public partial class CustomerListViewModel : BaseViewModel
    {
        private const int PageSize = 50;

        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly ISalesPersonCustomersService _salesPersonCustomersService;
        private readonly ICustomerService _customerService;
        private readonly ICatNSubCatService _catNSubCatService;
        private readonly IOrderHistoryService _orderHistoryService;
        private readonly ICartService _cartService;
        private readonly ISettingService _settingService;

        private int _loadedCount;
        private bool _hasMoreCustomers = true;
        private bool _isLoadingMore;

        [ObservableProperty]
        public ObservableCollection<SalesCustomer> _customers = new();

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
            _settingService = appServices._settingService;
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
            var customersResult = await _salesPersonCustomersService.GetSalesCustomer("");

            if (customersResult != null)
            {
                int totalAvailable = customersResult.Count;

                // Track your current page index pointer. 
                // Example: if page 1 loaded 20 items, your next 'currentOffset' is 20.
                int currentOffset = _customers.Count;

                // Safely calculate how many items are left to pull for the next segment chunk
                int itemsToFetch = Math.Min(PageSize, totalAvailable - currentOffset);

                if (itemsToFetch > 0)
                {
                    var nextChunk = new List<SalesCustomer>();

                    for (int i = 0; i < itemsToFetch; i++)
                    {
                        // Realm instantly resolves individual items by index with zero memory overhead
                        nextChunk.Add(customersResult[currentOffset + i]);
                    }

                    // Append the new page directly into your UI collection
                    foreach (var customer in nextChunk)
                    {
                        _customers.Add(customer);
                    }
                }
            }
            IsBusy = false;
        }

        [RelayCommand]
        public async Task SearchCustomerAsync()
        {
            IsBusy = true;
            var firstPage = await _salesPersonCustomersService.GetSalesCustomer(CustomerSearchText, 0, PageSize);
            ResetPaging(firstPage);
            IsBusy = false;
        }

        [RelayCommand]
        public async Task LoadMoreCustomersAsync()
        {
            if (_isLoadingMore || !_hasMoreCustomers)
                return;

            _isLoadingMore = true;
            try
            {
                var nextPage = await _salesPersonCustomersService.GetSalesCustomer(CustomerSearchText ?? "", _loadedCount, PageSize);
                foreach (var customer in nextPage)
                {
                    Customers.Add(customer);
                }
                _loadedCount += nextPage.Count;
                _hasMoreCustomers = nextPage.Count == PageSize;
            }
            finally
            {
                _isLoadingMore = false;
            }
        }

        private void ResetPaging(List<SalesCustomer> firstPage)
        {
            Customers = new ObservableCollection<SalesCustomer>(firstPage);
            _loadedCount = firstPage.Count;
            _hasMoreCustomers = firstPage.Count == PageSize;
        }

        [RelayCommand]
        private async Task SelectedCustomerAsync(SalesCustomer customer)
        {
            IsBusy = true;
            try
            {
                var currentCustomer = await _customerService.GetCustomerAsync();
                string OldCustNo = currentCustomer.CustNo;

                await _settingService.SaveChanges(new Dictionary<string, string>
                {
                    [nameof(AppSettings.CustomerNo)] = currentCustomer.CustNo
                });

                SalesCustomer salesCustomer = await _salesPersonCustomersService.FindSalesCustomer(customer.CustNo);

                if (!string.IsNullOrEmpty(salesCustomer.CustNo) && salesCustomer.CustNo != "0")
                {
                    await SyncAppAsync(salesCustomer.CustNo);
                }
                Customer newCustomer = BuildCustomer(salesCustomer);
                await _customerService.SaveCustomerAsync(newCustomer);
                await _cartService.SuspendCartItems(OldCustNo);
                await _cartService.ClearCartItems();
                await _orderHistoryService.ClearOrderHistory();
                await _cartService.RestoreCart();
                await _navigationService.GoBackAsync();
            }
            finally
            {
                IsBusy = false;
            }
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