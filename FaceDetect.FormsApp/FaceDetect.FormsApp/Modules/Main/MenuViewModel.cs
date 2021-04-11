namespace FaceDetect.FormsApp.Modules.Main
{
    using System;
    using System.Windows.Input;

    using FaceDetect.FormsApp.State;

    using Smart.Navigation;

    public class MenuViewModel : AppViewModelBase
    {
        public ICommand ForwardCommand { get; }
        public ICommand DetectForwardCommand { get; }

        public MenuViewModel(
            ApplicationState applicationState,
            Configuration configuration)
            : base(applicationState)
        {
            var canUseApi = !String.IsNullOrEmpty(configuration.ApiKey) &&
                            !String.IsNullOrEmpty(configuration.ApiEndPoint);

            ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));
            DetectForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x), _ => canUseApi);
        }
    }
}
