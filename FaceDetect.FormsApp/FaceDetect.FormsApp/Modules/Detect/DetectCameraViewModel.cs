namespace FaceDetect.FormsApp.Modules.Detect
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using FaceDetect.FormsApp.Messaging;
    using FaceDetect.FormsApp.Usecase;

    using Smart.Navigation;

    public class DetectCameraViewModel : AppViewModelBase
    {
        private readonly FaceDetectUsecase faceDetectUsecase;

        public CameraCaptureRequest CaptureRequest { get; } = new();

        public ICommand BackCommand { get; }
        public ICommand DetectCommand { get; }

        public DetectCameraViewModel(
            ApplicationState applicationState,
            FaceDetectUsecase faceDetectUsecase)
            : base(applicationState)
        {
            this.faceDetectUsecase = faceDetectUsecase;

            BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
            DetectCommand = MakeAsyncCommand(DetectAsync);
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.Menu);
        }

        private async Task DetectAsync()
        {
            var result = await CaptureRequest.CaptureAsync();
            if (result is not null)
            {
                var image = await faceDetectUsecase.NormalizeImage(result.ImageData, result.Rotation);
                await Navigator.ForwardAsync(ViewId.DetectResult, Parameters.MakeImage(image));
            }
        }
    }
}
