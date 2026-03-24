namespace POMuswick.Views
{
    public partial class ItemSearchPage : ContentPage
    {
        string _category;
        string _subcategory;
        bool _inStockOnly;
        string _search_text;
        List<Item> lstItems;
        List<Item> lstKeywordItems;

        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
                OnPropertyChanged();
            }
        }

        public string Subcategory
        {
            get { return _subcategory; }
            set
            {
                _subcategory = value;
                OnPropertyChanged();
            }
        }

        public bool InStockOnlyValue
        {
            get { return _inStockOnly; }
            set
            {
                _inStockOnly = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get { return _search_text; }
            set
            {
                _search_text = value;
                OnPropertyChanged();
            }
        }

        public ItemSearchPage()
        {
            InitializeComponent();
            BindingContext = this;

            App.g_SearchPage = this;

            try
            {
                InStockOnlyValue = App.g_InStockOnly;
            }
            catch
            {
                InStockOnlyValue = false;
            }

            NavigationPage.SetHasNavigationBar(this, false);
            Shell.SetNavBarIsVisible(this, false);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "ItemSearchPage";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "";

            Search.Text = App.g_SearchText;

            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasBackButton(this, false);

            RefreshList();
        }

        public async void RefreshList()
        {
            ItemsListSearch.ItemsSource = null;

            Category = App.g_Category.Description;
            Subcategory = App.g_Subcategory.Description;

            //Database db = new Database();
            if (Category == "NEW ITEMS")
            {
                lstItems = App.g_db.GetNewItems(App.g_SearchText, false);
            }
            else
            {
                lstItems = App.g_db.SearchItems(App.g_SearchText, App.g_Category, App.g_ScanBarcode, App.g_Subcategory);
                lstKeywordItems = App.g_db.SearchItemsKeyword(App.g_SearchText);

                foreach (Item itemKeyword in lstKeywordItems)
                {
                    bool bFound = false;

                    foreach (Item item in lstItems)
                    {
                        if (item.ItemNo == itemKeyword.ItemNo)
                        {
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound)
                    {
                        lstItems.Add(itemKeyword);
                    }
                }
            }

            int iItems = 0;

            foreach (Item i in lstItems)
            {
                iItems += 1;

                i.IsLoggedIn = App.g_IsLoggedIn;

                i.IsStepperVisible = false;
                i.IsAddToOrderVisible = true;

                try
                {
                    if (i.LongDescription.Length > 0)
                    {
                        i.Description = i.LongDescription;
                    }
                }
                catch
                {
                }

                try
                {
                    if (i.QtyOrder > 0)
                    {
                        i.IsStepperVisible = true;
                        i.IsAddToOrderVisible = false;
                    }
                }
                catch (Exception ex)
                {
                    i.IsStepperVisible = false;
                    i.IsAddToOrderVisible = true;
                }

                i.IsQOHBlackVisible = false;
                i.IsQOHRedVisible = false;
                if (App.g_QOHDisplay == "Q")
                {
                    i.IsQOHVisible = true;
                    i.IsInStockVisible = false;
                    i.IsOutOfStockVisible = false;
                    if (i.QOH > 0)
                    {
                        i.IsQOHBlackVisible = true;
                    }
                    else
                    {
                        i.IsQOHRedVisible = true;
                    }
                }
                else if (App.g_QOHDisplay == "I")
                {
                    i.IsQOHVisible = false;
                    if (i.QOH > 0)
                    {
                        i.IsInStockVisible = true;
                        i.IsOutOfStockVisible = false;
                    }
                    else
                    {
                        i.IsInStockVisible = false;
                        i.IsOutOfStockVisible = true;
                    }
                }
                else
                {
                    i.IsQOHVisible = false;
                    i.IsInStockVisible = false;
                    i.IsOutOfStockVisible = false;
                }
                if (i.IsQOHVisible || i.IsInStockVisible || i.IsOutOfStockVisible)
                {
                    i.IsStockRowVisible = true;
                }
                else
                {
                    i.IsStockRowVisible = false;
                }

                if (App.g_BlockItemsNoQOH)
                {
                    if (i.QOH == 0)
                    {
                        i.IsStepperVisible = false;
                        i.IsAddToOrderVisible = false;
                    }
                }
            }

            ItemsListSearch.ItemsSource = lstItems;
            if (iItems == 0)
            {
                //await Shell.Current.DisplayAlertAsync("Profit Order", "No items found matching search criteria", "Ok");
                bool answer = await Shell.Current.DisplayAlertAsync(
                "Muswik Wholesale Grocers",
                "No items found in selected category. Do you want to search in all categories?",
                "Yes",
                "No");

                if (answer)
                {
                    // User tapped 'Yes' - Call your search method here
                    App.g_SearchText = Search.Text;
                    //App.g_SearchFromPage = "HomePage";
                    App.g_Category.Code = "";
                    App.g_Category.Description = "ALL CATEGORIES";

                    App.g_Subcategory.Code = "";
                    App.g_Subcategory.Description = "ALL SUBCATEGORIES";

                    await App.g_Shell.GoToItemSearch();
                }
                else
                {
                    // User tapped 'No' - Handle cancellation or do nothing
                }
            }
            //if (iItems == 0)
            //{
            //    await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "No items found matching search criteria", "Ok");
            //}
        }

        private void OnTappedClearCategory(object sender, EventArgs e)
        {
            App.g_Category.Code = "";
            App.g_Category.Description = "ALL CATEGORIES";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            RefreshList();
        }

        private async void OnTappedCategory(object sender, EventArgs e)
        {
            //App.g_Category.Code = "";
            //App.g_Category.Description = "ALL CATEGORIES";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            await App.g_Shell.GoToCategories();
        }

        private void OnTappedClearSubcategory(object sender, EventArgs e)
        {
            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            RefreshList();
        }

        private async void OnTappedSubcategory(object sender, EventArgs e)
        {
            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            if (App.g_Category.Code == "")
            {
               await App.g_Shell.GoToCategories();
            }
            else
            {
                //App.g_Shell.GoToSubcategories();
            }
        }

        private void InStockOnly_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            App.g_InStockOnly = InStockOnly.IsChecked;
            Search.Text = App.g_SearchText;
            RefreshList();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        void OnTappedSearch(object sender, EventArgs e)
        {
            App.g_SearchText = Search.Text;
            RefreshList();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            ImageOverlay.IsVisible = false;
            if (ItemsListSearch.SelectedItem != null)
            {
                ItemsListSearch.SelectedItem = null;
            }
        }


        private void ItemsListSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.CurrentSelection?.FirstOrDefault() as Item;
            if (selectedItem == null)
                return;
            ImageOverlay.IsVisible = true;
            FullImage.Source = selectedItem.ImageURL;
        }
    }
}

