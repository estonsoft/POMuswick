using System.Windows.Input;

namespace POMuswick.Controls;

public class CustomStepper : Grid
{
    public static readonly BindableProperty ItemNoProperty = BindableProperty.Create("ItemNo", typeof(int), typeof(CustomStepper), 0);
    public static readonly BindableProperty QtyOrderProperty = BindableProperty.Create("QtyOrder", typeof(int), typeof(CustomStepper), 0);
    public static readonly BindableProperty MaxOrderQtyProperty = BindableProperty.Create("MaxOrderQty", typeof(int), typeof(CustomStepper), 0);
    public static readonly BindableProperty UOMProperty = BindableProperty.Create("UOM", typeof(string), typeof(CustomStepper), "");
    public static readonly BindableProperty TextProperty = BindableProperty.Create(propertyName: "Text", returnType: typeof(int), declaringType: typeof(CustomStepper), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay);
    public static readonly BindableProperty IsStepperVisibleProperty = BindableProperty.Create(propertyName: "IsStepperVisible", returnType: typeof(bool), declaringType: typeof(CustomStepper), defaultValue: false, defaultBindingMode: BindingMode.TwoWay);
    public static readonly BindableProperty IsAddToOrderVisibleProperty = BindableProperty.Create(propertyName: "IsAddToOrderVisible", returnType: typeof(bool), declaringType: typeof(CustomStepper), defaultValue: false, defaultBindingMode: BindingMode.TwoWay);

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

    private void Plus_Clicked(object sender, EventArgs e)
    {
        
        if (PlusCommand?.CanExecute(CommandParameter) == true)
            PlusCommand.Execute(CommandParameter);
    }

    private void Minus_Clicked(object sender, EventArgs e)
    {
        if (MinusCommand?.CanExecute(CommandParameter) == true)
            MinusCommand.Execute(CommandParameter);
    }
    

    ImageButton PlusBtn;
    ImageButton MinusBtn;
    VerticalStackLayout QtyStack;
    Label QtyLabel;
    Border QtyLabelBorder;
    Label InCartLabel;
    Button AddToOrderBtn;

    public CustomStepper()
    {
        ColumnDefinitions = new ColumnDefinitionCollection
        {
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Star }
        };

        // FIX: Solved the type instantiation error by using RowDefinitionCollection
        RowDefinitions = new RowDefinitionCollection();

        ColumnSpacing = 4;
        VerticalOptions = LayoutOptions.Center;

        PlusBtn = new ImageButton { MaximumWidthRequest = 40, MaximumHeightRequest = 40, Source = "blue_plus.png", Aspect = Aspect.AspectFit, BackgroundColor = Colors.Transparent, VerticalOptions = LayoutOptions.Center };
        PlusBtn.Clicked += Plus_Clicked;
        PlusBtn.SetBinding(IsVisibleProperty, new Binding(nameof(IsStepperVisible), source: this));

        MinusBtn = new ImageButton { MaximumWidthRequest = 40, MaximumHeightRequest = 40, Source = "blue_minus.png", Aspect = Aspect.AspectFit, BackgroundColor = Colors.Transparent, VerticalOptions = LayoutOptions.Center };
        MinusBtn.Clicked += Minus_Clicked;
        MinusBtn.SetBinding(IsVisibleProperty, new Binding(nameof(IsStepperVisible), source: this));

        AddToOrderBtn = new Button { Text = "Add", HeightRequest = 40, WidthRequest = 100, CornerRadius = 20, Padding = Thickness.Zero, TextTransform = TextTransform.None, FontSize = 16, FontAttributes = FontAttributes.Bold, BackgroundColor = Colors.LightGray, TextColor = Colors.Blue, VerticalOptions = LayoutOptions.Center };
        AddToOrderBtn.Clicked += Plus_Clicked;
        AddToOrderBtn.SetBinding(IsVisibleProperty, new Binding(nameof(IsAddToOrderVisible), source: this));

        QtyStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, Spacing = 2 };

        QtyLabel = new Label
        {
            WidthRequest = 35,
            HeightRequest = 30,
            Margin = Thickness.Zero,
            TextColor = Colors.Black,
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            BackgroundColor = Colors.Transparent,
        };

        QtyLabel.SetBinding(Label.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));

        QtyLabelBorder = new Border
        {
            Stroke = Colors.LightGray,
            StrokeThickness = 1,
            HeightRequest = 32,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 4 },
            Padding = Thickness.Zero,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Content = QtyLabel
        };

        InCartLabel = new Label { Text = "In Cart", WidthRequest = 45, Margin = Thickness.Zero, TextColor = Colors.Gray, FontSize = 10, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center, MaxLines = 1 };

        QtyStack.Children.Add(QtyLabelBorder);
        QtyStack.Children.Add(InCartLabel);
        QtyStack.SetBinding(IsVisibleProperty, new Binding(nameof(IsStepperVisible), source: this));

        Grid.SetColumn(MinusBtn, 0);
        Grid.SetColumn(QtyStack, 1);
        Grid.SetColumn(PlusBtn, 2);
        Grid.SetColumn(AddToOrderBtn, 3);

        Children.Add(MinusBtn);
        Children.Add(QtyStack);
        Children.Add(PlusBtn);
        Children.Add(AddToOrderBtn);
    }
}
