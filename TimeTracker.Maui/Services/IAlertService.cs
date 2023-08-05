namespace TimeTracker.Maui.Services;

public interface IAlertService
{
    Task ShowAlertAsync(string title, string message, string cancel = "OK");
    
    Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Continue", string cancel = "Cancel");

    Task<string> ShowActionSheetAsync(string title, string cancel, string destruction, params string[] buttons);

    void ShowAlert(string title, string message, string cancel = "OK");

    void ShowConfirmation(string title, string message, Action<bool> callback, string accept = "Continue", string cancel = "Cancel");

    void ShowActionSheet(string title, Action<string> callback, string cancel, string destruction, params string[] buttons);
}