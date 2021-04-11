namespace FaceDetect.FormsApp.Modules.Setting
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Smart.ComponentModel;
    using Smart.Navigation;

    public class SettingViewModel : AppViewModelBase
    {
        public NotificationValue<bool> CanScan { get; } = new();

        public ICommand ScanCommand { get; }

        public ICommand BackCommand { get; }

        public SettingViewModel(
            ApplicationState applicationState)
            : base(applicationState)
        {
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
            // TODO
            Debug.WriteLine(text);
            await Task.Delay(1000);
        }
    }
}
