namespace POMuswick.Controls;

public class CustomHeader : StackLayout
{
    StackLayout StackContainer;
    StackLayout StackBack;
    Image BackIcon;
    Label TitleText;
    TapGestureRecognizer TapBack;

    public CustomHeader()
    {
        Orientation = StackOrientation.Horizontal;
        HeightRequest = 60;
        BackgroundColor = Colors.White;

        StackContainer = new StackLayout { Orientation = StackOrientation.Horizontal, BackgroundColor = Colors.White, HeightRequest = 60, HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };

        Children.Add(StackContainer);

        StackBack = new StackLayout { Orientation = StackOrientation.Vertical, BackgroundColor = Colors.White, WidthRequest = 60, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
        StackContainer.Children.Add(StackBack);
        TapBack = new TapGestureRecognizer();
        TapBack.Tapped += (sender, e) =>
        {
            OnBackTapped(sender, e);
        };

        BackIcon = new Image { BackgroundColor = Colors.White, Margin = new Thickness(0, 0, 0, 0) };
        BackIcon.Source = new FontImageSource { Glyph = "\uF060", FontFamily = "FontAwesomeFreeSolid", Size = 20, Color = Colors.Blue };
        BackIcon.GestureRecognizers.Add(TapBack);
        StackBack.Children.Add(BackIcon);

        TitleText = new Label { Margin = new Thickness(12, 0, 0, 0), TextColor = Colors.Blue, FontSize = 21, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.StartAndExpand, HorizontalTextAlignment = TextAlignment.Start, VerticalTextAlignment = TextAlignment.Center };
        TitleText.Text = App.g_HeaderTitle;
        StackContainer.Children.Add(TitleText);
    }

    async void OnBackTapped(object sender, EventArgs e)
    {
        if (TitleText.Text == "Checkout")
        {
            await App.g_Shell.GoToShoppingCart();
        }
        else if (TitleText.Text == "Submit Order")
        {
            await App.g_Shell.GoToHome();
        }
        else if (TitleText.Text == "Order Detail")
        {
            await App.g_Shell.GoToMyPurchases();
        }
        else if (TitleText.Text == "Settings")
        {
            await App.g_Shell.GoToHome();
        }
        else if (TitleText.Text == "Search Products")
        {
            if (App.g_SearchFromPage == "CategoryPage")
            {
                await App.g_Shell.GoToCategories();
            }
            else if (App.g_SearchFromPage == "PurchaseHistoryPage")
            {
                await App.g_Shell.GoToMyPurchases();
            }
            else if (App.g_SearchFromPage == "ReorderItemsPage")
            {
                await App.g_Shell.GoToReorderItems();
            }
            else if (App.g_SearchFromPage == "HomePage")
            {
                await App.g_Shell.GoToHome();
            }
            else
            {
                await App.g_Shell.GoToCategories();
            }
        }
        else
        {
            await App.g_Shell.GoToHome();
        }
    }
}