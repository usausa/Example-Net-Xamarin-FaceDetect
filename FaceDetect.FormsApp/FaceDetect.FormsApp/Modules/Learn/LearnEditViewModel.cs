namespace FaceDetect.FormsApp.Modules.Learn
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using FaceDetect.FormsApp.Models.Entity;
    using FaceDetect.FormsApp.Services;

    using Smart.ComponentModel;
    using Smart.Navigation;
    using Smart.Navigation.Plugins.Scope;

    public class LearnEditViewModel : AppViewModelBase
    {
        private readonly DataService dataService;

        [Scope]
        public LearnContext Context { get; set; }

        public NotificationValue<string> Name { get; } = new();

        public ICommand BackCommand { get; }
        public ICommand UpdateCommand { get; }

        public LearnEditViewModel(
            ApplicationState applicationState,
            DataService dataService)
            : base(applicationState)
        {
            this.dataService = dataService;

            BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
            UpdateCommand = MakeAsyncCommand(UpdateAsync, () => !String.IsNullOrEmpty(Name.Value)).Observe(Name);
        }

        public override void OnNavigatedTo(INavigationContext context)
        {
            if (!context.Attribute.IsRestore())
            {
                if (Context.Person is not null)
                {
                    Name.Value = Context.Person.Name;
                }
            }
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.LearnList);
        }

        private async Task UpdateAsync()
        {
            if (Context.Person is null)
            {
                await dataService.InsertPerson(new PersonEntity { Id = Guid.NewGuid(),  Name = Name.Value });
            }
            else
            {
                Context.Person.Name = Name.Value;
                await dataService.UpdatePerson(Context.Person);
            }

            await Navigator.ForwardAsync(ViewId.LearnList);
        }
    }
}
