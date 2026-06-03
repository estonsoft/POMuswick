using FFImageLoading.Maui;

namespace POMuswick.Controls
{
    public partial class CustomSmallListItem : ContentView
    {
        public CustomSmallListItem()
        {
            InitializeComponent();
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            if (App.imageAlertView != null)
            {
                var selectedItem = sender as CachedImage;
                if (selectedItem == null)
                    return;
                App.imageAlertView.Show(selectedItem);
            }
        }
    }
}