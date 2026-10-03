using BarcodeScanning;
using POMuswick.ViewModels;
using System.Globalization;


namespace POMuswick.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuickEntryPage : ContentPage
    {
        private readonly QuickEntryViewModel _viewModel;
        public QuickEntryPage(QuickEntryViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            // Load your data or items here
            await _viewModel.OnAppearingAsync();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await _viewModel.OnDisappearingAsync();
        }

        private void OnScanItemTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not Entry entry || string.IsNullOrEmpty(e.NewTextValue))
                return;

            string normalizedText = new(e.NewTextValue.Select(character =>
            {
                int digit = CharUnicodeInfo.GetDecimalDigitValue(character);
                return digit is >= 0 and <= 9 ? (char)('0' + digit) : character;
            }).ToArray());

            if (!string.Equals(normalizedText, e.NewTextValue, StringComparison.Ordinal))
                entry.Text = normalizedText;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
