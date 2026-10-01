using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using POMuswick.Models;
using POMuswick.Services;
using POMuswick.UIModels;

namespace POMuswick.ViewModels
{
    public partial class ItemSearchViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IItemService _itemService;
        private readonly ISettingService _settingService;
        private readonly ICatNSubCatService _catNSubCatService;
        private AppSettings _appSettings;
        private CatNSubCatParameter catNSubCatParameter = CreateDefaultCategoryParameter();
        [ObservableProperty]
        public ObservableCollection<UIItems> _lstItems = new();
        [ObservableProperty]
        public List<Item> _lstKeywordItems;
        [ObservableProperty]
        public string _categoryCode;
        [ObservableProperty]
        public string _subCategoryCode;
        [ObservableProperty]
        public bool _inStockOnly;
        [ObservableProperty]
        public string _searchText = "";

        [ObservableProperty]
        public ImageSource? _selectedImage;
        [ObservableProperty]
        public bool _isImagePreviewVisible;

        public ItemSearchViewModel(IAppServices appServices) : base(appServices)
        {
            _dialogService = appServices._dialogService;
            _navigationService = appServices._navigationService;
            _itemService = appServices._itemService;
            _settingService = appServices._settingService;
            _catNSubCatService = appServices._catNSubCatService;
        }

        public async override Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("CatNSubCatParameter", out var categoryParameter) && categoryParameter is CatNSubCatParameter parameter)
            {
                catNSubCatParameter = parameter;
            }
            else if (query.TryGetValue("CategoryCode", out var categoryCode) && categoryCode is CatNSubCatParameter alternateParameter)
            {
                catNSubCatParameter = alternateParameter;
            }
            else if (query.ContainsKey("NEW ITEMS"))
            {
                catNSubCatParameter = new CatNSubCatParameter
                {
                    Category = new Category { Code = "NEW ITEMS", Description = "NEW ITEMS" },
                    Subcategory = new Subcategory { Code = "", Description = "ALL SUBCATEGORIES" }
                };
            }

            if (query.TryGetValue("SearchText", out var searchValue))
                SearchText = (string)searchValue;

            UpdatePageTitle();
            await RefreshList();
        }

        public async Task RefreshList()
        {
            _appSettings = await _settingService.LoadSetting();
            Category category = catNSubCatParameter.Category;
            Subcategory subcategory = catNSubCatParameter.Subcategory;
            var uiList = new List<UIItems>();

            if (category.Description == "NEW ITEMS")
            {
                var itemResult = await _itemService.FetchNewItemAsync(InStockOnly);
                uiList = itemResult.items
                .Select(x => x.ToUI())
                .ToList();
            }
            else
            {
                var items = await _itemService.SearchItemsAsync(InStockOnly, SearchText, category, _appSettings.scanBarcode, subcategory);
                uiList = items
                    .Select(x => x.ToUI())
                    .ToList();
                // ✅ Only merge keyword items if NO specific category is selected
                if (string.IsNullOrEmpty(category.Code) || category.Description == "ALL CATEGORIES")
                {
                    LstKeywordItems = await _itemService.SearchItemsKeyword(SearchText, InStockOnly);

                    foreach (Item itemKeyword in LstKeywordItems)
                    {
                        if (!uiList.Any(item => item.ItemNo == itemKeyword.ItemNo))
                        {
                            UIItems item = itemKeyword.ToUI();
                            uiList.Add(item);
                        }
                    }
                }
            }

            LstItems = new ObservableCollection<UIItems>(uiList);

            int iItems = 0;

            foreach (UIItems i in LstItems)
            {
                iItems += 1;

                i.IsLoggedIn = _appSettings.IsLoggedIn;

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
                catch
                {
                    i.IsStepperVisible = false;
                    i.IsAddToOrderVisible = true;
                }

                i.IsQOHBlackVisible = false;
                i.IsQOHRedVisible = false;

                if (_appSettings.QOHDisplay == "Q")
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
                else if (_appSettings.QOHDisplay == "I")
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

                if (_appSettings.BlockItemsNoQOH)
                {
                    if (i.QOH == 0)
                    {
                        i.IsStepperVisible = false;
                        i.IsAddToOrderVisible = false;
                    }
                }
            }

            if (iItems == 0 && category.Description != "ALL CATEGORIES")
            {
                bool answer = await Shell.Current.DisplayAlertAsync(
                    "Muswick Wholesale Grocers",
                    "No items found in selected category. Do you want to search in all categories?",
                    "Yes",
                    "No");

                if (answer)
                {
                    Category categoryAll = new Category
                    {
                        Code = "",
                        Description = "ALL CATEGORIES"
                    };
                    Subcategory subcategoryAll = new Subcategory
                    {
                        Code = "",
                        Description = "ALL SUBCATEGORIES"
                    };
                    catNSubCatParameter = new CatNSubCatParameter
                    {
                        Category = categoryAll,
                        Subcategory = subcategoryAll
                    };

                    await _navigationService.GoToAsync(AppRoutes.ItemSearch, new ShellNavigationQueryParameters
                    {
                        { "SearchText", SearchText },
                        { "CategoryCode", catNSubCatParameter }
                    });
                }
                else
                {
                    // User tapped 'No' - do nothing
                }
            }
            else if (iItems == 0)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Muswick Wholesale Grocers",
                    "No items found in selected category. Please modify your search.",
                    "Cancel");
            }
        }

        private void UpdatePageTitle()
        {
            var category = catNSubCatParameter.Category;
            var subcategory = catNSubCatParameter.Subcategory;

            if (category.Description == "NEW ITEMS")
                Title = "New Items";
            else if (!string.IsNullOrEmpty(subcategory.Code))
                Title = subcategory.Description;
            else if (!string.IsNullOrEmpty(category.Code) && category.Description != "ALL CATEGORIES")
                Title = category.Description;
            else
                Title = string.IsNullOrWhiteSpace(SearchText) ? "Search Products" : "Search Results";
        }

        private static CatNSubCatParameter CreateDefaultCategoryParameter()
        {
            return new CatNSubCatParameter
            {
                Category = new Category { Code = "", Description = "ALL CATEGORIES" },
                Subcategory = new Subcategory { Code = "", Description = "ALL SUBCATEGORIES" }
            };
        }

        [RelayCommand]
        private async Task ClearCategoryAsync()
        {
            Category categoryAll = new Category
            {
                Code = "",
                Description = "ALL CATEGORIES"
            };
            Subcategory subcategoryAll = new Subcategory
            {
                Code = "",
                Description = "ALL SUBCATEGORIES"
            };
            catNSubCatParameter = new CatNSubCatParameter
            {
                Category = categoryAll,
                Subcategory = subcategoryAll
            };

            await RefreshList();
        }

        [RelayCommand]
        private async Task CategoryAsync()
        {
            Subcategory subcategoryAll = new Subcategory
            {
                Code = "",
                Description = "ALL SUBCATEGORIES"
            };
            catNSubCatParameter = new CatNSubCatParameter
            {
                Subcategory = subcategoryAll
            };

            await _navigationService.GoToAsync(AppRoutes.Categories, new ShellNavigationQueryParameters
            {
                { "CategoryCode", catNSubCatParameter }
            });
        }

        [RelayCommand]
        private async Task SubcategoryAsync()
        {
            Subcategory subcategoryAll = new Subcategory
            {
                Code = "",
                Description = "ALL SUBCATEGORIES"
            };
            catNSubCatParameter = new CatNSubCatParameter
            {
                Subcategory = subcategoryAll
            };

            if (catNSubCatParameter.Category.Code == "")
            {
                await _navigationService.GoToAsync(AppRoutes.Categories, new ShellNavigationQueryParameters
                {
                    { "CategoryCode", catNSubCatParameter }
                });
            }
        }

        [RelayCommand]
        private async Task RefreshListAsync()
        {
            await RefreshList();
        }

        [RelayCommand]
        private void ShowImage(ImageSource? imageSource)
        {
            SelectedImage = imageSource;
            IsImagePreviewVisible = imageSource != null;
        }

        [RelayCommand]
        private void CloseImage()
        {
            IsImagePreviewVisible = false;
        }

        [RelayCommand]
        private async Task IncreaseQtyAsync(UIItems item)
        {
            if (item == null)
                return;

            if (item.QtyOrder >= 999)
                return;

            if (item.MaxOrderQty > 0 &&
                item.QtyOrder >= item.MaxOrderQty)
                return;

            var newQuantity = item.QtyOrder + 1;
            await _itemService.UpdateItemQtySet(item.ItemNo, newQuantity);

            item.QtyOrder = newQuantity;

            item.IsStepperVisible = true;
            item.IsAddToOrderVisible = false;
            await Task.CompletedTask;
        }

        [RelayCommand]
        private async Task DecreaseQtyAsync(UIItems item)
        {
            if (item == null)
                return;

            if (item.QtyOrder <= 0)
                return;

            var newQuantity = item.QtyOrder - 1;
            await _itemService.UpdateItemQtySet(item.ItemNo, newQuantity);

            item.QtyOrder = newQuantity;

            if (item.QtyOrder == 0)
            {
                item.IsStepperVisible = false;
                item.IsAddToOrderVisible = true;
            }
            await Task.CompletedTask;
        }
    }
}

