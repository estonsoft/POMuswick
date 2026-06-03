using System.Windows.Input;

namespace POMuswick.Controls;

public partial class CustomAlertDialog : ContentView
{
    // Bindable: Title
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(CustomAlertDialog), "Alert");

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    // Bindable: Message
    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(CustomAlertDialog), string.Empty);

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    // Bindable: IsLoading (shows/hides ActivityIndicator)
    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(CustomAlertDialog), true);

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    
    public CustomAlertDialog()
    {
        InitializeComponent();
    }
}