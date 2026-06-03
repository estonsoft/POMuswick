using FFImageLoading.Maui;

namespace POMuswick.Controls;

public partial class ImageAlertView : ContentView
{
    // Bindable: ImageSource
    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(
            nameof(ImageSource),
            typeof(string),
            typeof(ImageAlertView),
            default(string),
            propertyChanged: OnImageSourceChanged);

    public string ImageSource
    {
        get => (string)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ImageAlertView)bindable;
        control.PreviewImage.Source = newValue?.ToString();
    }

    // Events
    public event EventHandler? OkTapped;
    public event EventHandler? CancelTapped;
    public event EventHandler? Closed;

    public ImageAlertView()
    {
        InitializeComponent();
    }

    // Show with image
    public void Show(CachedImage image)
    {
        PreviewImage.Source = image.Source;
        IsVisible = true;
    }

    // Hide
    public void Hide()
    {
        IsVisible = false;
        PreviewImage.Source = null;
    }

    private void OnCloseTapped(object sender, TappedEventArgs e)
    {
        Hide();
        Closed?.Invoke(this, EventArgs.Empty);
    }
}