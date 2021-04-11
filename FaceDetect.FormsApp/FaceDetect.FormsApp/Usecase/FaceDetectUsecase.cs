namespace FaceDetect.FormsApp.Usecase
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using FaceDetect.FormsApp.Models.Result;

    using Microsoft.Azure.CognitiveServices.Vision.Face;
    using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
    using SkiaSharp;

    public class FaceDetectUsecase
    {
        private const string Key = "()";
        private const string EndPoint = "()";

        private const double Rotate90 = 90d;
        private const double Rotate180 = 180d;
        private const double Rotate270 = 270d;

        private const int NormalizeSize = 1024;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator", Justification = "Ignore")]
        public async ValueTask<byte[]> NormalizeImage(byte[] data, double rotation)
        {
            return await Task.Run(() =>
            {
                var source = SKBitmap.Decode(data);

                var factor = Math.Max((double)source.Width / NormalizeSize, (double)source.Height / NormalizeSize);
                var newWidth = (int)(source.Width / factor);
                var newHeight = (int)(source.Height / factor);

                var destination = source.Resize(new SKSizeI(newWidth, newHeight), SKFilterQuality.Medium);
                if (rotation == Rotate90)
                {
                    var rotated = new SKBitmap(destination.Height, destination.Width);
                    using var surface = new SKCanvas(rotated);
                    surface.Translate(rotated.Width, 0);
                    surface.RotateDegrees(90f);
                    surface.DrawBitmap(destination, 0, 0);

                    destination = rotated;
                }
                else if (rotation == Rotate180)
                {
                    using var surface = new SKCanvas(destination);
                    surface.RotateDegrees(180f, (float)destination.Width / 2, (float)destination.Height / 2);
                    surface.DrawBitmap(destination.Copy(), 0, 0);
                }
                else if (rotation == Rotate270)
                {
                    var rotated = new SKBitmap(destination.Height, destination.Width);
                    using var surface = new SKCanvas(rotated);
                    surface.Translate(0, rotated.Height);
                    surface.RotateDegrees(270f);
                    surface.DrawBitmap(destination, 0, 0);

                    destination = rotated;
                }

                using var ms = new MemoryStream();
                destination.Encode(ms, SKEncodedImageFormat.Jpeg, 85);
                return ms.ToArray();
            });
        }

        public async ValueTask<DetectResult> DetectAsync(byte[] image)
        {
            using var client = new FaceClient(new ApiKeyServiceClientCredentials(Key)) { Endpoint = EndPoint };
            await using var stream = new MemoryStream(image);

            // TODO
            var detectedFaces = await client.Face.DetectWithStreamWithHttpMessagesAsync(
                stream,
                returnFaceAttributes: new List<FaceAttributeType>
                {
                    FaceAttributeType.Accessories,
                    FaceAttributeType.Age,
                    FaceAttributeType.Blur,
                    FaceAttributeType.Emotion,
                    FaceAttributeType.Exposure,
                    FaceAttributeType.FacialHair,
                    FaceAttributeType.Gender,
                    FaceAttributeType.Glasses,
                    FaceAttributeType.Hair,
                    FaceAttributeType.HeadPose,
                    FaceAttributeType.Makeup,
                    FaceAttributeType.Noise,
                    FaceAttributeType.Occlusion,
                    FaceAttributeType.Smile
                },
                detectionModel: DetectionModel.Detection01,
                recognitionModel: RecognitionModel.Recognition03);

            return detectedFaces.Body.Count > 0 ? new DetectResult() : null;
        }
    }
}
