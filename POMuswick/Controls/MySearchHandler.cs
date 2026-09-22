namespace POMuswick.Controls;

public class MySearchHandler : SearchHandler
{
    string searchText;
    public MySearchHandler()
    {
        FontSize = 12;
        ItemsSource = null;
        ShowsResults = false;
    }

    protected override void OnQueryChanged(string oldValue, string newValue)
    {
        ShowsResults = false;
        base.OnQueryChanged(oldValue, newValue);

        if (!string.IsNullOrEmpty(newValue))
        {
            searchText = newValue;
        }
    }

    protected override void OnQueryConfirmed()
    {
        GoToSearchPage();
    }

    protected async void GoToSearchPage()
    {
        // await App.g_Shell.GoToItemSearch();
    }

    protected override async void OnItemSelected(object item)
    {
        base.OnItemSelected(item);
    }
}
