namespace FaceDetect.FormsApp.Modules.Identify
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using FaceDetect.FormsApp.Messaging;

    using Smart.Navigation;

    public class IdentifyCameraViewModel : AppViewModelBase
    {
        public CameraCaptureRequest CaptureRequest { get; } = new();

        public ICommand BackCommand { get; }
        public ICommand IdentifyCommand { get; }

        public IdentifyCameraViewModel(
            ApplicationState applicationState)
            : base(applicationState)
        {
            BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
            IdentifyCommand = MakeAsyncCommand(IdentifyAsync);
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.Menu);
        }

        private async Task IdentifyAsync()
        {
            var image = await CaptureRequest.CaptureAsync();
            if (image is not null)
            {
                await Navigator.ForwardAsync(ViewId.IdentifyResult, Parameters.MakeImage(image));
            }
        }
    }
}
