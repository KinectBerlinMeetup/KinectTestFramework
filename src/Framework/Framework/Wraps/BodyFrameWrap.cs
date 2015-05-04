using System;
using Microsoft.Kinect;

namespace Framework.Wraps
{
    public class BodyFrameWrap
    {
        public int BodyCount { get; set; }
        public BodyFrameSource BodyFrameSource { get; set; }
        public Vector4 FloorClipPlane { get; set; }
        public TimeSpan RelativeTime { get; set; }
        public Body[] Bodies { get; set; }
    }
}