namespace FaceDetect.FormsApp.Modules.Learn
{
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using FaceDetect.FormsApp.Components.Dialog;
    using FaceDetect.FormsApp.Usecase;

    using Smart.ComponentModel;
    using Smart.Navigation;
    using Smart.Navigation.Plugins.Scope;

    using Xamarin.Forms;

    public class LearnConfirmViewModel : AppViewModelBase
    {
        private readonly IApplicationDialog dialog;

        private readonly FaceDetectUsecase faceDetectUsecase;

        private byte[] image;

        [Scope]
        public NotificationValue<LearnContext> Context { get; } = new();

        public NotificationValue<ImageSource> Image { get; } = new();

        public ICommand BackCommand { get; }
        public ICommand LearnCommand { get; }

        public LearnConfirmViewModel(
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

        public override void OnNavigatingTo(INavigationContext context)
        {
            if (!context.Attribute.IsRestore())
            {
                image = context.Parameter.GetImage();
                Image.Value = ImageSource.FromStream(() => new MemoryStream(image));
            }
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.LearnCamera);
        }

        private async Task LearnAsync()
        {
            using (dialog.Loading("Learning"))
            {
                await faceDetectUsecase.LearnAsync(Context.Value.Person.Id.ToString(), image);
            }

            if (await dialog.Confirm("Add more picture to learn ?"))
            {
                await Navigator.ForwardAsync(ViewId.LearnCamera);
            }
            else
            {
                await Navigator.ForwardAsync(ViewId.LearnList);
            }
        }
    }
}
