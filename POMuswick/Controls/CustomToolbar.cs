using System.Security.AccessControl;
using System.Windows.Input;

namespace POMuswick.Controls;

public class CustomToolbar : StackLayout
{
    public static readonly BindableProperty HomeCommandProperty =
    BindableProperty.Create(
        nameof(HomeCommand),
        typeof(ICommand),
        typeof(CustomToolbar));

    public ICommand HomeCommand
    {
        get => (ICommand)GetValue(HomeCommandProperty);
        set => SetValue(HomeCommandProperty, value);
    }

    public static readonly BindableProperty CartCommandProperty =
    BindableProperty.Create(
        nameof(CartCommand),
        typeof(ICommand),
        typeof(CustomToolbar));

    public ICommand CartCommand
    {
        get => (ICommand)GetValue(CartCommandProperty);
        set => SetValue(CartCommandProperty, value);
    }

    public static readonly BindableProperty HistoryCommandProperty =
    BindableProperty.Create(
        nameof(HistoryCommand),
        typeof(ICommand),
        typeof(CustomToolbar));

    public ICommand HistoryCommand
    {
        get => (ICommand)GetValue(HistoryCommandProperty);
        set => SetValue(HistoryCommandProperty, value);
    }

    public static readonly BindableProperty ShopCommandProperty =
    BindableProperty.Create(
        nameof(ShopCommand),
        typeof(ICommand),
        typeof(CustomToolbar));

    public ICommand ShopCommand
    {
        get => (ICommand)GetValue(ShopCommandProperty);
        set => SetValue(ShopCommandProperty, value);
    }

    public static readonly BindableProperty ScanCommandProperty =
    BindableProperty.Create(
        nameof(ScanCommand),
        typeof(ICommand),
        typeof(CustomToolbar));

    public ICommand ScanCommand
    {
        get => (ICommand)GetValue(ScanCommandProperty);
        set => SetValue(ScanCommandProperty, value);
    }
    Grid gridContainer;

    VerticalStackLayout StackHome;
    VerticalStackLayout StackShoppingCart;
    VerticalStackLayout StackPurchaseHistory;
    VerticalStackLayout StackShopNow;
    VerticalStackLayout StackScanBarcode;

    Image LabelHomeIcon;
    Label LabelHomeText;
    Image LabelShoppingCartIcon;
    Label LabelShoppingCartText;
    Label LabelShoppingCartItems;
    Image LabelPurchaseHistoryIcon;
    Label LabelPurchaseHistoryText;
    Image LabelShopNowIcon;
    Label LabelShopNowText;
    Image LabelScanBarcodeIcon;
    Label LabelScanBarcodeText;

    TapGestureRecognizer TapHome;
    TapGestureRecognizer TapShoppingCart;
    TapGestureRecognizer TapPurchaseHistory;
    TapGestureRecognizer TapShopNow;
    TapGestureRecognizer TapScanBarcode;

    public CustomToolbar()
    {
        HeightRequest = 70;
        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            HeightRequest = 80;
        }
        BackgroundColor = Colors.Blue;

        gridContainer = new Grid
        {
            ColumnSpacing = 5,
            Padding = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            BackgroundColor = Colors.Blue,
            HeightRequest = HeightRequest,
        };
        gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

        Children.Add(gridContainer);

        StackHome = CreateStack(out LabelHomeIcon, out LabelHomeText, "\uF015", "Home");
        TapHome = new TapGestureRecognizer();
        TapHome.SetBinding(TapGestureRecognizer.CommandProperty,
            new Binding(nameof(HomeCommand), source: this));
        AddTap(StackHome, TapHome);

        StackShoppingCart = CreateStack(out LabelShoppingCartIcon, out LabelShoppingCartText, "\uF07A", "Shopping\nCart");
        TapShoppingCart = new TapGestureRecognizer();
        TapShoppingCart.SetBinding(TapGestureRecognizer.CommandProperty,
            new Binding(nameof(CartCommand), source: this));
        AddTap(StackShoppingCart, TapShoppingCart);

        StackPurchaseHistory = CreateStack(out LabelPurchaseHistoryIcon, out LabelPurchaseHistoryText, "\uF571", "Order\nHistory");
        TapPurchaseHistory = new TapGestureRecognizer();
        TapPurchaseHistory.SetBinding(TapGestureRecognizer.CommandProperty,
            new Binding(nameof(HistoryCommand), source: this));
        AddTap(StackPurchaseHistory, TapPurchaseHistory);

        StackShopNow = CreateStack(out LabelShopNowIcon, out LabelShopNowText, "\uF0CA", "Shop Now");
        TapShopNow = new TapGestureRecognizer();
        TapShopNow.SetBinding(TapGestureRecognizer.CommandProperty,
            new Binding(nameof(ShopCommand), source: this));
        AddTap(StackShopNow, TapShopNow);

        StackScanBarcode = CreateStack(out LabelScanBarcodeIcon, out LabelScanBarcodeText, "\uF464", "Scan\nBarcode", "FontAwesomePro6Regular");
        TapScanBarcode = new TapGestureRecognizer();
        TapScanBarcode.SetBinding(TapGestureRecognizer.CommandProperty,
           new Binding(nameof(ScanCommand), source: this));
        AddTap(StackScanBarcode, TapScanBarcode);
    }

    VerticalStackLayout CreateStack(out Image icon, out Label text, string glyph, string label, string font = "FontAwesomeFreeSolid")
    {
        var stack = new VerticalStackLayout
        {
            BackgroundColor = Colors.Blue,
            HeightRequest = HeightRequest,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            stack.Margin = new Thickness(0, 5, 0, 0);
        }

        glyph = glyph.Replace("/u", "0x");
        icon = new Image
        {
            Margin = new Thickness(0, 10, 0, 0),
            Source = new FontImageSource
            {
                Glyph = glyph,
                FontFamily = font,
                Size = 22,
                Color = Colors.White
            }
        };

        text = new Label
        {
            Text = label,
            TextColor = Colors.White,
            FontSize = 10,
            HorizontalTextAlignment = TextAlignment.Center
        };

        stack.Children.Add(icon);
        stack.Children.Add(text);
        Grid.SetColumn(stack, gridContainer.Children.Count);
        gridContainer.Children.Add(stack);

        return stack;
    }

    void AddTap(VerticalStackLayout stack, TapGestureRecognizer tap)
    {
        stack.GestureRecognizers.Add(tap);
    }
}
