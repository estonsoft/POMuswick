namespace POMuswick.Controls;

public class CustomStepper : StackLayout
{
    public static readonly BindableProperty ItemNoProperty = BindableProperty.Create("ItemNo", typeof(int), typeof(NumericEntryBehavior), 0);
    public static readonly BindableProperty QtyOrderProperty = BindableProperty.Create("QtyOrder", typeof(int), typeof(NumericEntryBehavior), 0);
    public static readonly BindableProperty MaxOrderQtyProperty = BindableProperty.Create("MaxOrderQty", typeof(int), typeof(NumericEntryBehavior), 0);
    public static readonly BindableProperty UOMProperty = BindableProperty.Create("UOM", typeof(string), typeof(string), "");
    public static readonly BindableProperty TextProperty = BindableProperty.Create(propertyName: "Text", returnType: typeof(int), declaringType: typeof(CustomStepper), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay);
    public static readonly BindableProperty IsStepperVisibleProperty = BindableProperty.Create(propertyName: "IsStepperVisible", returnType: typeof(bool), declaringType: typeof(StackLayout), defaultValue: false, defaultBindingMode: BindingMode.TwoWay);
    public static readonly BindableProperty IsAddToOrderVisibleProperty = BindableProperty.Create(propertyName: "IsAddToOrderVisible", returnType: typeof(bool), declaringType: typeof(Button), defaultValue: false, defaultBindingMode: BindingMode.TwoWay);

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
    StackLayout QtyStack;
    Label QtyLabel;
    Label InCartLabel;
    StackLayout AddToOrderStack;
    Button AddToOrderBtn;

    public CustomStepper()
    {
        Orientation = StackOrientation.Horizontal;
        HeightRequest = 30;

        PlusBtn = new ImageButton { WidthRequest = 40, HeightRequest = 40, Source = "blue_plus.png", Aspect = Aspect.AspectFit, BackgroundColor = Colors.Transparent };
        PlusBtn.Clicked += PlusBtn_Clicked;
        PlusBtn.SetBinding(Label.IsVisibleProperty, new Binding(nameof(IsStepperVisible), BindingMode.TwoWay, source: this));

        MinusBtn = new ImageButton { WidthRequest = 40, HeightRequest = 40, Source = "blue_minus.png", Aspect = Aspect.AspectFit, BackgroundColor = Colors.Transparent };
        MinusBtn.Clicked += MinusBtn_Clicked;
        MinusBtn.SetBinding(Label.IsVisibleProperty, new Binding(nameof(IsStepperVisible), BindingMode.TwoWay, source: this));

        AddToOrderStack = new StackLayout { Orientation = StackOrientation.Vertical, Margin = new Thickness(0, 5, 0, 0) };

        AddToOrderBtn = new Button { Text = "Add To Order", HeightRequest = 33, WidthRequest = 120, CornerRadius = 15, Margin = new Thickness(0, -5, 0, 0), Padding = new Thickness(0, 0, 0, 0), TextTransform = TextTransform.None, FontSize = 16, FontAttributes = FontAttributes.Bold, BackgroundColor = Colors.LightGray, TextColor = Colors.Blue };
        AddToOrderBtn.Clicked += PlusBtn_Clicked;
        AddToOrderBtn.SetBinding(Button.IsVisibleProperty, new Binding(nameof(IsAddToOrderVisible), BindingMode.TwoWay, source: this));

        AddToOrderStack.Children.Add(AddToOrderBtn);

        QtyStack = new StackLayout { Orientation = StackOrientation.Vertical };

        QtyLabel = new Label { WidthRequest = 35, Margin = new Thickness(0, 0, 0, 0), TextColor = Colors.Black, FontSize = 20, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.CenterAndExpand, HorizontalTextAlignment = TextAlignment.Center };
        QtyLabel.SetBinding(Label.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
        QtyLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(IsStepperVisible), BindingMode.TwoWay, source: this));

        InCartLabel = new Label { Text = "In Cart", WidthRequest = 35, Margin = new Thickness(0, -8, 0, 0), TextColor = Colors.Gray, FontSize = 10, HorizontalOptions = LayoutOptions.CenterAndExpand, HorizontalTextAlignment = TextAlignment.Center, MaxLines = 1 };
        InCartLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(IsStepperVisible), BindingMode.TwoWay, source: this));

        QtyStack.Children.Add(QtyLabel);
        QtyStack.Children.Add(InCartLabel);

        Children.Add(MinusBtn);
        Children.Add(QtyStack);
        Children.Add(PlusBtn);
        Children.Add(AddToOrderStack);
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        int item = ItemNo;

        if (!string.IsNullOrEmpty(e.NewTextValue))
        {
            try
            {
                this.Text = int.Parse(e.NewTextValue);
            }
            catch
            {
            }
        }
    }

    private async void MinusBtn_Clicked(object sender, EventArgs e)
    {
        if (Text > 0)
        {
            //Database db = new Database();
            int iQty = App.g_db.GetItemQty(ItemNo);

            if (iQty > 0)
            {
                App.g_db.UpdateItemQty(ItemNo, -1);
            }

            Text--;
            QtyOrder--;

            App.g_ShoppingCartItems = App.g_db.GetCartPieces();

            try
            {
                App.g_ShoppingCartPage.UpdateTotals();
            }
            catch { }

            try
            {
                App.g_CheckoutPage.UpdateTotals();
            }
            catch { }

            if (Text == 0)
            {
                try
                {
                    App.g_ShoppingCartPage.UpdateTotals();
                }
                catch { }
                try
                {
                    App.g_CheckoutPage.UpdateTotals();
                }
                catch { }

                IsStepperVisible = false;
                IsAddToOrderVisible = true;
            }
        }
    }

    private void PlusBtn_Clicked(object sender, EventArgs e)
    {
        if (Text == 999)
        {
            return;
        }

        if ((Text >= MaxOrderQty) && (MaxOrderQty > 0))
        {
            return;
        }

        //Database db = new Database();
        App.g_db.UpdateItemQty(ItemNo, 1);

        Text++;
        QtyOrder++;

        App.g_ShoppingCartItems = App.g_db.GetCartPieces();

        try
        {
            App.g_ShoppingCartPage.UpdateTotals();
        }
        catch { }

        try
        {
            App.g_CheckoutPage.UpdateTotals();
        }
        catch { }

        IsStepperVisible = true;
        IsAddToOrderVisible = false;
    }
}
