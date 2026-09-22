using System.Windows.Input;
using FFImageLoading.Maui;

namespace POMuswick.Controls;

public partial class ImageAlertView : ContentView
{
    // Bindable: ImageSource
    public static readonly BindableProperty ImageSourceProperty =
      BindableProperty.Create(
        nameof(ImageSource),
        typeof(ImageSource),
        typeof(ImageAlertView),
        default(ImageSource));

    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }


    public static readonly BindableProperty CloseCommandProperty =
    BindableProperty.Create(
        nameof(CloseCommand),
        typeof(ICommand),
        typeof(ImageAlertView));

    public ICommand CloseCommand
    {
        get => (ICommand)GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    public ImageAlertView()
    {
        InitializeComponent();
    }

    private async void CloseTapped(object sender, EventArgs e)
    {
        CloseCommand?.Execute(this);
    }
}