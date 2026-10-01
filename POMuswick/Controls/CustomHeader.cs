using System.Windows.Input;

namespace POMuswick.Controls;

public class CustomHeader : StackLayout
{
    StackLayout StackContainer;
    StackLayout StackBack;
    Image BackIcon;
    TapGestureRecognizer TapBack;
    private Label TitleText;
    public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(CustomHeader),
            string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty BackCommandProperty =
    BindableProperty.Create(
        nameof(BackCommand),
        typeof(ICommand),
        typeof(CustomHeader));

    public ICommand BackCommand
    {
        get => (ICommand)GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    public CustomHeader()
    {
        var white = GetColor("White", Colors.White);
        var ink = GetColor("Ink", Colors.Black);
        var primary = GetColor("Primary", Colors.Teal);

        Orientation = StackOrientation.Horizontal;
        HeightRequest = 60;
        BackgroundColor = white;

        StackContainer = new StackLayout { Orientation = StackOrientation.Horizontal, BackgroundColor = white, HeightRequest = 60, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Center };

        Children.Add(StackContainer);

        StackBack = new StackLayout { Orientation = StackOrientation.Vertical, BackgroundColor = white, WidthRequest = 60, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Center };
        StackContainer.Children.Add(StackBack);
        TapBack = new TapGestureRecognizer();
        TapBack.SetBinding(TapGestureRecognizer.CommandProperty,
            new Binding(nameof(BackCommand), source: this));
        StackBack.GestureRecognizers.Add(TapBack);
        BackIcon = new Image { BackgroundColor = white, Margin = new Thickness(0, 0, 0, 0) };
        BackIcon.VerticalOptions = LayoutOptions.Center;
        BackIcon.Source = new FontImageSource { Glyph = "\uF060", FontFamily = "FontAwesomeFreeSolid", Size = 20, Color = primary };
        BackIcon.GestureRecognizers.Add(TapBack);
        StackBack.Children.Add(BackIcon);

        TitleText = new Label { Margin = new Thickness(12, 0, 0, 0), TextColor = ink, FontSize = 21, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Fill, HorizontalTextAlignment = TextAlignment.Start, VerticalTextAlignment = TextAlignment.Center };
        TitleText.SetBinding(
            Label.TextProperty,
            new Binding(nameof(Title), source: this));
        StackContainer.Children.Add(TitleText);
    }

    private static Color GetColor(string key, Color fallback)
    {
        var resources = Application.Current?.Resources;
        return resources != null && resources.TryGetValue(key, out var value) && value is Color color
            ? color
            : fallback;
    }
}