namespace FaceDetect.FormsApp.Modules.Learn
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using FaceDetect.FormsApp.Messaging;

    using Smart.Navigation;
    using Smart.Navigation.Plugins.Scope;

    public class LearnCameraViewModel : AppViewModelBase
    {
        [Scope]
        [AllowNull]
        public LearnContext Context { get; set; }

        public CameraCaptureRequest CaptureRequest { get; } = new();

        public ICommand BackCommand { get; }
        public ICommand ShotCommand { get; }

        public LearnCameraViewModel(
            ApplicationState applicationState)
            : base(applicationState)
        {
            BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
            ShotCommand = MakeAsyncCommand(ShotAsync);
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.LearnList);
        }

        private async Task ShotAsync()
        {
            var image = await CaptureRequest.CaptureAsync();
            if (image is not null)
            {
                await Navigator.ForwardAsync(ViewId.LearnConfirm, Parameters.MakeImage(image));
            }
        }
    }
}
