using System;
using Microsoft.Kinect;

namespace Framework.Wraps
{
    public class ColorFrameWrap
    {
        public ColorCameraSettings ColorCameraSettings { get; set; }
        public FrameDescription FrameDescription { get; set; }
        public ColorImageFormat RawColorImageFormat { get; set; }
        public TimeSpan RelativeTime { get; set; }
        public byte[] RgbaPixelArray { get; set; }
    }
}