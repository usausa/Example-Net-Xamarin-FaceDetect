namespace FaceDetect.FormsApp.Modules
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using FaceDetect.FormsApp.Shell;

    using Smart.Forms.ViewModels;
    using Smart.Navigation;

    public class AppViewModelBase : ViewModelBase, INavigatorAware, INavigationEventSupport, INotifySupportAsync<ShellEvent>
    {
        [AllowNull]
        public INavigator Navigator { get; set; }

        public ApplicationState ApplicationState { get; }

        protected AppViewModelBase(ApplicationState applicationState)
            : base(applicationState)
        {
            ApplicationState = applicationState;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            System.Diagnostics.Debug.WriteLine($"{GetType()} is Disposed");
        }

        public virtual void OnNavigatingFrom(INavigationContext context)
        {
        }

        public virtual void OnNavigatingTo(INavigationContext context)
        {
        }

        public virtual void OnNavigatedTo(INavigationContext context)
        {
        }

        public Task NavigatorNotifyAsync(ShellEvent parameter)
        {
            return parameter switch
            {
                ShellEvent.Back => OnNotifyBackAsync(),
                _ => Task.CompletedTask
            };
        }

        protected virtual Task OnNotifyBackAsync()
        {
            return Task.CompletedTask;
        }
    }
}
