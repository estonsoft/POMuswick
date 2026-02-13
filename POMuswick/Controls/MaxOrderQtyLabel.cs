namespace Profit_Order.Controls
{
    public class MaxOrderQtyLabel : Label
    {
        public static readonly BindableProperty MaxOrderQtyDisplayProperty = BindableProperty.Create("MaxOrderQtyDisplay", typeof(string), typeof(string));

        public string MaxOrderQtyDisplay
        {
            get => (string)GetValue(MaxOrderQtyDisplayProperty);
            set => SetValue(MaxOrderQtyDisplayProperty, value);
        }

        public MaxOrderQtyLabel()
        {
        }
    }
}
