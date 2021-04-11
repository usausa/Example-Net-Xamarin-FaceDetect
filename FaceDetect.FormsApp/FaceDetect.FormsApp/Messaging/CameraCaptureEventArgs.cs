namespace FaceDetect.FormsApp.Messaging
{
    using System;
    using System.Threading.Tasks;

    using Xamarin.Forms;

    public sealed class CameraCaptureEventArgs : EventArgs
    {
        public TaskCompletionSource<ImageSource> CompletionSource { get; } = new();
    }
}
