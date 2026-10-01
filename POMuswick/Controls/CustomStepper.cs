using System.Windows.Input;

namespace POMuswick.Controls;

public partial class CustomStepper : ContentView
{
    public static readonly BindableProperty ItemNoProperty = BindableProperty.Create("ItemNo", typeof(int), typeof(CustomStepper), 0);
    public static readonly BindableProperty QtyOrderProperty = BindableProperty.Create("QtyOrder", typeof(int), typeof(CustomStepper), 0);
    public static readonly BindableProperty MaxOrderQtyProperty = BindableProperty.Create("MaxOrderQty", typeof(int), typeof(CustomStepper), 0);
    public static readonly BindableProperty UOMProperty = BindableProperty.Create("UOM", typeof(string), typeof(CustomStepper), "");
    public static readonly BindableProperty TextProperty = BindableProperty.Create(propertyName: "Text", returnType: typeof(int), declaringType: typeof(CustomStepper), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay);
    public static readonly BindableProperty IsStepperVisibleProperty = BindableProperty.Create(propertyName: "IsStepperVisible", returnType: typeof(bool), declaringType: typeof(CustomStepper), defaultValue: false, defaultBindingMode: BindingMode.TwoWay);
    public static readonly BindableProperty IsAddToOrderVisibleProperty = BindableProperty.Create(propertyName: "IsAddToOrderVisible", returnType: typeof(bool), declaringType: typeof(CustomStepper), defaultValue: false, defaultBindingMode: BindingMode.TwoWay);
    public static readonly BindableProperty IsMaxOrderQtyVisibleProperty = BindableProperty.Create(propertyName: "IsMaxOrderQtyVisible", returnType: typeof(bool), declaringType: typeof(CustomStepper), defaultValue: false);

    public int ItemNo
    {
        get => (int)GetValue(ItemNoProperty);
        set => SetValue(ItemNoProperty, value);
    }

    public int QtyOrder
    {
        get => (int)GetValue(QtyOrderProperty);
        set => SetValue(QtyOrderProperty, value);
    }

    public int MaxOrderQty
    {
        get => (int)GetValue(MaxOrderQtyProperty);
        set => SetValue(MaxOrderQtyProperty, value);
    }

    public string UOM
    {
        get => (string)GetValue(UOMProperty);
        set => SetValue(UOMProperty, value);
    }

    public int Text
    {
        get { return (int)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public bool IsStepperVisible
    {
        get { return (bool)GetValue(IsStepperVisibleProperty); }
        set { SetValue(IsStepperVisibleProperty, value); }
    }

    public bool IsAddToOrderVisible
    {
        get { return (bool)GetValue(IsAddToOrderVisibleProperty); }
        set { SetValue(IsAddToOrderVisibleProperty, value); }
    }

    public bool IsMaxOrderQtyVisible
    {
        get { return (bool)GetValue(IsMaxOrderQtyVisibleProperty); }
        set { SetValue(IsMaxOrderQtyVisibleProperty, value); }
    }

    public static readonly BindableProperty PlusCommandProperty =
            BindableProperty.Create(
                nameof(PlusCommand),
                typeof(ICommand),
                typeof(CustomStepper));

    public ICommand? PlusCommand
    {
        get => (ICommand?)GetValue(PlusCommandProperty);
        set => SetValue(PlusCommandProperty, value);
    }

    public static readonly BindableProperty MinusCommandProperty =
        BindableProperty.Create(
            nameof(MinusCommand),
            typeof(ICommand),
            typeof(CustomStepper));

    public ICommand? MinusCommand
    {
        get => (ICommand?)GetValue(MinusCommandProperty);
        set => SetValue(MinusCommandProperty, value);
    }

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(
            nameof(CommandParameter),
            typeof(object),
            typeof(CustomStepper));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public CustomStepper()
    {
        InitializeComponent();
    }
}
