namespace TimeTracker.Maui.Services;

public class AlertService : IAlertService
{
    // Async calls work like standard currentShell.DisplayAlert. These will run on the same thread

    /// <summary>
    /// Displays an alert box to the user
    /// </summary>
    /// <param name="title">Title of the alert</param>
    /// <param name="message">Alert body</param>
    /// <param name="cancel">Cancel button text</param>
    /// <returns>App alert dialog</returns>
    public Task ShowAlertAsync(string title, string message, string cancel = "OK")
    {
        return Application.Current?.MainPage?.DisplayAlert(title, message, cancel);
    }

    /// <summary>
    /// Displays an alert box to the user asking to continue operation or to cancel
    /// </summary>
    /// <param name="title">Title of the alert</param>
    /// <param name="message">Alert body</param>
    /// <param name="accept">Accept button text</param>
    /// <param name="cancel">Cancel button text</param>
    /// <returns>The user's choice as a Boolean value</returns>
    public Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Continue", string cancel = "Cancel")
    {
        return Application.Current?.MainPage?.DisplayAlert(title, message, accept, cancel);
    }

    /// <summary>
    /// Displays an alert box to the user with multiple options to select
    /// </summary>
    /// <param name="title">Title of the alert</param>
    /// <param name="cancel">Cancel button text</param>
    /// <param name="destruction">Destruct button text</param>
    /// <param name="buttons">Labels for inputted options</param>
    /// <returns>The user's selection as a String value</returns>
    public Task<string> ShowActionSheetAsync(string title, string cancel, string destruction, params string[] buttons)
    {
        return Application.Current?.MainPage?.DisplayActionSheet(title, cancel, destruction, buttons);
    }

    // These methods return before message is displayed

    /// <summary>
    /// Displays an alert box to the user
    /// </summary>
    /// <param name="title">Title of the alert</param>
    /// <param name="message">Alert body</param>
    /// <param name="cancel">Cancel button text</param>
    /// <returns>App alert dialog</returns>
    public void ShowAlert(string title, string message, string cancel = "OK")
    {
        async void Action() => await ShowAlertAsync(title, message, cancel);

        Application.Current?.MainPage?.Dispatcher.Dispatch(Action);
    }

    /// <summary>
    /// Displays an alert box to the user asking to continue operation or to cancel
    /// </summary>
    /// <param name="title">Title of the alert</param>
    /// <param name="message">Alert body</param>
    /// <param name="callback">Action to perform afterwards</param>
    /// <param name="accept">Accept button text</param>
    /// <param name="cancel">Cancel button text</param>
    /// <returns>The user's choice as a Boolean value</returns>
    public void ShowConfirmation(string title, string message, Action<bool> callback, string accept = "Continue", string cancel = "Cancel")
    {
        async void Action()
        {
            var answer = await ShowConfirmationAsync(title, message, accept, cancel);
            callback(answer);
        }

        Application.Current?.MainPage?.Dispatcher.Dispatch(Action);
    }

    /// <summary>
    /// Displays an alert box to the user with multiple options to select
    /// </summary>
    /// <param name="title">Title of the alert</param>
    /// <param name="callback">Action to perform afterwards</param>
    /// <param name="cancel">Cancel button text</param>
    /// <param name="destruction">Destruct button text</param>
    /// <param name="buttons">Labels for inputted options</param>
    /// <returns>The user's selection as a String value</returns>
    public void ShowActionSheet(string title, Action<string> callback, string cancel, string destruction, params string[] buttons)
    {
        async void Action()
        {
            var selection = await ShowActionSheetAsync(title, cancel, destruction, buttons);
            callback(selection);
        }

        Application.Current?.MainPage?.Dispatcher.Dispatch(Action);
    }
}