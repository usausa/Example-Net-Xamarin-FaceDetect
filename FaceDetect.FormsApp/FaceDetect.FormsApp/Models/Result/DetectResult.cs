namespace FaceDetect.FormsApp.Models.Result
{
    using System.Drawing;

    public class DetectResult
    {
        public double? Age { get; set; }

        public string Gender { get; set; }

        public string PredominantEmotion { get; set; }

        public string Hair { get; set; }

        public string[] Accessories { get; set; }

        public string Glasses { get; set; }

        public double? Smile { get; set; }

        public Rectangle FaceRectangle { get; set; }
    }
}
