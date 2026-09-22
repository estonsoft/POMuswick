using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Services;

namespace POMuswick.ViewModels;

public partial class CategoryViewModel : BaseViewModel
{
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly ICatNSubCatService _catNSubCatService;
    private Category _selectedItem;

    [ObservableProperty]
    public List<Category> _categories;

    public CategoryViewModel(IAppServices appServices) : base(appServices)
    {
        Title = "Product Categories";
        _dialogService = appServices._dialogService;
        _navigationService = appServices._navigationService;
        _catNSubCatService = appServices._catNSubCatService;
    }

    public async override Task OnAppearingAsync()
    {
        await base.OnAppearingAsync();
        await LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        IsBusy = true;
        Categories = await _catNSubCatService.GetCategories();

        if (Categories == null || Categories.Count == 0)
        {
            await _dialogService.AlertAsync("Error", "Unable to load categories.", "OK");
        }
        IsBusy = false;
    }

    [RelayCommand]
    private async Task CategoriesSelectedAsync(Category selectedCategory)
    {
        if (selectedCategory == null)
            return;

        CatNSubCatParameter parameter = new CatNSubCatParameter
        {
            Category = selectedCategory,
            Subcategory = new Subcategory { Code = "", Description = "ALL SUBCATEGORIES" }
        };

        await _navigationService.GoToAsync(AppRoutes.ItemSearch, new ShellNavigationQueryParameters
        {
            { "CatNSubCatParameter", parameter }
        });
    }
}