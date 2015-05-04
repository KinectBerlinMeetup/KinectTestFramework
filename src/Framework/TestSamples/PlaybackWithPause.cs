using System;
using System.Collections.Generic;
using Framework;
using NUnit.Framework;

namespace TestSamples
{
    public class PlaybackWithPause
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
            KinectTesting.SetPauseMarkers(new List<TimeSpan>
            {
                new TimeSpan(0, 0, 1),
                new TimeSpan(0, 0, 2),
                new TimeSpan(0, 0, 3)
            });
        }

        [Test]
        public void TestPlayAndIngoreMarker()
        {
            KinectTesting.Play();
        }

        [Test]
        public void TestTillNext()
        {
            KinectTesting.PlayTillNextMarker(); // 
        }

        [Test]
        public void TestTillTimespan()
        {
            KinectTesting.PlayTillMarker(new TimeSpan(0, 0, 3));
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