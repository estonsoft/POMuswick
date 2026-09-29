using CommunityToolkit.Mvvm.ComponentModel;

namespace POMuswick.Services;

// Single app-wide loading indicator shared by every ViewModel via IAppServices,
// so the loading overlay reflects app state instead of a single page's ViewModel.
public partial class AppLoadingState : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string syncStatus = string.Empty;

    [ObservableProperty]
    private int progressPercentage;

    [ObservableProperty]
    private double progressValue;
}
