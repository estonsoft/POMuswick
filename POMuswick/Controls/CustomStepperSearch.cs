namespace POMuswick.Controls;

public class CustomStepperSearch : Grid
{
    public static readonly BindableProperty ItemNoProperty =
        BindableProperty.Create(nameof(ItemNo), typeof(int), typeof(CustomStepperSearch), 0);

    public static readonly BindableProperty QtyOrderProperty =
        BindableProperty.Create(nameof(QtyOrder), typeof(int), typeof(CustomStepperSearch), 0);

    public static readonly BindableProperty UOMProperty =
        BindableProperty.Create(nameof(UOM), typeof(string), typeof(CustomStepperSearch), string.Empty);

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(int), typeof(CustomStepperSearch), 0, BindingMode.TwoWay);

    public static readonly BindableProperty IsStepperVisibleProperty =
        BindableProperty.Create(nameof(IsStepperVisible), typeof(bool), typeof(CustomStepperSearch), false, BindingMode.TwoWay);

    public static readonly BindableProperty IsAddToOrderVisibleProperty =
        BindableProperty.Create(nameof(IsAddToOrderVisible), typeof(bool), typeof(CustomStepperSearch), false, BindingMode.TwoWay);

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

    public string UOM
    {
        get => (string)GetValue(UOMProperty);
        set => SetValue(UOMProperty, value);
    }

    public int Text
    {
        get => (int)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsStepperVisible
    {
        get => (bool)GetValue(IsStepperVisibleProperty);
        set => SetValue(IsStepperVisibleProperty, value);
    }

    public bool IsAddToOrderVisible
    {
        get => (bool)GetValue(IsAddToOrderVisibleProperty);
        set => SetValue(IsAddToOrderVisibleProperty, value);
    }

    ImageButton PlusBtn;
    ImageButton MinusBtn;
    VerticalStackLayout QtyStack; // Replaced StackLayout to optimize multi-row sizing
    Entry QtyLabel;
    Border QtyLabelBorder;
    Label InCartLabel;
    Button AddToOrderBtn;

    public CustomStepperSearch()
    {
        // Setup a strict cross-platform horizontal layout matrix using a Grid layout container
        ColumnDefinitions = new ColumnDefinitionCollection
        {
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Auto }
        };

        // Correct collection instantiation prevents cross-platform runtime exceptions
        RowDefinitions = new RowDefinitionCollection();

        ColumnSpacing = 4;
        VerticalOptions = LayoutOptions.Center;

        PlusBtn = new ImageButton
        {
            WidthRequest = 40,
            HeightRequest = 40,
            Source = "blue_plus.png",
            Aspect = Aspect.AspectFit,
            BackgroundColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.Center
        };
        PlusBtn.Clicked += PlusBtn_Clicked;
        PlusBtn.SetBinding(IsVisibleProperty, new Binding(nameof(IsStepperVisible), source: this));

        MinusBtn = new ImageButton
        {
            WidthRequest = 40,
            HeightRequest = 40,
            Source = "blue_minus.png",
            Aspect = Aspect.AspectFit,
            BackgroundColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.Center
        };
        MinusBtn.Clicked += MinusBtn_Clicked;
        MinusBtn.SetBinding(IsVisibleProperty, new Binding(nameof(IsStepperVisible), source: this));

        AddToOrderBtn = new Button
        {
            Text = "Add",
            HeightRequest = 40,
            WidthRequest = 120,
            CornerRadius = 15,
            Padding = Thickness.Zero,
            TextTransform = TextTransform.None,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = Colors.LightGray,
            TextColor = Colors.Blue,
            VerticalOptions = LayoutOptions.Center
        };
        AddToOrderBtn.Clicked += PlusBtn_Clicked;
        AddToOrderBtn.SetBinding(IsVisibleProperty, new Binding(nameof(IsAddToOrderVisible), source: this));

        QtyStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, Spacing = 2 };

        QtyLabel = new Entry
        {
            WidthRequest = 35,
            HeightRequest = 30, // Form-fitted height matches border frame limits cleanly
            Margin = Thickness.Zero,
            TextColor = Colors.Black,
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            BackgroundColor = Colors.Transparent,
            Keyboard = Keyboard.Numeric, // Displays the numeric software keypad automatically,
            MaxLength = 3
        };
        QtyLabel.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
        QtyLabel.TextChanged += Entry_TextChanged;

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

        InCartLabel = new Label
        {
            Text = "In Cart",
            WidthRequest = 35,
            Margin = Thickness.Zero,
            TextColor = Colors.Gray,
            FontSize = 10,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            MaxLines = 1
        };

        QtyStack.Children.Add(QtyLabelBorder);
        QtyStack.Children.Add(InCartLabel);
        QtyStack.SetBinding(IsVisibleProperty, new Binding(nameof(IsStepperVisible), source: this));

        // Maps control objects to explicit Grid coordinate indexes
        Grid.SetColumn(MinusBtn, 0);
        Grid.SetColumn(QtyStack, 1);
        Grid.SetColumn(PlusBtn, 2);
        Grid.SetColumn(AddToOrderBtn, 3);

        Children.Add(MinusBtn);
        Children.Add(QtyStack);
        Children.Add(PlusBtn);
        Children.Add(AddToOrderBtn);
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Prevent infinite loops during binding updates
        if (string.IsNullOrEmpty(e.NewTextValue)) return;

        if (int.TryParse(e.NewTextValue, out int newQty))
        {
            //// 1. Enforce MaxOrderQty bounds check if applicable
            //if (MaxOrderQty > 0 && newQty > MaxOrderQty)
            //{
            //    newQty = MaxOrderQty;
            //    QtyLabel.Text = newQty.ToString(); // Force UI to respect the limit
            //    return;
            //}

            // 2. Calculate the difference between the old quantity and the new quantity
            int currentDbQty = App.g_db.GetItemQty(ItemNo);
            int difference = newQty - currentDbQty;

            // 3. Update the database using your existing delta-based system
            if (difference != 0)
            {
                App.g_db.UpdateItemQty(ItemNo, difference);
            }

            // 4. Update control states and synchronizations
            this.Text = newQty;
            this.QtyOrder = newQty;
            App.g_ShoppingCartItems = App.g_db.GetCartPieces();

            // 5. Safely handle visibility transitions if the user types '0'
            if (newQty == 0)
            {
                IsStepperVisible = false;
                IsAddToOrderVisible = true;
            }
            else
            {
                IsStepperVisible = true;
                IsAddToOrderVisible = false;
            }

            // 6. Refresh pages
            try { App.g_ShoppingCartPage.UpdateTotals(); } catch { }
            try { App.g_CheckoutPage.UpdateTotals(); } catch { }
        }
    }


    void MinusBtn_Clicked(object sender, EventArgs e)
    {
        if (Text <= 0)
            return;

        int iQty = App.g_db.GetItemQty(ItemNo);
        if (iQty > 0)
            App.g_db.UpdateItemQty(ItemNo, -1);

        Text--;
        QtyOrder--;
        App.g_ShoppingCartItems = App.g_db.GetCartPieces();

        try { App.g_ShoppingCartPage.UpdateTotals(); } catch { }
        try { App.g_CheckoutPage.UpdateTotals(); } catch { }

        if (Text == 0)
        {
            IsStepperVisible = false;
            IsAddToOrderVisible = true;
        }
    }

    void PlusBtn_Clicked(object sender, EventArgs e)
    {
        if (Text == 999)
            return;

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
