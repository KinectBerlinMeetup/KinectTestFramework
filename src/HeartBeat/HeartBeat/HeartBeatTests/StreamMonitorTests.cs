using System;
using Framework;
using HeartBeat;
using NUnit.Framework;

namespace HeartBeatTests
{
    public class StreamMonitorTests
    {
        [Test]
        public void ConstrutctorTest()
        {
            var monitor = new StreamMonitor(KinectStreams.All);
            Assert.That(monitor.StreamType, Is.EqualTo(KinectStreams.All));
        }

        [Test]
        public void UpdateTest_FrameCount()
        {
            var monitor = new StreamMonitor(KinectStreams.All);
            monitor.Update(new TimeSpan());
            Assert.That(monitor.FrameCount, Is.EqualTo(1));
        }
    }
}