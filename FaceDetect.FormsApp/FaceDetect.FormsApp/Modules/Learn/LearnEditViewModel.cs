namespace FaceDetect.FormsApp.Modules.Learn
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using FaceDetect.FormsApp.Models.Entity;
    using FaceDetect.FormsApp.Services;

    using Smart.ComponentModel;
    using Smart.Navigation;

    public class LearnEditViewModel : AppViewModelBase
    {
        private readonly DataService dataService;

        private PersonEntity entity;

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
                entity = context.Parameter.GetPersonOrDefault();
                if (entity is not null)
                {
                    Name.Value = entity.Name;
                }
            }
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.LearnList);
        }

        private async Task UpdateAsync()
        {
            if (entity is null)
            {
                await dataService.InsertPerson(new PersonEntity { Id = Guid.NewGuid(),  Name = Name.Value });
            }
            else
            {
                entity.Name = Name.Value;
                await dataService.UpdatePerson(entity);
            }

            await Navigator.ForwardAsync(ViewId.LearnList);
        }
    }
}
