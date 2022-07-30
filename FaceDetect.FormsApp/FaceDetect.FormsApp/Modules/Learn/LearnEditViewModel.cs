namespace FaceDetect.FormsApp.Modules.Learn;

using System;
using System.Threading.Tasks;
using System.Windows.Input;

using FaceDetect.FormsApp.Usecase;

using Smart.ComponentModel;
using Smart.Navigation;
using Smart.Navigation.Plugins.Scope;

public class LearnEditViewModel : AppViewModelBase
{
    private readonly FaceDetectUsecase faceDetectUsecase;

    [Scope]
    public LearnContext Context { get; set; } = default!;

    public NotificationValue<string> Name { get; } = new();

    public ICommand BackCommand { get; }
    public ICommand UpdateCommand { get; }

    public LearnEditViewModel(
        ApplicationState applicationState,
        FaceDetectUsecase faceDetectUsecase)
        : base(applicationState)
    {
        this.faceDetectUsecase = faceDetectUsecase;

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
            await faceDetectUsecase.CreatePersonAsync(Name.Value);
        }
        else
        {
            Context.Person.Name = Name.Value;
            await faceDetectUsecase.UpdatePersonAsync(Context.Person);
        }

        await Navigator.ForwardAsync(ViewId.LearnList);
    }
}
