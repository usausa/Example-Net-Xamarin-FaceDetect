namespace FaceDetect.FormsApp.Messaging
{
    using Xamarin.Forms;

    public class CaptureResult
    {
        public ImageSource Image { get; }

        public double Rotation { get; }

        public CaptureResult(ImageSource image, double rotation)
        {
            Image = image;
            Rotation = rotation;
        }
    }
}
