using System;
using System.Threading;
using Framework;
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
            KinectTesting.Play(PlaybackTiming.Fast, new TimeSpan(0,0,1), new TimeSpan(0,0,4));
        }

        [Test]
        public void TestSingelFrameBody()
        {
            KinectTesting.PlaySingleEvent(PlaybackStreams.Depth, new TimeSpan(0, 0, 1)); 
        }

        [Test]
        public void TestSingelFrameAudio()
        {
            KinectTesting.PlaySingleEvent(PlaybackStreams.Audio, new TimeSpan(0, 0, 1)); 
        }

        [Test]
        public void TestSingelFrameIr()
        {
            KinectTesting.PlaySingleEvent(PlaybackStreams.Ir, new TimeSpan(0, 0, 1)); 
        }

        [Test]
        public void TestSingelFrameColor()
        {
            KinectTesting.PlaySingleEvent(PlaybackStreams.Color, new TimeSpan(0, 0, 1));

            //KinectTesting.Playback.StartPaused();
            //for (int n = 0; n < 20; n++)
            //{
            //    KinectTesting.Playback.StepOnce();
            //    Thread.Sleep(100);
            //}
        }

        [Test]
        public void TestSingelFrameDepth()
        {
            KinectTesting.PlaySingleEvent(PlaybackStreams.Depth, new TimeSpan(0, 0, 1)); 
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