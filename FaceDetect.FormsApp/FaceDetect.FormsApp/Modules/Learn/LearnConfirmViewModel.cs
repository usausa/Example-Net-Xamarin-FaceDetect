namespace FaceDetect.FormsApp.Modules.Learn;

using System.Threading.Tasks;
using System.Windows.Input;

using FaceDetect.FormsApp.Components.Dialog;
using FaceDetect.FormsApp.Messaging;
using FaceDetect.FormsApp.Usecase;

using Smart.ComponentModel;
using Smart.Navigation;
using Smart.Navigation.Plugins.Scope;

public class LearnConfirmViewModel : AppViewModelBase
{
    private readonly IApplicationDialog dialog;

    private readonly FaceDetectUsecase faceDetectUsecase;

    private byte[] image = default!;

    [Scope]
    public NotificationValue<LearnContext> Context { get; } = new();

    public LoadImageRequest LoadImageRequest { get; } = new();

    public ICommand BackCommand { get; }
    public ICommand LearnCommand { get; }

    public LearnConfirmViewModel(
        ApplicationState applicationState,
        IApplicationDialog dialog,
        FaceDetectUsecase faceDetectUsecase)
        : base(applicationState)
    {
        this.dialog = dialog;
        this.faceDetectUsecase = faceDetectUsecase;

        BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
        LearnCommand = MakeAsyncCommand(LearnAsync);
    }

    public override void OnNavigatingTo(INavigationContext context)
    {
        if (!context.Attribute.IsRestore())
        {
            image = context.Parameter.GetImage();
            LoadImageRequest.Load(image);
        }
    }

    protected override Task OnNotifyBackAsync()
    {
        return Navigator.ForwardAsync(ViewId.LearnCamera);
    }

    private async Task LearnAsync()
    {
        if (!await faceDetectUsecase.LearnAsync(Context.Value.Person!.Id, image))
        {
            return;
        }

        if (await dialog.Confirm("Add more picture to learn ?"))
        {
            await Navigator.ForwardAsync(ViewId.LearnCamera);
        }
        else
        {
            await Navigator.ForwardAsync(ViewId.LearnList);
        }
    }
}
