using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using Window = Microsoft.Maui.Controls.Window;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TimeTracker.Maui.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		this.InitializeComponent();

        SwitchHandler.Mapper.AppendToMapping("Custom", (h, v) =>
        {
            // removes On/Off label next to Switch control on Windows
            h.PlatformView.OffContent = string.Empty;
            h.PlatformView.OnContent = string.Empty;
            h.PlatformView.MinWidth = 0;
        });
    }

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

