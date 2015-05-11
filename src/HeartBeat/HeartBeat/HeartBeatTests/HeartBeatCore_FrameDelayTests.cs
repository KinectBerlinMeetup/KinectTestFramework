using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Framework;
using HeartBeat;
using Microsoft.Kinect;
using NUnit.Framework;

namespace HeartBeatTests
{
    internal class HeartBeatCore_FrameDelayTests
    {
        private readonly string _filepath = @"D:\Repositories\KinectTestFramework\Files\TestFile_allStreams.xef";

        [SetUp]
        public void Setup()
        {
            KinectTesting.SetupPlayback(_filepath);
        }

        [Test]
        public void FrameDelayTest_EventsInvoked()
        {
            var frameDelayCount = 0;
            var heartBeat = new HeartBeatCore(new TimeSpan(0, 0, 0, 1), KinectStreams.Color, KinectSensor.GetDefault());
            heartBeat.FrameDelayEventHandler += delegate { frameDelayCount++; };
            heartBeat.Start();

            KinectTesting.PlayTillMarker(new TimeSpan(0,0,0,1));

            Thread.Sleep(new TimeSpan(0, 0, 0, 10));

            Assert.That(frameDelayCount, Is.GreaterThan(0));
        }

        [Test]
        public void FrameDelayTest_EventTypesCorrect()
        {
            var frameDelayEventArgs = new List<FrameDelayEventArgs>();
            var heartBeat = new HeartBeatCore(new TimeSpan(0, 0, 0, 1), KinectStreams.Color, KinectSensor.GetDefault());
            heartBeat.FrameDelayEventHandler += delegate(object sender, FrameDelayEventArgs e) { frameDelayEventArgs.Add(e); };
            heartBeat.Start();

            KinectTesting.PlayTillMarker(new TimeSpan(0, 0, 0, 1));

            Thread.Sleep(new TimeSpan(0, 0, 0, 10));

            Assert.That(frameDelayEventArgs.All(e => e.Type == KinectStreams.Color));
        }

        [TearDown]
        public void TearDown()
        {
            KinectTesting.ExterminatePlayback();
        }
    }
}