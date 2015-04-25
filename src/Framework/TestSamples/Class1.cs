﻿using System;
using Framework;
using NUnit.Framework;

namespace TestSamples
{
    public class Class1
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
            KinectTesting.PlaybackEverythingAndWait(); // async and await? 
        }

        [Test]
        public void TestSingelFrameBody()
        {
            KinectTesting.PlaySingleEvent(PlaybackStreams.Depth, new TimeSpan(0, 0, 1)); // async and await? 
        }

        [Test]
        public void TestSingelFrameAudio()
        {
            KinectTesting.PlaySingleEvent(PlaybackStreams.Audio, new TimeSpan(0, 0, 1)); // async and await? 
        }

        [Test]
        public void TestSingelFrameIr()
        {
            KinectTesting.PlaySingleEvent(PlaybackStreams.Ir, new TimeSpan(0, 0, 1)); // async and await? 
        }

        [Test]
        public void TestSingelFrameColor()
        {
            KinectTesting.PlaySingleEvent(PlaybackStreams.Color, new TimeSpan(0, 0, 1)); // async and await? 
        }

        [Test]
        public void TestSingelFrameDepth()
        {
            KinectTesting.PlaySingleEvent(PlaybackStreams.Depth, new TimeSpan(0, 0, 1)); // async and await? 
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