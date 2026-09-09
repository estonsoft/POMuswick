using FFImageLoading.Maui;

namespace POMuswick.Controls
{
    public partial class CustomSmallListItem : ContentView
    {
        public CustomSmallListItem()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, EventArgs e)
        {
            // 1. Get the data model from the cell
            if (BindingContext is Item model && !string.IsNullOrEmpty(model.ImageURL))
            {
                MyImage.Source = ImageSource.FromUri(new Uri(model.ImageURL));
            }
            MyImage.IsVisible = true;
        }

        private void Grid_Unloaded(object sender, EventArgs e)
        {
            MyImage.IsVisible = false;
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