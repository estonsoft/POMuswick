using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POMuswick.Services;

namespace POMuswick.ViewModels;

public partial class SubCategoryViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly ICatNSubCatService _catNSubCatService;
    private Subcategory _selectedItem;

    [ObservableProperty]
    public List<Subcategory> _subcategories;
    private CatNSubCatParameter parameter;

    public SubCategoryViewModel(IAppServices appServices) : base(appServices)
    {
        Title = "Sub Categories";
        _dialogService = appServices._dialogService;
        _navigationService = appServices._navigationService;
        _catNSubCatService = appServices._catNSubCatService;
    }

    public async override Task OnAppearingAsync()
    {
        await base.OnAppearingAsync();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("CatNSubCatParameter", out var searchValue))
        {
            parameter = (CatNSubCatParameter)searchValue;
        }
        await LoadSubCategoriesAsync(parameter);
    }
    private async Task LoadSubCategoriesAsync(CatNSubCatParameter parameter)
    {
        IsBusy = true;
        Subcategories = await _catNSubCatService.GetSubCategories(parameter.Category.Code);

        if (Subcategories == null || Subcategories.Count == 0)
        {
            await _dialogService.AlertAsync("Error", "Unable to load subcategories.", "OK");
        }
        IsBusy = false;
    }

    [RelayCommand]
    private async Task CategoriesSelectedAsync()
    {
        if (Subcategories == null || Subcategories.Count == 0)
        {
            await _dialogService.AlertAsync("Error", "Unable to load subcategories.", "OK");
        }
        IsBusy = false;
    }

    [RelayCommand]
    private async Task SubcategoriesSelectedAsync(Subcategory selectedSubcategory)
    {
        if (selectedSubcategory == null)
            return;

        parameter.Subcategory = selectedSubcategory;

        await _navigationService.GoToAsync(AppRoutes.SubCategories, new ShellNavigationQueryParameters
        {
            { "CatNSubCatParameter", parameter }
        });
    }
}
