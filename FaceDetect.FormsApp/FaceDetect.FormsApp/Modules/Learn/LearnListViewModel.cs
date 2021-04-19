namespace FaceDetect.FormsApp.Modules.Learn
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using FaceDetect.FormsApp.Components.Dialog;
    using FaceDetect.FormsApp.Models.Entity;
    using FaceDetect.FormsApp.Services;

    using Smart.Collections.Generic;
    using Smart.Forms.ViewModels;
    using Smart.Navigation;

    public class LearnListViewModel : AppViewModelBase
    {
        private readonly IApplicationDialog dialog;

        private readonly DataService dataService;

        public ObservableCollection<PersonEntity> Items { get; } = new();

        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand LearnCommand { get; }

        public ICommand BackCommand { get; }
        public ICommand AddCommand { get; }

        public LearnListViewModel(
            ApplicationState applicationState,
            IApplicationDialog dialog,
            DataService dataService)
            : base(applicationState)
        {
            this.dialog = dialog;
            this.dataService = dataService;

            DeleteCommand = MakeAsyncCommand<PersonEntity>(DeleteAsync);
            EditCommand = MakeAsyncCommand<PersonEntity>(x => Navigator.ForwardAsync(ViewId.LearnEdit, Parameters.MakePerson(x)));
            LearnCommand = MakeAsyncCommand<PersonEntity>(x => Navigator.ForwardAsync(ViewId.LearnCamera, Parameters.MakePerson(x)));
            BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
            AddCommand = MakeAsyncCommand(() => Navigator.ForwardAsync(ViewId.LearnEdit));
        }

        public override async void OnNavigatedTo(INavigationContext context)
        {
            if (!context.Attribute.IsRestore())
            {
                await Navigator.PostActionAsync(() => BusyState.Using(async () =>
                    Items.AddRange(await dataService.QueryPersonListAsync())));
            }
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.Menu);
        }

        private async Task DeleteAsync(PersonEntity entity)
        {
            if (!await dialog.Confirm($"Delete {entity.Name} ?"))
            {
                return;
            }

            await dataService.DeletePerson(entity);

            Items.Remove(entity);
        }
    }
}
