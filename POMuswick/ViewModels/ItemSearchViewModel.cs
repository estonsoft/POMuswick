using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FFImageLoading.Maui;
using POMuswick.Models;
using POMuswick.Services;
using POMuswick.UIModels;

namespace POMuswick.ViewModels
{
    public partial class ItemSearchViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IItemService _itemService;
        private readonly ISettingService _settingService;
        private readonly ICatNSubCatService _catNSubCatService;
        private AppSettings _appSettings;
        private CatNSubCatParameter catNSubCatParameter;
        [ObservableProperty]
        public List<UIItems> _lstItems;
        [ObservableProperty]
        public List<Item> _lstKeywordItems;
        [ObservableProperty]
        public string _categoryCode;
        [ObservableProperty]
        public string _subCategoryCode;
        [ObservableProperty]
        public bool _inStockOnly;
        [ObservableProperty]
        public string _searchText;

        [ObservableProperty]
        public CachedImage _selectedImage;

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
            if (query.TryGetValue("SearchText", out var searchValue))
            {
                SearchText = (string)searchValue;
            }
            else if (query.TryGetValue("CatNSubCatParameter", out var CategoryValue))
            {
                catNSubCatParameter = (CatNSubCatParameter)CategoryValue;
            }
        }

        public async void RefreshList()
        {
            _appSettings = await _settingService.LoadSetting();
            Category category = catNSubCatParameter.Category;
            Subcategory subcategory = catNSubCatParameter.Subcategory;

            if (category.Description == "NEW ITEMS")
            {
                var itemResult = await _itemService.FetchNewItemAsync(InStockOnly);
                List<UIItems> uiList = itemResult.items
                .Select(x => x.ToUI())
                .ToList();
                LstItems = uiList;
            }
            else
            {
                var items = await _itemService.SearchItemsAsync(InStockOnly,SearchText, category, _appSettings.scanBarcode, subcategory);
                List<UIItems> uiList = items
                    .Select(x => x.ToUI())
                    .ToList();
                    LstItems = uiList;
                // ✅ Only merge keyword items if NO specific category is selected
                if (string.IsNullOrEmpty(category.Code) || category.Description == "ALL CATEGORIES")
                {
                    LstKeywordItems = await _itemService.SearchItemsKeyword(SearchText,InStockOnly);

                    foreach (Item itemKeyword in LstKeywordItems)
                    {
                        bool bFound = false;

                        foreach (UIItems item in LstItems)
                        {
                            if (item.ItemNo == itemKeyword.ItemNo)
                            {
                                bFound = true;
                                break;
                            }
                        }

                        if (!bFound)
                        {
                            UIItems item = itemKeyword.ToUI();
                            LstItems.Add(item);
                        }
                    }
                }
            }

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
                catch (Exception ex)
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

            RefreshList();
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
        private async Task RefreshListCommand()
        {
            RefreshList();
        }

        [RelayCommand]
        private async Task ShowImageAsync(CachedImage cachedImage)
        {
            SelectedImage = cachedImage;
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

            _itemService.UpdateItemQtySet(item.ItemNo, 1);

            item.QtyOrder++;

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

            _itemService.UpdateItemQtySet(item.ItemNo, -1);

            item.QtyOrder--;

            if (item.QtyOrder == 0)
            {
                item.IsStepperVisible = false;
                item.IsAddToOrderVisible = true;
            }
            await Task.CompletedTask;
        }
    }
}

