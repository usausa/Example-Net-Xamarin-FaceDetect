namespace FaceDetect.FormsApp.Modules.Detect
{
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using FaceDetect.FormsApp.Components.Dialog;
    using FaceDetect.FormsApp.Usecase;

    using Smart.ComponentModel;
    using Smart.Forms.ViewModels;
    using Smart.Navigation;

    using Xamarin.Forms;

    public class DetectResultViewModel : AppViewModelBase
    {
        private readonly IApplicationDialog dialog;

        private readonly FaceDetectUsecase faceDetectUsecase;

        public NotificationValue<ImageSource> Image { get; } = new();

        public ICommand RetryCommand { get; }
        public ICommand CloseCommand { get; }

        public DetectResultViewModel(
            ApplicationState applicationState,
            IApplicationDialog dialog,
            FaceDetectUsecase faceDetectUsecase)
            : base(applicationState)
        {
            this.dialog = dialog;
            this.faceDetectUsecase = faceDetectUsecase;

            RetryCommand = MakeAsyncCommand(OnNotifyBackAsync);
            CloseCommand = MakeAsyncCommand(() => Navigator.ForwardAsync(ViewId.Menu));
        }

        public override async void OnNavigatingTo(INavigationContext context)
        {
            if (!context.Attribute.IsRestore())
            {
                var image = context.Parameter.GetImage();
                Image.Value = ImageSource.FromStream(() => new MemoryStream(image));

                await Navigator.PostActionAsync(() => BusyState.UsingAsync(async () =>
                {
                    using (dialog.Loading("Detecting..."))
                    {
                        // TODO
                        await faceDetectUsecase.DetectAsync(image);
                    }
                }));
            }
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.DetectCamera);
        }
    }
}
