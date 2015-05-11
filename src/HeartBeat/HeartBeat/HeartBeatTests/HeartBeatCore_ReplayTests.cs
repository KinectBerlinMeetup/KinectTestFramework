using System;
using System.Collections.Generic;
using System.Threading;
using Framework;
using HeartBeat;
using Microsoft.Kinect;
using NUnit.Framework;

namespace HeartBeatTests
{
    public class HeartBeatCore_ReplayTests
    {
        private readonly string _filepath = @"D:\Repositories\KinectTestFramework\Files\TestFile_allStreams.xef";

        // TODO (KO): Make Testcases generic

        [TestFixtureSetUp]
        public void Setup()
        {
            KinectTesting.SetupPlayback(_filepath);
        }

        [Test]
        public void FrameArrivedTest_AudioStream()
        {
            var heartBeat = new HeartBeatCore(new TimeSpan(0, 0, 1), KinectStreams.Audio, KinectSensor.GetDefault());
            heartBeat.Start();
            KinectTesting.PlaySingleEvent(KinectStreams.Audio, new TimeSpan(0, 0, 1));
            Thread.Sleep(50);
            Assert.That(heartBeat.AudioStreamMonitor.FrameCount, Is.EqualTo(1));
        }

        //[Test]
        //public void FrameArrivedTest_BodyStream()
        //{
        //    var heartBeat = new HeartBeatCore(new TimeSpan(0, 0, 1), KinectStreams.Body, KinectSensor.GetDefault());
        //    heartBeat.Initialize();
        //    heartBeat.Start();
        //    KinectTesting.PlaySingleEvent(KinectStreams.Body, new TimeSpan(0, 0, 1));
        //    Thread.Sleep(50);
        //    Assert.That(heartBeat.BodyStreamMonitor.FrameCount, Is.EqualTo(1));
        //}

        [Test]
        public void FrameArrivedTest_DepthStream()
        {
            var heartBeat = new HeartBeatCore(new TimeSpan(0, 0, 1), KinectStreams.Depth, KinectSensor.GetDefault());
            heartBeat.Start();
            KinectTesting.PlaySingleEvent(KinectStreams.Depth, new TimeSpan(0, 0, 1));
            Thread.Sleep(50);
            Assert.That(heartBeat.DepthStreamMonitor.FrameCount, Is.EqualTo(1));
        }

        [Test]
        public void FrameArrivedTest_IrStream()
        {
            var heartBeat = new HeartBeatCore(new TimeSpan(0, 0, 1), KinectStreams.Ir, KinectSensor.GetDefault());
            heartBeat.Start();
            KinectTesting.PlaySingleEvent(KinectStreams.Ir, new TimeSpan(0, 0, 1));
            Thread.Sleep(50);
            Assert.That(heartBeat.IrStreamMonitor.FrameCount, Is.EqualTo(1));
        }

        [Test]
        public void FrameArrivedTest_ColorStream()
        {
            var heartBeat = new HeartBeatCore(new TimeSpan(0, 0, 1), KinectStreams.Color, KinectSensor.GetDefault());
            heartBeat.Start();
            KinectTesting.PlaySingleEvent(KinectStreams.Color, new TimeSpan(0, 0, 1));
            Thread.Sleep(50);
            Assert.That(heartBeat.ColorStreamMonitor.FrameCount, Is.EqualTo(1));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            KinectTesting.ExterminatePlayback();
        }
    }
}