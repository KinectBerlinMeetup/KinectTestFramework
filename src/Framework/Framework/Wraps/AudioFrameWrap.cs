using System;
using System.Collections.Generic;
using Microsoft.Kinect;

namespace Framework.Wraps
{
    public class AudioFrameWrap
    {
        public TimeSpan Duration { get; set; }
        public AudioBeam AudioBeam { get; set; }
        public TimeSpan RelativeTimeStart { get; set; }
        public IReadOnlyList<AudioBeamSubFrame> SubFrames { get; set; }
    }
}