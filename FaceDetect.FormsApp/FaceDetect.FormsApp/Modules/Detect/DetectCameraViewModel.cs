namespace FaceDetect.FormsApp.Modules.Detect;

using System.Threading.Tasks;
using System.Windows.Input;

using FaceDetect.FormsApp.Messaging;

using Smart.Navigation;

public class DetectCameraViewModel : AppViewModelBase
{
    public CameraCaptureRequest CaptureRequest { get; } = new();

    public ICommand BackCommand { get; }
    public ICommand DetectCommand { get; }

    public DetectCameraViewModel(
        ApplicationState applicationState)
        : base(applicationState)
    {
        BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
        DetectCommand = MakeAsyncCommand(DetectAsync);
    }

    protected override Task OnNotifyBackAsync()
    {
        return Navigator.ForwardAsync(ViewId.Menu);
    }

    private async Task DetectAsync()
    {
        var image = await CaptureRequest.CaptureAsync();
        if (image is not null)
        {
            await Navigator.ForwardAsync(ViewId.DetectResult, Parameters.MakeImage(image));
        }
    }
}
