namespace FaceDetect.FormsApp.Modules.Detect
{
    using System.Windows.Input;

    using Smart.Navigation;

    public class DetectResultViewModel : AppViewModelBase
    {
        public ICommand ForwardCommand { get; }

        public DetectResultViewModel(
            ApplicationState applicationState)
            : base(applicationState)
        {
            ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));
        }
    }
}
