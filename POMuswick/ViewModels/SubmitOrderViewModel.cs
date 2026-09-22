using System.Security.Cryptography;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.Internals;
using POMuswick.Common;
using POMuswick.Models;
using POMuswick.Services;

namespace POMuswick.ViewModels
{
    public class SubmitOrderViewModel : BaseViewModel,IQueryAttributable
    {
        private readonly IDialogService _dialogService;
        private readonly ICartService _cartService;
        private readonly INavigationService _navigationService;
        private readonly ISettingService _settingService;
        private readonly ISubmitOrderService _submitOrderService;

        public SubmitOrderViewModel(IAppServices appServices) : base(appServices)
        {
            Title = PageTitles.SubmitOrder;
            _dialogService = appServices._dialogService;
            _cartService = appServices._cartService;
            _settingService = appServices._settingService;
            _navigationService = appServices._navigationService;
            _submitOrderService = appServices._submitOrderService;
        }

        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            bool IsDeliveryHighlighted = false;
            bool HoldReview = false;
            if (query.TryGetValue("IsDeliveryHighlighted", out var searchValue))
            {
                IsDeliveryHighlighted = (bool)searchValue;
            }
            if (query.TryGetValue("HoldReview", out var hold))
            {
                HoldReview = (bool)hold;
            }
            await LoadData(IsDeliveryHighlighted,HoldReview);
        }
        private async Task LoadData(bool isDeliveryHighlighted,bool hold)
        {
            List<Item> lstCartItems = await _cartService.GetCartItems();
            String sOrderInfo = "";

            foreach (Item item in lstCartItems)
            {
                sOrderInfo += item.ItemNo.ToString() + "|";
                sOrderInfo += item.QtyOrder.ToString() + "|";
                sOrderInfo += "0" + "~";
            }

            String sDeliveryPickup;
            if (isDeliveryHighlighted)
            {
                sDeliveryPickup = "D";
            }
            else
            {
                sDeliveryPickup = "P";
            }

            AppSettings appSettings = await _settingService.LoadSetting();
            SubmitOrderRequest submitOrderRequest = new()
            {
                CustomerNo = appSettings.CustomerNo,
                OrderInfo = sOrderInfo,
                DeliveryPickup = sDeliveryPickup,
                User = appSettings.UserName,
                HoldForReview = hold,
                PurchaseOrder = string.Empty,
                PaymentMethod = string.Empty,
                CreditCardInfo = string.Empty,
                Notes = string.Empty,
                OrderType = "0"
            };
            var result = await _submitOrderService.SubmitOrder2Async(submitOrderRequest);
            if (result.validateResponse.IsValid)
            {
                await _navigationService.GoToRootAsync(AppRoutes.Home);
            }
            else
            {
                var response = await _dialogService.ConfirmAsync("Error", result.validateResponse.Message, "Yes", "No");
                await _navigationService.GoToRootAsync(AppRoutes.Home);
            }
        }
    }
}