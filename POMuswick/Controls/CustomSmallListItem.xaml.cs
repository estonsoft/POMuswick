using System.Windows.Input;

namespace POMuswick.Controls;

public partial class CustomSmallListItem : ContentView
{
    public CustomSmallListItem()
    {
        InitializeComponent();
    }

    #region ImageViewCommand

    public static readonly BindableProperty ImageViewCommandProperty =
        BindableProperty.Create(
            nameof(ImageViewCommand),
            typeof(ICommand),
            typeof(CustomSmallListItem));

    public ICommand? ImageViewCommand
    {
        get => (ICommand?)GetValue(ImageViewCommandProperty);
        set => SetValue(ImageViewCommandProperty, value);
    }

    #endregion

    #region PlusCommand

    public static readonly BindableProperty PlusCommandProperty =
        BindableProperty.Create(
            nameof(PlusCommand),
            typeof(ICommand),
            typeof(CustomSmallListItem));

    public ICommand? PlusCommand
    {
        get => (ICommand?)GetValue(PlusCommandProperty);
        set => SetValue(PlusCommandProperty, value);
    }

    #endregion

    #region MinusCommand

    public static readonly BindableProperty MinusCommandProperty =
        BindableProperty.Create(
            nameof(MinusCommand),
            typeof(ICommand),
            typeof(CustomSmallListItem));

    public ICommand? MinusCommand
    {
        get => (ICommand?)GetValue(MinusCommandProperty);
        set => SetValue(MinusCommandProperty, value);
    }

    #endregion

    #region CommandParameter

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(
            nameof(CommandParameter),
            typeof(object),
            typeof(CustomSmallListItem));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    #endregion
}