using POMuswick;

public interface INavigationService
{
    Task GoToAsync(string route);
    Task GoToAsync(string route, ShellNavigationQueryParameters keyValuePairs);
    Task GoToRootAsync(string route);
    Task GoBackAsync();
    Task PopModalAsync();
    Task HideCustomerMenu();
    Task HideMyAccountMenu();
    Task ShowCustomerMenu();
    Task ShowMyAccountMenu();
    Task ConfirmLogoutAsync();
}
public class NavigationService : INavigationService
{
    public async Task GoToAsync(string route)
    {
        await Shell.Current.GoToAsync(route);
    }

    public async Task GoToRootAsync(string route)
    {
        await Shell.Current.GoToAsync($"//{route}");
    }

    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    public async Task PopModalAsync()
    {
        if (Shell.Current.Navigation.ModalStack.Count > 0)
            await Shell.Current.Navigation.PopModalAsync();
    }

    public async Task HideCustomerMenu()
    {
        if (Shell.Current is AppShell shell)
        {
            shell.HideCustomerMenu();
        }
    }
    public async Task HideMyAccountMenu()
    {
        if (Shell.Current is AppShell shell)
        {
            shell.HideMyAccountMenu();
        }
    }
    public async Task ShowCustomerMenu()
    {
        if (Shell.Current is AppShell shell)
        {
            shell.ShowCustomerMenu();
        }
    }
    public async Task ShowMyAccountMenu()
    {
        if (Shell.Current is AppShell shell)
        {
            shell.ShowMyAccountMenu();
        }
    }

    public async Task GoToAsync(string route, ShellNavigationQueryParameters parameter)
    {
        await Shell.Current.GoToAsync(route, parameter);
    }

    public async Task ConfirmLogoutAsync()
    {
        if (Shell.Current is AppShell shell)
        {
            await shell.Logout();
        }
    }
}