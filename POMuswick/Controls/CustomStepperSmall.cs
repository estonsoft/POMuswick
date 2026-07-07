namespace POMuswick.Controls;

public class CustomStepperSmall : Grid
{
    public static readonly BindableProperty ItemNoProperty = BindableProperty.Create("ItemNo", typeof(int), typeof(CustomStepperSmall), 0);
    public static readonly BindableProperty QtyOrderProperty = BindableProperty.Create("QtyOrder", typeof(int), typeof(CustomStepperSmall), 0);
    public static readonly BindableProperty MaxOrderQtyProperty = BindableProperty.Create("MaxOrderQty", typeof(int), typeof(CustomStepperSmall), 0);
    public static readonly BindableProperty UOMProperty = BindableProperty.Create("UOM", typeof(string), typeof(CustomStepperSmall), "");
    public static readonly BindableProperty SellUnitProperty = BindableProperty.Create("SellUnit", typeof(int), typeof(CustomStepperSmall), 0);
    public static readonly BindableProperty TextProperty = BindableProperty.Create(propertyName: "Text", returnType: typeof(int), declaringType: typeof(CustomStepperSmall), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay);
    public static readonly BindableProperty IsStepperVisibleProperty = BindableProperty.Create(propertyName: "IsStepperVisible", returnType: typeof(bool), declaringType: typeof(CustomStepperSmall), defaultValue: false, defaultBindingMode: BindingMode.TwoWay);
    public static readonly BindableProperty IsAddToOrderVisibleProperty = BindableProperty.Create(propertyName: "IsAddToOrderVisible", returnType: typeof(bool), declaringType: typeof(CustomStepperSmall), defaultValue: false, defaultBindingMode: BindingMode.TwoWay);
    public static readonly BindableProperty IsMaxOrderQtyVisibleProperty = BindableProperty.Create(propertyName: "IsMaxOrderQtyVisible", returnType: typeof(bool), declaringType: typeof(CustomStepperSmall), defaultValue: false, defaultBindingMode: BindingMode.TwoWay);

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

    public int SellUnit
    {
        get => (int)GetValue(SellUnitProperty);
        set => SetValue(SellUnitProperty, value);
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

    ImageButton PlusBtn;
    ImageButton MinusBtn;
    VerticalStackLayout QtyStack;
    Label QtyLabel;
    Border QtyLabelBorder;
    Label InCartLabel;
    Button AddToOrderBtn;

    public CustomStepperSmall()
    {
        ColumnDefinitions = new ColumnDefinitionCollection
        {
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Star }
        };

        RowDefinitions = new RowDefinitionCollection();

        ColumnSpacing = 4;
        VerticalOptions = LayoutOptions.Center;

        PlusBtn = new ImageButton { MaximumWidthRequest = 32, MaximumHeightRequest = 32, Source = "blue_plus.png", Aspect = Aspect.AspectFit, BackgroundColor = Colors.Transparent, VerticalOptions = LayoutOptions.Center };
        PlusBtn.Clicked += PlusBtn_Clicked;
        PlusBtn.SetBinding(IsVisibleProperty, new Binding(nameof(IsStepperVisible), source: this));

        MinusBtn = new ImageButton { MaximumWidthRequest = 32, MaximumHeightRequest = 32, Source = "blue_minus.png", Margin = new Thickness(5, 0, 0, 0), Aspect = Aspect.AspectFit, BackgroundColor = Colors.Transparent, VerticalOptions = LayoutOptions.Center };
        MinusBtn.Clicked += MinusBtn_Clicked;
        MinusBtn.SetBinding(IsVisibleProperty, new Binding(nameof(IsStepperVisible), source: this));

        AddToOrderBtn = new Button { Text = "Add", MaximumHeightRequest = 32, MaximumWidthRequest = 103, CornerRadius = 15, Padding = Thickness.Zero, TextTransform = TextTransform.None, FontSize = 14, FontAttributes = FontAttributes.Bold, BackgroundColor = Colors.LightGray, TextColor = Colors.Blue, VerticalOptions = LayoutOptions.Center };
        AddToOrderBtn.Clicked += PlusBtn_Clicked;
        AddToOrderBtn.SetBinding(IsVisibleProperty, new Binding(nameof(IsAddToOrderVisible), source: this));

        QtyStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, Spacing = 1 };

        QtyLabel = new Label
        {
            WidthRequest = 35,
            HeightRequest = 26,
            Margin = Thickness.Zero,
            TextColor = Colors.Black,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            BackgroundColor = Colors.Transparent
        };
        QtyLabel.SetBinding(Label.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));

        QtyLabelBorder = new Border
        {
            Stroke = Colors.LightGray,
            StrokeThickness = 1,
            HeightRequest = 28,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 4 },
            Padding = Thickness.Zero,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Content = QtyLabel
        };

        InCartLabel = new Label { Text = "In Cart", WidthRequest = 35, Margin = Thickness.Zero, TextColor = Colors.Gray, FontSize = 10, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center, MaxLines = 1 };

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

        int iQty = App.g_db.GetItemQty(ItemNo);
        if (iQty > 0)
        {
            App.g_db.UpdateItemQty(ItemNo, -1);
        }

        Text--;
        QtyOrder--;
        App.g_ShoppingCartItems = App.g_db.GetCartPieces();

        try { App.g_ShoppingCartPage.UpdateTotals(); } catch { }
        try { App.g_CheckoutPage.UpdateTotals(); } catch { }

        if (Text == 0)
        {
            try { App.g_ShoppingCartPage.UpdateTotals(); } catch { }
            try { App.g_CheckoutPage.UpdateTotals(); } catch { }

            IsStepperVisible = false;
            IsAddToOrderVisible = true;
        }
    }

    private void PlusBtn_Clicked(object sender, EventArgs e)
    {
        if (Text == 999) return;
        if ((Text >= MaxOrderQty) && (MaxOrderQty > 0)) return;

        App.g_db.UpdateItemQty(ItemNo, 1);

        Text++;
        QtyOrder++;
        App.g_ShoppingCartItems = App.g_db.GetCartPieces();

        try { App.g_ShoppingCartPage.UpdateTotals(); } catch { }
        try { App.g_CheckoutPage.UpdateTotals(); } catch { }

        IsStepperVisible = true;
        IsAddToOrderVisible = false;
    }
}
