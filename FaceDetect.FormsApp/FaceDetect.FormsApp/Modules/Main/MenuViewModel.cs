namespace FaceDetect.FormsApp.Modules.Main
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using FaceDetect.FormsApp.Components.Dialog;
    using FaceDetect.FormsApp.State;
    using FaceDetect.FormsApp.Usecase;

    using Smart.Navigation;

    public class MenuViewModel : AppViewModelBase
    {
        private readonly IApplicationDialog dialog;

        private readonly FaceDetectUsecase faceDetectUsecase;

        public ICommand ForwardCommand { get; }
        public ICommand DetectForwardCommand { get; }
        public ICommand ResetCommand { get; }

        public MenuViewModel(
            ApplicationState applicationState,
            Configuration configuration,
            IApplicationDialog dialog,
            FaceDetectUsecase faceDetectUsecase)
            : base(applicationState)
        {
            this.dialog = dialog;
            this.faceDetectUsecase = faceDetectUsecase;

            var canUseApi = !String.IsNullOrEmpty(configuration.ApiKey) &&
                            !String.IsNullOrEmpty(configuration.ApiEndPoint);

            ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));
            DetectForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x), _ => canUseApi);
            ResetCommand = MakeAsyncCommand(ResetAsync, () => canUseApi);
        }

        private async Task ResetAsync()
        {
            if (!await dialog.Confirm("Reset all learning data ?"))
            {
                return;
            }

            await faceDetectUsecase.ResetAsync();
        }
    }
}
