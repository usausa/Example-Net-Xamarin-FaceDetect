namespace FaceDetect.FormsApp.Messaging
{
    using Xamarin.Forms;

    public class CaptureResult
    {
        public ImageSource Image { get; }

        public byte[] ImageData { get; }

        public double Rotation { get; }

        public CaptureResult(ImageSource image, byte[] imageData, double rotation)
        {
            Image = image;
            ImageData = imageData;
            Rotation = rotation;
        }
    }
}
