using System.Windows.Input;

namespace POMuswick.Controls;

public partial class CustomSmallStepper : ContentView
{
    public CustomSmallStepper()
    {
        InitializeComponent();
    }

    #region QtyOrder

    public static readonly BindableProperty QtyOrderProperty =
        BindableProperty.Create(
            nameof(QtyOrder),
            typeof(int),
            typeof(CustomSmallStepper),
            0);

    public int QtyOrder
    {
        get => (int)GetValue(QtyOrderProperty);
        set => SetValue(QtyOrderProperty, value);
    }

    #endregion

    #region ItemNo

    public static readonly BindableProperty ItemNoProperty =
        BindableProperty.Create(
            nameof(ItemNo),
            typeof(int),
            typeof(CustomSmallStepper),
            0);

    public int ItemNo
    {
        get => (int)GetValue(ItemNoProperty);
        set => SetValue(ItemNoProperty, value);
    }

    #endregion

    #region MaxOrderQty

    public static readonly BindableProperty MaxOrderQtyProperty =
        BindableProperty.Create(
            nameof(MaxOrderQty),
            typeof(int),
            typeof(CustomSmallStepper),
            0);

    public int MaxOrderQty
    {
        get => (int)GetValue(MaxOrderQtyProperty);
        set => SetValue(MaxOrderQtyProperty, value);
    }

    #endregion

    #region UOM

    public static readonly BindableProperty UOMProperty =
        BindableProperty.Create(
            nameof(UOM),
            typeof(string),
            typeof(CustomSmallStepper),
            string.Empty);

    public string UOM
    {
        get => (string)GetValue(UOMProperty);
        set => SetValue(UOMProperty, value);
    }

    #endregion

    #region Visibility

    public static readonly BindableProperty IsStepperVisibleProperty =
        BindableProperty.Create(
            nameof(IsStepperVisible),
            typeof(bool),
            typeof(CustomSmallStepper),
            false);

    public bool IsStepperVisible
    {
        get => (bool)GetValue(IsStepperVisibleProperty);
        set => SetValue(IsStepperVisibleProperty, value);
    }

    public static readonly BindableProperty IsAddToOrderVisibleProperty =
        BindableProperty.Create(
            nameof(IsAddToOrderVisible),
            typeof(bool),
            typeof(CustomSmallStepper),
            true);

    public bool IsAddToOrderVisible
    {
        get => (bool)GetValue(IsAddToOrderVisibleProperty);
        set => SetValue(IsAddToOrderVisibleProperty, value);
    }

    #endregion

    #region Commands

    public static readonly BindableProperty PlusCommandProperty =
        BindableProperty.Create(
            nameof(PlusCommand),
            typeof(ICommand),
            typeof(CustomSmallStepper));

    public ICommand? PlusCommand
    {
        get => (ICommand?)GetValue(PlusCommandProperty);
        set => SetValue(PlusCommandProperty, value);
    }

    public static readonly BindableProperty MinusCommandProperty =
        BindableProperty.Create(
            nameof(MinusCommand),
            typeof(ICommand),
            typeof(CustomSmallStepper));

    public ICommand? MinusCommand
    {
        get => (ICommand?)GetValue(MinusCommandProperty);
        set => SetValue(MinusCommandProperty, value);
    }

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(
            nameof(CommandParameter),
            typeof(object),
            typeof(CustomSmallStepper));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    #endregion
}