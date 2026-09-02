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

        PlusBtn = new ImageButton { MaximumWidthRequest = 30, MaximumHeightRequest = 30, Source = "blue_plus.png", Aspect = Aspect.AspectFit, BackgroundColor = Colors.Transparent, VerticalOptions = LayoutOptions.Center };
        PlusBtn.Clicked += PlusBtn_Clicked;
        PlusBtn.SetBinding(IsVisibleProperty, new Binding(nameof(IsStepperVisible), source: this));

        MinusBtn = new ImageButton { MaximumWidthRequest = 30, MaximumHeightRequest = 30, Source = "blue_minus.png", Aspect = Aspect.AspectFit, BackgroundColor = Colors.Transparent, VerticalOptions = LayoutOptions.Center };
        MinusBtn.Clicked += MinusBtn_Clicked;
        MinusBtn.SetBinding(IsVisibleProperty, new Binding(nameof(IsStepperVisible), source: this));

        AddToOrderBtn = new Button { Text = "Add", HeightRequest = 30, WidthRequest = 100, CornerRadius = 20, Padding = Thickness.Zero, TextTransform = TextTransform.None, FontSize = 16, FontAttributes = FontAttributes.Bold, BackgroundColor = Colors.LightGray, TextColor = Colors.Blue, VerticalOptions = LayoutOptions.Center };
        AddToOrderBtn.Clicked += PlusBtn_Clicked;
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

    private async void MinusBtn_Clicked(object sender, EventArgs e)
    {
        if (Text <= 0) return;

        int iQty = await App.g_db.GetItemQty(ItemNo);
        if (iQty > 0)
        {
            await App.g_db.UpdateItemQty(ItemNo, -1);
        }

        Text--;
        QtyOrder--;
        App.g_ShoppingCartItems = await App.g_db.GetCartPieces();

        try { App.g_ShoppingCartPage.UpdateTotals(); } catch { }
        try { App.g_CheckoutPage.UpdateTotals(); } catch { }

        if (Text == 0)
        {
            IsStepperVisible = false;
            IsAddToOrderVisible = true;
        }
    }

    private async void PlusBtn_Clicked(object sender, EventArgs e)
    {
        if (Text == 999) return;
        if ((Text >= MaxOrderQty) && (MaxOrderQty > 0)) return;

        await App.g_db.UpdateItemQty(ItemNo, 1);

        Text++;
        QtyOrder++;
        App.g_ShoppingCartItems = await App.g_db.GetCartPieces();

        try { App.g_ShoppingCartPage.UpdateTotals(); } catch { }
        try { App.g_CheckoutPage.UpdateTotals(); } catch { }

        IsStepperVisible = true;
        IsAddToOrderVisible = false;
    }
}
