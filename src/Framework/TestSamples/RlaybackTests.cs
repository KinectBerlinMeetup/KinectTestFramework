using System;
using System.Diagnostics;
using System.Threading;
using Framework;
using Microsoft.Kinect;
using NUnit.Framework;

namespace TestSamples
{
    public class RlaybackTests
    {
        private readonly string _filepath = @"D:\Repositories\KinectTestFramework\Files\TestFile_allStreams.xef";

        /// <summary>
        ///     This methods should be called only once per class, otherwise the tests would be slowed down because everythime the
        ///     Client has to be created
        /// </summary>
        [TestFixtureSetUp]
        public void Setup()
        {
            KinectTesting.SetupPlayback(_filepath);
        }

        [Test]
        public void TestCase()
        {
            KinectTesting.PlayAllWithoutPauses();
        }

        [Test]
        public void TestCase2()
        {
            KinectTesting.Play();
        }

        [Test]
        public void TestCase3()
        {
            KinectTesting.Play(PlaybackTiming.Fast);
        }

        [Test]
        public void TestCase4()
        {
            KinectTesting.Play(PlaybackTiming.Fast, new TimeSpan(0, 0, 1), new TimeSpan(0, 0, 4));
        }

        [Test]
        public void TestSingelFrameBody()
        {
            var framesRecieved = 0;
            var reader = KinectSensor.GetDefault().BodyFrameSource.OpenReader();
            reader.FrameArrived += delegate { framesRecieved++; };

            KinectTesting.PlaySingleEvent(PlaybackStreams.Body, new TimeSpan(0, 0, 1));

            Assert.That(framesRecieved, Is.EqualTo(1));
        }

        [Test]
        public void TestSingelFrameAudio()
        {
            var framesRecieved = 0;
            var reader = KinectSensor.GetDefault().AudioSource.OpenReader();
            reader.FrameArrived += delegate { framesRecieved++; };

            KinectTesting.PlaySingleEvent(PlaybackStreams.Audio, new TimeSpan(0, 0, 1));

            Assert.That(framesRecieved, Is.EqualTo(1));
        }

        [Test]
        public void TestSingelFrameIr()
        {
            var framesRecieved = 0;
            var reader = KinectSensor.GetDefault().InfraredFrameSource.OpenReader();
            reader.FrameArrived += delegate { framesRecieved++; };

            KinectTesting.PlaySingleEvent(PlaybackStreams.Ir, new TimeSpan(0, 0, 1));

            Assert.That(framesRecieved, Is.EqualTo(1));

            Thread.Sleep(1000); // Note: Added to check behaviour change when running all tests. 
        }

        [Test]
        public void TestSingelFrameColor()
        {
            var framesRecieved = 0;
            var reader = KinectSensor.GetDefault().ColorFrameSource.OpenReader();
            reader.FrameArrived += delegate { framesRecieved++; };

            KinectTesting.PlaySingleEvent(PlaybackStreams.Color, new TimeSpan(0, 0, 1));

            Assert.That(framesRecieved, Is.EqualTo(1));
        }

        [Test]
        public void TestSingelFrameDepth()
        {
            var framesRecieved = 0;
            var reader = KinectSensor.GetDefault().DepthFrameSource.OpenReader();
            reader.FrameArrived += delegate { framesRecieved++; };

            KinectTesting.PlaySingleEvent(PlaybackStreams.Depth, new TimeSpan(0, 0, 1));

            Assert.That(framesRecieved, Is.EqualTo(1));
        }

        [Test]
        public void TestStepOnceSpeed()
        {
            var framesRecieved = 0;
            var reader = KinectSensor.GetDefault().DepthFrameSource.OpenReader();
            reader.FrameArrived += delegate { framesRecieved++; };
            var watch = new Stopwatch();
            watch.Start();

            KinectTesting.PlayNumberOfEvents(PlaybackStreams.Depth, new TimeSpan(0, 0, 1),100);

            watch.Stop();
            Assert.That(framesRecieved, Is.EqualTo(100));
        }

        /// <summary>
        ///     This methods should be called only once per class, otherwise the tests would be slowed down
        /// </summary>
        [TestFixtureTearDown]
        public void TearDown()
        {
            KinectTesting.ExterminatePlayback();
        }
    }
}