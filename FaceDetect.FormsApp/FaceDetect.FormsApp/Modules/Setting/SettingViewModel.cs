namespace FaceDetect.FormsApp.Modules.Setting;

using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

using FaceDetect.FormsApp.Components.Dialog;
using FaceDetect.FormsApp.State;

using Smart.ComponentModel;
using Smart.IO;
using Smart.Navigation;

public class SettingViewModel : AppViewModelBase
{
    private readonly Configuration configuration;

    private readonly IApplicationDialog dialog;

    public NotificationValue<bool> CanScan { get; } = new();

    public ICommand ScanCommand { get; }

    public ICommand BackCommand { get; }

    public SettingViewModel(
        ApplicationState applicationState,
        Configuration configuration,
        IApplicationDialog dialog)
        : base(applicationState)
    {
        this.configuration = configuration;
        this.dialog = dialog;

        ScanCommand = MakeAsyncCommand<string>(ExecuteScan);

        BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
    }

    public override void OnNavigatingFrom(INavigationContext context)
    {
        CanScan.Value = false;
    }

    public override void OnNavigatingTo(INavigationContext context)
    {
        CanScan.Value = true;
    }

    protected override Task OnNotifyBackAsync()
    {
        return Navigator.ForwardAsync(ViewId.Menu);
    }

    private async Task ExecuteScan(string text)
    {
        var updated = false;

        using var reader = new StringReader(text);
        foreach (var line in reader.ReadLines())
        {
            var values = line.Split('=');
            if (values.Length != 2)
            {
                continue;
            }

            if (values[0] == "EndPoint")
            {
                configuration.ApiEndPoint = values[1];
                updated = true;
            }
            else if (values[0] == "Key")
            {
                configuration.ApiKey = values[1];
                updated = true;
            }
        }

        if (updated)
        {
            await dialog.Information("Configuration updated.");

            await Navigator.ForwardAsync(ViewId.Menu);
        }
    }
}
