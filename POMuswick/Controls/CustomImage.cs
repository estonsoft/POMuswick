using POMuswick.Controls;

namespace Profit_Order.Controls
{
    public class CustomImage : StackLayout
    {
        public static readonly BindableProperty ItemNoProperty = BindableProperty.Create("ItemNo", typeof(int), typeof(NumericEntryBehavior), 0);

        public int ItemNo
        {
            get => (int)GetValue(ItemNoProperty);
            set => SetValue(ItemNoProperty, value);
        }

        StackLayout StackContainer;
        TapGestureRecognizer TapShowImage;

        public CustomImage()
        {
            Orientation = StackOrientation.Horizontal;
            HeightRequest = 60;
            BackgroundColor = Colors.White;

            StackContainer = new StackLayout { Orientation = StackOrientation.Horizontal, BackgroundColor = Colors.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

            Children.Add(StackContainer);

            TapShowImage = new TapGestureRecognizer();
            TapShowImage.Tapped += (sender, e) =>
            {
                OnImageTapped(sender, e);
            };
        }

        void OnImageTapped(object sender, EventArgs e)
        {
        }
    }
}
