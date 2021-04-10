namespace FaceDetect.FormsApp.Modules.Detect
{
    using System.Windows.Input;
    using Smart.Navigation;

    public class DetectCameraViewModel : AppViewModelBase
    {
        public ICommand ForwardCommand { get; }

        public DetectCameraViewModel(
            ApplicationState applicationState)
            : base(applicationState)
        {
            ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));
        }
    }
}
