using System;
using System.Collections.Generic;
using System.IO;
using Framework;
using NUnit.Framework;

namespace TestSamples
{
    public class PlaybackWithPause
    {
        private readonly string _filepath = Path.GetFullPath("Kitty.xef");

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
                new TimeSpan(0, 0, 0, 0, 1),
                new TimeSpan(0, 0, 0, 0, 200),
                new TimeSpan(0, 0, 0, 0, 300)
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
            KinectTesting.PlayTillMarker(new TimeSpan(0, 0, 0, 0, 300));
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