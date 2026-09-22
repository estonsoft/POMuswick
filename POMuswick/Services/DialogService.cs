public interface IDialogService
{
    Task<bool> ConfirmAsync(string title, string message, string accept = "Yes", string cancel = "No");
    Task AlertAsync(string title, string message, string cancel = "OK");
}
public class DialogService : IDialogService
{
    public async Task<bool> ConfirmAsync(string title, string message, string accept = "Yes", string cancel = "No")
    {
        return await Shell.Current.DisplayAlertAsync(title, message, accept, cancel);
    }

    public async Task AlertAsync(string title, string message, string cancel = "OK")
    {
        await Shell.Current.DisplayAlertAsync(title, message, cancel);
    }
}