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
        public List<DateTimeOffset> FrameTimeStamps { get; private set; }
        public KinectStreams StreamType { get; private set; }

        public DateTimeOffset LastFrameTimeStamp
        {
            get
            {
                if (FrameTimeStamps.Count > 0)
                {
                    return FrameTimeStamps.Last();
                }
                return bootTime;

            }
        }

        public StreamMonitor(KinectStreams streamType)
        {
            StreamType = streamType;
            FrameTimeStamps = new List<DateTimeOffset>();
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
            FrameTimeStamps.Add(bootTime + relativeFrameTime);
        }
    }
}