namespace FaceDetect.FormsApp.Controls
{
    using System;

    using SkiaSharp;
    using SkiaSharp.Views.Forms;

    using Xamarin.Forms;

    public class FaceCanvasView : SKCanvasView
    {
        private SKBitmap? image;

        public static readonly BindableProperty FaceRectangleProperty =
            BindableProperty.CreateAttached(
                nameof(FaceRectangle),
                typeof(System.Drawing.Rectangle),
                typeof(FaceCanvasView),
                null,
                propertyChanged: HandleFaceRectanglePropertyChanged);

        public System.Drawing.Rectangle? FaceRectangle
        {
            get => (System.Drawing.Rectangle?)GetValue(FaceRectangleProperty);
            set => SetValue(FaceRectangleProperty, value);
        }

        private static void HandleFaceRectanglePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((FaceCanvasView)bindable).InvalidateSurface();
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var info = e.Info;
            var canvas = e.Surface.Canvas;

            // Fill background
            using var backgroundPaint = new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.White };

            canvas.DrawRect(info.Rect, backgroundPaint);

            if (image is null)
            {
                return;
            }

            var scale = Math.Min(info.Width / (float)image.Width, info.Height / (float)image.Height);

            var scaleWidth = scale * image.Width;
            var scaleHeight = scale * image.Height;

            var left = (info.Width - scaleWidth) / 2;
            var top = (info.Height - scaleHeight) / 2;

            // Draw bitmap
            canvas.DrawBitmap(image, new SKRect(left, top, left + scaleWidth, top + scaleHeight));

            // Draw box
            if (FaceRectangle is not null)
            {
                var scaledBoxLeft = left + (scale * FaceRectangle.Value.Left);
                var scaledBoxWidth = scale * FaceRectangle.Value.Width;
                var scaledBoxTop = top + (scale * FaceRectangle.Value.Top);
                var scaledBoxHeight = scale * FaceRectangle.Value.Height;
                using var path = CreateBoxPath(scaledBoxLeft, scaledBoxTop, scaledBoxWidth, scaledBoxHeight);
                using var strokePaint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.OrangeRed,
                    StrokeWidth = 5,
                    PathEffect = SKPathEffect.CreateDash(new[] { 5f, 5f }, 5f)
                };

                canvas.DrawPath(path, strokePaint);
            }
        }

        private static SKPath CreateBoxPath(float left, float top, float width, float height)
        {
            var path = new SKPath();
            path.MoveTo(left, top);
            path.LineTo(left + width, top);
            path.LineTo(left + width, top + height);
            path.LineTo(left, top + height);
            path.LineTo(left, top);
            return path;
        }

        public void Load(byte[] data)
        {
            image = SKBitmap.Decode(data);
            InvalidateSurface();
        }
    }
}
