using TimeTracker.Maui.ViewModels;

namespace TimeTracker.Maui.Pages;

public partial class SettingsPage
{
	public SettingsPage(SettingsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}