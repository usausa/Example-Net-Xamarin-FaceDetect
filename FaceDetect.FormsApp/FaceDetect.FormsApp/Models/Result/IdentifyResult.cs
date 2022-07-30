namespace FaceDetect.FormsApp.Models.Result;

using System;
using System.Drawing;

public class IdentifyResult
{
    public Guid Id { get; set; }

    public double Confidence { get; set; }

    public Rectangle FaceRectangle { get; set; }
}
