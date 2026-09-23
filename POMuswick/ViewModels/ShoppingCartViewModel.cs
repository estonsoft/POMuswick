using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Common;
using POMuswick.Services;
using POMuswick.UIModels;
namespace POMuswick.ViewModels
{
    public partial class ShoppingCartViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IItemService _itemService;
        private readonly INavigationService _navigationService;
        private readonly ICustomerService _customerService;
        private readonly ICartService _cartService;
        private readonly ISettingService _settingService;
        private readonly ISubmitOrderService _submitOrderService;

        [ObservableProperty]
        List<UIItems> _cartItemList;
        [ObservableProperty]
        int iCartItems = 0;
        [ObservableProperty]
        int iCartPieces = 0;
        [ObservableProperty]
        decimal dCartTotal = 0;
        [ObservableProperty]
        public bool _isSignInVisible;
        [ObservableProperty]
        public bool _IsDeliveryHighlighted;
        [ObservableProperty]
        public bool _IsPickupHighlighted;
        [ObservableProperty]
        public Location _Location = new Location();

        [ObservableProperty]
        string _cartItems;
        [ObservableProperty]
        string _cartPieces;
        [ObservableProperty]
        string _cartTotal;
        [ObservableProperty]
        public String _companyName;

        [ObservableProperty]
        public String _companyAddress;

        [ObservableProperty]
        public String _companyCityStateZip;
        [ObservableProperty]
        public String _locationName;
        [ObservableProperty]
        public String _locationAddress;
        [ObservableProperty]
        public String _locationCityStateZip;
        public ShoppingCartViewModel(IAppServices appServices) : base(appServices)
        {
            Title = PageTitles.ShoppingCart;
            _dialogService = appServices._dialogService;
            _itemService = appServices._itemService;
            _customerService = appServices._customerService;
            _cartService = appServices._cartService;
            _navigationService = appServices._navigationService;
            _settingService = appServices._settingService;
            _submitOrderService = appServices._submitOrderService;
        }

        public async override Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            await DPStatus();
            await GetCartStatus();
        }

        private async Task DPStatus()
        {
            var customer = await _customerService.GetCustomerAsync();
            var appSetting = await _settingService.LoadSetting();
            IsSignInVisible = !appSetting.IsLoggedIn;
            if (customer.Delivery == 1)
            {
                IsDeliveryHighlighted = true;
                IsPickupHighlighted = false;

                CompanyName = customer.CompanyName;
                CompanyAddress = customer.Address1;
                CompanyCityStateZip = customer.CityStateZip;
            }
            else
            {
                IsDeliveryHighlighted = false;
                IsPickupHighlighted = true;

                CompanyName = customer.CompanyName;
                CompanyAddress = "Delivery Not Available";
                CompanyCityStateZip = "";
            }
        }

        private async Task GetCartStatus()
        {
            var cartPieces = await _cartService.GetCartPieces();
            if (cartPieces > 0)
            {
                await RefreshListAsync();
            }
            else
            {
                await _navigationService.GoToRootAsync(AppRoutes.Home);
                await _dialogService.AlertAsync("Your shopping cart is empty", "Muswick Wholesale Grocers", "Ok");
            }
        }

        public async Task RefreshListAsync()
        {
            var cartItems = await _cartService.GetCartItems();
            List<UIItems> uiList = cartItems
                .Select(x => x.ToUI())
                .ToList();
            CartItemList = uiList;

            var appSetting = await _settingService.LoadSetting();
            foreach (UIItems i in CartItemList)
            {
                i.IsLoggedIn = appSetting.IsLoggedIn;

                if (i.QtyOrder == 0)
                {
                    i.IsStepperVisible = false;
                    i.IsAddToOrderVisible = true;
                }
                else if (i.QtyOrder < 0)
                {
                    i.IsStepperVisible = false;
                    i.IsAddToOrderVisible = false;
                }
                else
                {
                    i.IsStepperVisible = true;
                    i.IsAddToOrderVisible = false;
                }
                i.IsQOHBlackVisible = false;
                i.IsQOHRedVisible = false;
                if (appSetting.QOHDisplay == "Q")
                {
                    i.IsQOHVisible = true;
                    i.IsInStockVisible = false;
                    i.IsOutOfStockVisible = false;
                    if (i.QOH > 0)
                    {
                        i.IsQOHBlackVisible = true;
                    }
                    else
                    {
                        i.IsQOHRedVisible = true;
                    }
                }
                else if (appSetting.QOHDisplay == "I")
                {
                    i.IsQOHVisible = false;
                    if (i.QOH > 0)
                    {
                        i.IsInStockVisible = true;
                        i.IsOutOfStockVisible = false;
                    }
                    else
                    {
                        i.IsInStockVisible = false;
                        i.IsOutOfStockVisible = true;
                    }
                }
                else
                {
                    i.IsQOHVisible = false;
                    i.IsInStockVisible = false;
                    i.IsOutOfStockVisible = false;
                }
                if (i.IsQOHVisible || i.IsInStockVisible || i.IsOutOfStockVisible)
                {
                    i.IsStockRowVisible = true;
                }
                else
                {
                    i.IsStockRowVisible = false;
                }
            }
            UpdateTotals();
        }

        public async void RefreshList()
        {
            try
            {
                await RefreshListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cart refresh failed: {ex}");
            }
        }
        public void UpdateTotals()
        {
            ICartItems = 0;
            ICartPieces = 0;
            DCartTotal = 0m;

            foreach (UIItems i in CartItemList)
            {
                if (i.QtyOrder > 0)
                {
                    i.PriceOrder = i.Price;
                    ICartItems += 1;
                    DCartTotal += i.PriceOrder * i.QtyOrder;
                    ICartPieces += i.QtyOrder;
                }
            }
            CartItems = ICartItems.ToString();
            CartPieces = ICartPieces.ToString();
            CartTotal = DCartTotal.ToString("0.00");

            if (Shell.Current is AppShell shell)
                shell.SetCartTabCount(ICartItems);
        }

        [RelayCommand]
        public async Task CheckoutAsync()
        {
            await Validate();
        }

        private async Task Validate()
        {
            IsBusy = true;
            string sOrderInfo = "";
            foreach (UIItems item in CartItemList)
            {
                sOrderInfo += item.ItemNo.ToString() + "|";
                sOrderInfo += item.QtyOrder.ToString() + "|";
                sOrderInfo += "0" + "~";
            }
            SubmitOrderResult result = await _submitOrderService.ValidateOrderAsync(sOrderInfo);
            if (result.validateResponse.IsValid)
            {
                var customer = await _customerService.GetCustomerAsync();
                IsBusy = false;
                if ((DCartTotal < customer.MinOrderAmount) && IsDeliveryHighlighted)
                {
                    bool bContinue = await _dialogService.ConfirmAsync("Muswick Wholesale Grocers", $"Your order total must be at least ${customer.MinOrderAmount:0.00} to avoid a ${customer.ShippingFee:0.00} shipping fee.  Do you wish to continue?  Yes to continue and place order.  No to go back and add more items to your order.", "YES", "NO");

                    if (bContinue)
                    {
                        await _navigationService.GoToAsync(AppRoutes.Checkout, new ShellNavigationQueryParameters
                            {
                                {"IsDeliveryHighlighted",IsDeliveryHighlighted}
                            });
                        return;
                    }
                }
                else
                {
                    await _navigationService.GoToAsync(AppRoutes.Checkout, new ShellNavigationQueryParameters
                            {
                                {"IsDeliveryHighlighted",IsDeliveryHighlighted}
                            });
                }
            }
            else
            {
                IsBusy = false;
                await _dialogService.AlertAsync("Muswick Wholesale Grocers", result.validateResponse.Message, "Ok");
            }
        }
        [RelayCommand]
        public async Task SignInAsync()
        {
            await _navigationService.GoToAsync(AppRoutes.Login);
        }

        [RelayCommand]
        public async Task ClearCartAsync()
        {
            bool bClear = await _dialogService.ConfirmAsync("Profit Order", "Are you sure you wish to remove all the items from your shopping cart?", "Yes", "No");

            if (bClear)
            {
                await _cartService.ClearCartItems();
                await _navigationService.GoToRootAsync(AppRoutes.Home);
            }
        }

        [RelayCommand]
        public async Task DeliveryAsync()
        {
            var customer = await _customerService.GetCustomerAsync();
            if (customer.Delivery == 1)
            {
                IsDeliveryHighlighted = true;
                IsPickupHighlighted = false;
            }
        }
        [RelayCommand]
        public async Task PickupAsync()
        {
            IsDeliveryHighlighted = false;
            IsPickupHighlighted = true;
        }

        [RelayCommand]
        private async Task IncreaseQtyAsync(UIItems item)
        {
            if (item == null)
                return;

            if (item.QtyOrder >= 999)
                return;

            if (item.MaxOrderQty > 0 &&
                item.QtyOrder >= item.MaxOrderQty)
                return;
            item.QtyOrder++;
            await _itemService.UpdateItemQtySet(item.ItemNo, item.QtyOrder);



            item.IsStepperVisible = true;
            item.IsAddToOrderVisible = false;

            UpdateTotals();
            await Task.CompletedTask;
        }

        [RelayCommand]
        private async Task DecreaseQtyAsync(UIItems item)
        {
            if (item == null)
                return;

            if (item.QtyOrder <= 0)
                return;

            item.QtyOrder--;
            await _itemService.UpdateItemQtySet(item.ItemNo, item.QtyOrder);

            if (item.QtyOrder == 0)
            {
                item.IsStepperVisible = false;
                item.IsAddToOrderVisible = true;
            }
            UpdateTotals();
            await Task.CompletedTask;
        }
    }

    public static class ItemMapper
    {
        public static UIItems ToUI(this Item item)
        {
            if (item == null)
                return null!;

            return new UIItems
            {
                ItemNo = item.ItemNo,
                ItemNoDisplay = item.ItemNoDisplay,
                ItemNoDisplayUPC = item.ItemNoDisplayUPC,
                Qty = item.Qty,
                QtyDisplay = item.QtyDisplay,
                Description = item.Description,
                ImageURL = item.ImageURL,
                ImageBase64 = item.ImageBase64,
                CategoryCode = item.CategoryCode,
                CategoryDesc = item.CategoryDesc,
                SubcategoryCode = item.SubcategoryCode,
                SubcategoryDesc = item.SubcategoryDesc,
                VendorCode = item.VendorCode,
                VendorName = item.VendorName,
                UOM = item.UOM,
                Size = item.Size,
                SizeDisplay = item.SizeDisplay,
                Form = item.Form,
                RetailUOM = item.RetailUOM,
                RetailSize = item.RetailSize,
                PackSize = item.PackSize,
                SellUnitsInPurchaseUnit = item.SellUnitsInPurchaseUnit,
                Price = item.Price,
                PriceDisplay = item.PriceDisplay,
                Tax = item.Tax,
                TaxDisplay = item.TaxDisplay,
                RetailPrice = item.RetailPrice,
                RetailPriceDisplay = item.RetailPriceDisplay,
                SizeUOM = item.SizeUOM,
                RowHeight = item.RowHeight,
                UPC_1 = item.UPC_1,
                UPC_2 = item.UPC_2,
                UPC_3 = item.UPC_3,
                UPC_4 = item.UPC_4,
                Status = item.Status,
                QtyOrder = item.QtyOrder,
                PriceOrder = item.PriceOrder,
                ExtPriceOrder = item.ExtPriceOrder,
                PriceOrderDisplay = item.PriceOrderDisplay,
                IsCart = item.IsCart,
                IsCheckout = item.IsCheckout,
                IsLoggedIn = item.IsLoggedIn,
                CategoryRank = item.CategoryRank,
                IsStepperVisible = item.IsStepperVisible,
                IsAddToOrderVisible = item.IsAddToOrderVisible,
                QOH = item.QOH,
                IsQOHVisible = item.IsQOHVisible,
                IsInStockVisible = item.IsInStockVisible,
                IsOutOfStockVisible = item.IsOutOfStockVisible,
                IsStockRowVisible = item.IsStockRowVisible,
                IsQOHRedVisible = item.IsQOHRedVisible,
                IsQOHBlackVisible = item.IsQOHBlackVisible,
                IsOutOfStock = item.IsOutOfStock,
                NewItem = item.NewItem,
                DateAdded = item.DateAdded,
                DateAddedDisplay = item.DateAddedDisplay,
                LastPurchDate = item.LastPurchDate,
                LastPurchDateDisplay = item.LastPurchDateDisplay,
                QtyLastOrder = item.QtyLastOrder,
                QtyLastOrderDisplay = item.QtyLastOrderDisplay,
                MaxOrderQty = item.MaxOrderQty,
                IsMaxOrderQtyVisible = item.IsMaxOrderQtyVisible,
                MaxOrderQtyDisplay = item.MaxOrderQtyDisplay,
                Keyword1 = item.Keyword1,
                Keyword2 = item.Keyword2,
                Keyword3 = item.Keyword3,
                LongDescription = item.LongDescription,
                SearchDescription = item.SearchDescription
            };
        }
    }
}