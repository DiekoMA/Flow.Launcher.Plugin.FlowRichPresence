namespace Flow.Launcher.Plugin.FlowRichPresence.Views;

public partial class SettingsView : UserControl
{
    public SettingsView(SettingsViewModel viewModel)
    {
        InitializeComponent();
        this.DataContext = viewModel;
    }
}