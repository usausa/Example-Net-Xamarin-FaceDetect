namespace FaceDetect.FormsApp.Models.Result
{
    using System.Drawing;

    public class DetectResult
    {
        public double? Age { get; set; }

        public string? Gender { get; set; }

        public string PredominantEmotion { get; set; } = default!;

        public string Hair { get; set; } = default!;

        public string[] Accessories { get; set; } = default!;

        public string? Glasses { get; set; }

        public double? Smile { get; set; }

        public Rectangle FaceRectangle { get; set; }
    }
}
