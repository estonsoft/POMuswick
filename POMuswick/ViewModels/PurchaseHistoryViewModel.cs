using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Common;
using POMuswick.Controls;
using POMuswick.Services;

namespace POMuswick.ViewModels
{
    public partial class PurchaseHistoryViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IOrderHistoryService _orderHistoryService;
        private readonly ICustomerService _customerService;

        [ObservableProperty]
        public List<OrderHeader> _orderHistoryList;
        [ObservableProperty]
        public string _order;

        public PurchaseHistoryViewModel(IAppServices appServices) : base(appServices)
        {
            Title = PageTitles.OrderHistory;
            _dialogService = appServices._dialogService;
            _navigationService = appServices._navigationService;
            _orderHistoryService = appServices._orderHistoryService;
            _customerService =appServices._customerService;
        }

        public async override Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            RefreshList();
        }

        public async void RefreshList()
        {
            var customer = await _customerService.GetCustomerAsync();
            OrderHistoryList = await _orderHistoryService.FetchOrderHeadersAsync(customer.CustNo);
        }

        [RelayCommand]
        public async Task DetailsAsync(OrderHeader header)
        {
            await _navigationService.GoToAsync(AppRoutes.PurchaseHistoryDetail,
                new ShellNavigationQueryParameters
                {
                    { "OrderNo", header.OrderNo }
                });
        }
    }
}

