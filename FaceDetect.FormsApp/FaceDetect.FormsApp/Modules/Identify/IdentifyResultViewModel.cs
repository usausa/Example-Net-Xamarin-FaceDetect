namespace FaceDetect.FormsApp.Modules.Identify
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using FaceDetect.FormsApp.Messaging;
    using FaceDetect.FormsApp.Models.Entity;
    using FaceDetect.FormsApp.Models.Result;
    using FaceDetect.FormsApp.Services;
    using FaceDetect.FormsApp.Usecase;

    using Smart.ComponentModel;
    using Smart.Forms.ViewModels;
    using Smart.Navigation;

    public class IdentifyResultViewModel : AppViewModelBase
    {
        private readonly DataService dataService;

        private readonly FaceDetectUsecase faceDetectUsecase;

        public NotificationValue<IdentifyResult> Result { get; } = new();
        public NotificationValue<PersonEntity> Person { get; } = new();

        public LoadImageRequest LoadImageRequest { get; } = new();

        public ICommand RetryCommand { get; }
        public ICommand CloseCommand { get; }

        public IdentifyResultViewModel(
            ApplicationState applicationState,
            DataService dataService,
            FaceDetectUsecase faceDetectUsecase)
            : base(applicationState)
        {
            this.dataService = dataService;
            this.faceDetectUsecase = faceDetectUsecase;

            RetryCommand = MakeAsyncCommand(OnNotifyBackAsync);
            CloseCommand = MakeAsyncCommand(() => Navigator.ForwardAsync(ViewId.Menu));
        }

        public override async void OnNavigatingTo(INavigationContext context)
        {
            if (!context.Attribute.IsRestore())
            {
                var image = context.Parameter.GetImage();
                LoadImageRequest.Load(image);

                await Navigator.PostActionAsync(() => BusyState.UsingAsync(async () =>
                {
                    Result.Value = await faceDetectUsecase.IdentifyAsync(image);
                    if (Result.Value is not null)
                    {
                        Person.Value = await dataService.QueryPersonAsync(Result.Value.Id);
                    }
                }));
            }
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.IdentifyCamera);
        }
    }
}
