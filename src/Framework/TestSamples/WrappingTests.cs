using System;
using System.IO;
using Framework;
using NUnit.Framework;

namespace TestSamples
{
    public class WrappingTests
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
        public void TestBodyFrameWrap()
        {
            var wrap = KinectWrapping.GetSingleBodyFrameWrapWithTrackedBody(new TimeSpan(0, 0, 0, 0, 1),
                KinectTesting.Playback);
            Assert.That(wrap, Is.Not.Null);
        }

        [Test]
        public void TestBodies()
        {
            var wrap = KinectWrapping.GetAllBodies(KinectTesting.Playback);

            Assert.That(wrap.Count, Is.GreaterThan(100));
        }

        [Test]
        public void TestBodiesSingleFrame()
        {
            var bodies = KinectWrapping.GetBodyTrackedBodiesForSingleFrame(new TimeSpan(0, 0, 0, 0, 1), KinectTesting.Playback);
            Assert.That(bodies.Count, Is.GreaterThan(0));
        }

        [Test]
        public void TestBodyFrameWrapArray()
        {
            var wraps = KinectWrapping.GetArrayOfBodyFrameWraps(new TimeSpan(0, 0, 0, 0, 1), KinectTesting.Playback, 15);
            Assert.That(wraps.Length, Is.EqualTo(15));
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