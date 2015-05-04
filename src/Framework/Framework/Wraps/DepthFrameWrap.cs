using System;
using Microsoft.Kinect;

namespace Framework.Wraps
{
    public class DepthFrameWrap
    {
        public FrameDescription FrameDescription { get; set; }
        public TimeSpan RelativeTime { get; set; }
        public ushort[] DepthPixelArray { get; set; }
    }
}