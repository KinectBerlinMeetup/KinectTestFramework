using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Framework;

namespace HeartBeat
{
    public class StreamMonitor
    {
        private readonly DateTimeOffset bootTime;
        public uint FrameCount { get; private set; }
        public List<DateTimeOffset> FrameTimeStemps { get; private set; }
        public KinectStreams StreamType { get; private set; }

        public DateTimeOffset LastFrameTimestemp
        {
            get { return FrameTimeStemps.Last(); }
        }

        public StreamMonitor(KinectStreams streamType)
        {
            StreamType = streamType;
            FrameTimeStemps = new List<DateTimeOffset>();
            FrameCount = 0;

            using (var uptime = new PerformanceCounter("System", "System Up Time"))
            {
                uptime.NextValue(); //Call this an extra time before reading its value
                bootTime = DateTimeOffset.Now - TimeSpan.FromSeconds(uptime.NextValue());
            }
        }

        public void Update(TimeSpan relativeFrameTime)
        {
            FrameCount++;
            FrameTimeStemps.Add(bootTime + relativeFrameTime);
        }
    }
}