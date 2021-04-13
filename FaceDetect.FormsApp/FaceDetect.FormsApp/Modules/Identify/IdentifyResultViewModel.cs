namespace FaceDetect.FormsApp.Modules.Identify
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Smart.Navigation;

    public class IdentifyResultViewModel : AppViewModelBase
    {
        public ICommand BackCommand { get; }

        public IdentifyResultViewModel(
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
