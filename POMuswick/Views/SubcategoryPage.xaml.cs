namespace POMuswick.Views
{
    public partial class SubcategoryPage : ContentPage
    {

        public SubcategoryPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            App.g_SearchText = "";

            App.g_CurrentPage = "SubcategoryPage";

            //Database db = new Database();
            List<Subcategory> lst = App.g_db.GetSubcategory(App.g_Category.Code);

            SubcategoriesListSearch.ItemsSource = lst;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //Category item = (Category) e.SelectedItem;
            //App.g_Category = item.Code;

            //Shell.Current.GoToAsync("//HomePage/ItemSearchPage");
        }

        private async void OnSubcategoryTapped(object sender, ItemTappedEventArgs e)
        {
            App.g_Subcategory = (Subcategory)e.Item;
            App.g_ScanBarcode = "";

            await App.g_Shell.GoToItemSearch();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}