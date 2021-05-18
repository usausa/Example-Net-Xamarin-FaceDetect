namespace FaceDetect.FormsApp.Modules.Learn
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using FaceDetect.FormsApp.Components.Dialog;
    using FaceDetect.FormsApp.Models.Entity;
    using FaceDetect.FormsApp.Services;
    using FaceDetect.FormsApp.Usecase;

    using Smart.Collections.Generic;
    using Smart.Forms.ViewModels;
    using Smart.Navigation;
    using Smart.Navigation.Plugins.Scope;

    public class LearnListViewModel : AppViewModelBase
    {
        private readonly IApplicationDialog dialog;

        private readonly DataService dataService;

        private readonly FaceDetectUsecase faceDetectUsecase;

        [Scope]
        [AllowNull]
        public LearnContext Context { get; set; }

        public ObservableCollection<PersonEntity> Items { get; } = new();

        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand LearnCommand { get; }

        public ICommand BackCommand { get; }
        public ICommand AddCommand { get; }

        public LearnListViewModel(
            ApplicationState applicationState,
            IApplicationDialog dialog,
            DataService dataService,
            FaceDetectUsecase faceDetectUsecase)
            : base(applicationState)
        {
            this.dialog = dialog;
            this.dataService = dataService;
            this.faceDetectUsecase = faceDetectUsecase;

            DeleteCommand = MakeAsyncCommand<PersonEntity>(DeleteAsync);
            EditCommand = MakeAsyncCommand<PersonEntity>(async x =>
            {
                Context.Person = x;
                await Navigator.ForwardAsync(ViewId.LearnEdit);
            });
            LearnCommand = MakeAsyncCommand<PersonEntity>(async x =>
            {
                Context.Person = x;
                await Navigator.ForwardAsync(ViewId.LearnCamera);
            });
            BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
            AddCommand = MakeAsyncCommand(async () =>
            {
                Context.Person = null;
                await Navigator.ForwardAsync(ViewId.LearnEdit);
            });
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

            await faceDetectUsecase.DeletePersonAsync(entity.Id);

            Items.Remove(entity);
        }
    }
}
