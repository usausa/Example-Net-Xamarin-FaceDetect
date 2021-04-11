namespace FaceDetect.FormsApp.Messaging
{
    using System;
    using System.Threading.Tasks;

    using Smart.Forms.Messaging;

    using Xamarin.Forms;

    public sealed class CameraCaptureRequest : IEventRequest<CameraCaptureEventArgs>
    {
        public event EventHandler<CameraCaptureEventArgs> Requested;

        public Task<ImageSource> CaptureAsync()
        {
            var args = new CameraCaptureEventArgs();
            Requested?.Invoke(this, args);
            return args.CompletionSource.Task;
        }
    }
}
