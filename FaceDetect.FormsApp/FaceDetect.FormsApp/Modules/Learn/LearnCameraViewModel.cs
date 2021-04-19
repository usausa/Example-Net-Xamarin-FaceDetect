namespace FaceDetect.FormsApp.Modules.Learn
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using FaceDetect.FormsApp.Components.Dialog;
    using FaceDetect.FormsApp.Messaging;
    using FaceDetect.FormsApp.Models.Entity;
    using FaceDetect.FormsApp.Usecase;

    using Smart.Navigation;

    public class LearnCameraViewModel : AppViewModelBase
    {
        private readonly IApplicationDialog dialog;

        private readonly FaceDetectUsecase faceDetectUsecase;

        private PersonEntity entity;

        public CameraCaptureRequest CaptureRequest { get; } = new();

        public ICommand BackCommand { get; }
        public ICommand LearnCommand { get; }

        public LearnCameraViewModel(
            ApplicationState applicationState,
            IApplicationDialog dialog,
            FaceDetectUsecase faceDetectUsecase)
            : base(applicationState)
        {
            this.dialog = dialog;
            this.faceDetectUsecase = faceDetectUsecase;

            BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
            LearnCommand = MakeAsyncCommand(LearnAsync);
        }

        public override void OnNavigatedTo(INavigationContext context)
        {
            if (!context.Attribute.IsRestore())
            {
                entity = context.Parameter.GetPerson();
            }
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.LearnList);
        }

        private async Task LearnAsync()
        {
            var image = await CaptureRequest.CaptureAsync();
            if (image is null)
            {
                return;
            }

            // TODO
            if ((await faceDetectUsecase.LearnAsync(entity.Id, image)) &&
                !await dialog.Confirm("Re learn ?"))
            {
                await Navigator.ForwardAsync(ViewId.LearnList);
            }
        }
    }
}
