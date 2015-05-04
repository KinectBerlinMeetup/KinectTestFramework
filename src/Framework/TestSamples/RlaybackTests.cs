using System;
using Framework;
using Microsoft.Kinect;
using NUnit.Framework;

namespace TestSamples
{
    public class RlaybackTests
    {
        private readonly string _filepath = @"D:\Repositories\KinectTestFramework\Files\TestFile_allStreams.xef";
        public int FramesRecieved { get; set; }

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
            FramesRecieved = 0;
            var reader = KinectSensor.GetDefault().BodyFrameSource.OpenReader();
            reader.FrameArrived += delegate { FramesRecieved++; };


            //KinectTesting.Play(PlaybackTiming.Fast);
            KinectTesting.PlaySingleEvent(PlaybackStreams.Body, new TimeSpan(0, 0, 1));

            Assert.That(FramesRecieved, Is.GreaterThan(0));
        }

        [Test]
        public void TestSingelFrameAudio()
        {
            FramesRecieved = 0;
            var reader = KinectSensor.GetDefault().AudioSource.OpenReader();
            reader.FrameArrived += delegate { FramesRecieved++; };


            //KinectTesting.Play(PlaybackTiming.Fast);
            KinectTesting.PlaySingleEvent(PlaybackStreams.Audio, new TimeSpan(0, 0, 1));

            Assert.That(FramesRecieved, Is.GreaterThan(0));
        }

        [Test]
        public void TestSingelFrameIr()
        {
            FramesRecieved = 0;
            var reader = KinectSensor.GetDefault().InfraredFrameSource.OpenReader();
            reader.FrameArrived += delegate { FramesRecieved++; };


            //KinectTesting.Play(PlaybackTiming.Fast);
            KinectTesting.PlaySingleEvent(PlaybackStreams.Ir, new TimeSpan(0, 0, 1));

            Assert.That(FramesRecieved, Is.GreaterThan(0));
        }

        [Test]
        public void TestSingelFrameColor()
        {
            FramesRecieved = 0;
            var reader = KinectSensor.GetDefault().ColorFrameSource.OpenReader();
            reader.FrameArrived += delegate { FramesRecieved++; };


            //KinectTesting.Play(PlaybackTiming.Fast);
            KinectTesting.PlaySingleEvent(PlaybackStreams.Color, new TimeSpan(0, 0, 1));

            Assert.That(FramesRecieved, Is.GreaterThan(0));
        }

        [Test]
        public void TestSingelFrameDepth()
        {
            FramesRecieved = 0;
            var reader = KinectSensor.GetDefault().DepthFrameSource.OpenReader();
            reader.FrameArrived += delegate { FramesRecieved++; };


            //KinectTesting.Play(PlaybackTiming.Fast);
            KinectTesting.PlaySingleEvent(PlaybackStreams.Depth, new TimeSpan(0, 0, 1));

            Assert.That(FramesRecieved, Is.GreaterThan(0));
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