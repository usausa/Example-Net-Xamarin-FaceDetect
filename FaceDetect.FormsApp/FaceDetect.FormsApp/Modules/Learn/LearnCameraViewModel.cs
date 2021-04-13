namespace FaceDetect.FormsApp.Modules.Learn
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Smart.Navigation;

    public class LearnCameraViewModel : AppViewModelBase
    {
        public ICommand BackCommand { get; }

        public LearnCameraViewModel(
            ApplicationState applicationState)
            : base(applicationState)
        {
            BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.Menu);
        }
    }
}
