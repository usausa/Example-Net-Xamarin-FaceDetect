namespace FaceDetect.FormsApp.Modules.Learn
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Smart.Navigation;

    public class LearnEditViewModel : AppViewModelBase
    {
        public ICommand BackCommand { get; }

        public LearnEditViewModel(
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
