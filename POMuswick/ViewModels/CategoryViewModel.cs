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
    public List<Category> _categories = new();
    [ObservableProperty]
    private bool _isLoadingCategories;

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
        IsLoadingCategories = true;
        try
        {
            Categories = await _catNSubCatService.GetCategories();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Category load failed: {ex}");
        }
        finally
        {
            IsLoadingCategories = false;
        }
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