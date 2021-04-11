namespace FaceDetect.FormsApp.Modules.Detect
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Smart.ComponentModel;
    using Smart.Forms.ViewModels;
    using Smart.Navigation;

    using Xamarin.Forms;

    public class DetectResultViewModel : AppViewModelBase
    {
        public NotificationValue<ImageSource> Image { get; } = new();
        public NotificationValue<double> Rotation { get; } = new();

        public ICommand RetryCommand { get; }
        public ICommand CloseCommand { get; }

        public DetectResultViewModel(
            ApplicationState applicationState)
            : base(applicationState)
        {
            RetryCommand = MakeAsyncCommand(OnNotifyBackAsync);
            CloseCommand = MakeAsyncCommand(() => Navigator.ForwardAsync(ViewId.Menu));
        }

        public override async void OnNavigatingTo(INavigationContext context)
        {
            if (!context.Attribute.IsRestore())
            {
                Image.Value = context.Parameter.GetImage();
                Rotation.Value = context.Parameter.GetRotation();

                await Navigator.PostActionAsync(() => BusyState.UsingAsync(async () =>
                {
                    // TODO
                    await Task.Delay(0);
                }));
            }
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.DetectCamera);
        }
    }
}
