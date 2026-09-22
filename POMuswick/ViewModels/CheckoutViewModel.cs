using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Common;
using POMuswick.Models;
using POMuswick.Repository;
using POMuswick.Services;

namespace POMuswick.ViewModels;

public partial class CheckoutViewModel : BaseViewModel, IQueryAttributable
{
    private readonly INavigationService _navigationService;
    private readonly IItemService _itemService;
    private readonly ISettingService _settingService;
    private readonly ICustomerService _customerService;
    private readonly ILocationService _locationService;
    private readonly ICartService _cartService;

    [ObservableProperty]
    int _iCartItems;
    [ObservableProperty]
    int _iCartPieces;
    [ObservableProperty]
    decimal dCartTotal;
    [ObservableProperty]
    string _sCartItems;
    [ObservableProperty]
    string _sCartPieces;
    [ObservableProperty]
    string _sCartTotal;
    [ObservableProperty]
    public bool _IsDeliveryHighlighted;
    [ObservableProperty]
    public bool _IsPickupHighlighted;
    [ObservableProperty]
    public bool _holdForReview;
    [ObservableProperty]
    public string _companyName;
    [ObservableProperty]
    public string _companyAddress;
    [ObservableProperty]
    public String _companyCityStateZip;
    [ObservableProperty]
    public string _locationName;
    [ObservableProperty]
    public string _locationAddress;
    [ObservableProperty]
    public string _locationCityStateZip;
    [ObservableProperty]
    List<Item> _itemList;
    [ObservableProperty]
    public bool _holdForCheckReviewIsVisible;
    [ObservableProperty]
    public bool _holdForLabelReviewIsVisible;
    private AppSettings appSettings;
    private Location location;
    public CheckoutViewModel(IAppServices appServices) : base(appServices)
    {
        Title = PageTitles.Checkout;
        _itemService = appServices._itemService;
        _navigationService = appServices._navigationService;
        _settingService = appServices._settingService;
        _customerService = appServices._customerService;
        _locationService = appServices._locationService;
        _cartService = appServices._cartService;
    }

    public async override Task OnAppearingAsync()
    {
        await base.OnAppearingAsync();
    }

    public async Task LoadCheckOutItem()
    {
        IsBusy = true;
        CatNSubCatParameter catNSubCatParameter = new();
        await SetUpPage();
        // if (App.g_ForceSubmit)
        // {
        //     HoldForReviewCheckbox.IsVisible = false;
        //     HoldForReviewLabel.IsVisible = false;
        // }
        await RefreshList();
        IsBusy = false;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("IsDeliveryHighlighted", out var searchValue))
        {
            IsDeliveryHighlighted = (bool)searchValue;
        }
        else
        {
            IsPickupHighlighted = true;
        }
        await LoadCheckOutItem();
    }

    private async Task SetUpPage()
    {
        appSettings = await _settingService.LoadSetting();
        var customer = await _customerService.GetCustomerAsync();
        HoldForReview = appSettings.HoldForReview;
        if (IsDeliveryHighlighted)
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

        location = await _locationService.GetLocationAsync(customer.Warehouse);
        LocationName = location.Name;
        LocationAddress = location.Address;
        LocationCityStateZip = location.CityStateZip;

    }

    public async Task RefreshList()
    {
        ICartItems = 0;
        ICartPieces = 0;
        DCartTotal = 0;
        ItemList = await _cartService.GetCheckoutItem();
        foreach (Item item in ItemList)
        {
            item.IsLoggedIn = appSettings.IsLoggedIn;
            item.IsStepperVisible = false;
            item.IsAddToOrderVisible = false;
            if (item.QtyOrder > 0)
            {
                item.PriceOrder = item.Price;
                item.PriceOrderDisplay = string.Format("{0:C}", item.PriceOrder);

                ICartItems += 1;
                DCartTotal += item.PriceOrder * item.QtyOrder;
                ICartPieces += item.QtyOrder;
            }
        }

        SCartItems = ICartItems.ToString();
        SCartPieces = ICartPieces.ToString();
        SCartTotal = DCartTotal.ToString("0:C2");
    }

    [RelayCommand]
    async Task PlaceOrderAsync()
    {
        await _navigationService.GoToAsync(AppRoutes.SubmitOrder, new ShellNavigationQueryParameters
                            {
                                {"IsDeliveryHighlighted",IsDeliveryHighlighted},
                                {"HoldReview",HoldForReview}
                            });
    }
}
