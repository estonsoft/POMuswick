using FFImageLoading.Maui;

namespace POMuswick.Controls
{
    public partial class CustomListItem : ContentView
    {
        public CustomListItem()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, EventArgs e)
        {
            // 1. Get the data model from the cell
            if (BindingContext is Item model && !string.IsNullOrEmpty(model.ImageURL))
            {
                // 2. Show spinner while loading
                MySpinner.IsVisible = true;
                MySpinner.IsRunning = true;

                // 3. Set the image source explicitly
                MyImage.Source = ImageSource.FromUri(new Uri(model.ImageURL));

                // Hide spinner when done (or handle via event/async)
                MySpinner.IsVisible = false;
                MySpinner.IsRunning = false;
            }
        }

        private void Grid_Unloaded(object sender, EventArgs e)
        {
            // 1. Stop the spinner immediately
            MySpinner.IsRunning = false;
            MySpinner.IsVisible = false;

            // 2. Clear the image source to release memory when scrolling away
            MyImage.Source = null;

            // 3. Drop native platform references
#if ANDROID
        if (MyImage.Handler?.PlatformView is Android.Widget.ImageView nativeAndroid)
        {
            nativeAndroid.SetImageBitmap(null);
        }
#elif IOS
            if (MyImage.Handler?.PlatformView is UIKit.UIImageView nativeIOS)
            {
                nativeIOS.Image = null;
            }
#endif
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