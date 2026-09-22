using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Common;
using POMuswick.Services;

namespace POMuswick.ViewModels
{
    public partial class AccountViewModel : BaseViewModel
    {
        private readonly ICustomerService _customerService;
        [ObservableProperty]
        string _companyNameText;
        [ObservableProperty]
        string _addressText;
        [ObservableProperty]
        string _cityStateZipText;
        [ObservableProperty]
        string _phoneText;
        [ObservableProperty]
        string _emailText;
        [ObservableProperty]
        string _creditLimitText;

        [ObservableProperty]
        string _aRBalanceText;

        public AccountViewModel(IAppServices appServices) : base(appServices)
        {
            Title = PageTitles.MyAccount;
            _customerService = appServices._customerService;
        }

        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            await PopulateData();
        }

        private async Task PopulateData()
        {
            var customer = await _customerService.GetCustomerAsync();
            CompanyNameText = customer.CompanyName;
            AddressText = customer.Address1;
            if (customer.Address2 != "")
            {
                AddressText += "\n" + customer.Address2;
            }
            CityStateZipText = customer.CityStateZip;
            PhoneText = customer.Phone;
            EmailText = customer.Email;
            if (customer.CreditLimit == 0)
            {
                CreditLimitText = "N/A";
            }
            else
            {
                CreditLimitText = string.Format("{0:C}", customer.CreditLimit);
            }
            ARBalanceText = string.Format("{0:C}", customer.ARBalance);
        }
    }
}