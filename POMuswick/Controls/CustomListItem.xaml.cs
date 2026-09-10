using FFImageLoading.Args;
using FFImageLoading.Maui;

namespace POMuswick.Controls
{
    public partial class CustomListItem : ContentView
    {
        public CustomListItem()
        {
            InitializeComponent();
        }

        // private void DownloadStarted(object sender, EventArgs e)
        // {
        //     MyImage.IsVisible = false;
        // }

        // private void DownloadSuccess(object sender, SuccessEventArgs e)
        // {
        //     MyImage.IsVisible = true;
        // }

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