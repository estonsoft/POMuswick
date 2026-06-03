using FFImageLoading.Maui;

namespace POMuswick.Controls
{
    public partial class CustomListItem : ContentView
    {
        public CustomListItem()
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