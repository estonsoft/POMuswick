using POMuswick.ViewModels;

namespace POMuswick.Views
{
    public partial class CategoryPage : ContentPage
    {
        CategoryViewModel _viewModel;

        public CategoryPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new CategoryViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();

            App.g_CurrentPage = "CategoryPage";

            //App.g_Category.Code = "";
            //App.g_Category.Description = "ALL CATEGORIES";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            //App.g_Subsubcategory.Code = "";
            //App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";

            App.g_SearchText = "";
            App.g_ScanBarcode = "";

            List<Category> categories = App.g_db.GetCategories();
            CategoriesListSearch.ItemsSource = categories;
            if (App.g_Category.Code != "")
            {
                int index = categories.FindIndex(c => c.Code == App.g_Category.Code);
                CategoriesListSearch.ScrollTo(index,0,ScrollToPosition.Start, false);
            }

            App.g_Category.Code = "";
            App.g_Category.Description = "ALL CATEGORIES";
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //Category item = (Category) e.SelectedItem;
            //App.g_Category = item.Code;

            //App.g_Shell.GoToItemSearch();
        }

        private async void CategoriesListSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategory = e.CurrentSelection?.FirstOrDefault() as Category;
            if (selectedCategory == null)
                return;
            App.g_Category = selectedCategory;
            App.g_ScanBarcode = ""; 
            App.g_SearchFromPage = "CategoryPage";
            await App.g_Shell.GoToItemSearch();
        }        

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}