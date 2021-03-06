﻿using System;
using System.Diagnostics;
using System.IO;
using Framework;
using Microsoft.Kinect;
using NUnit.Framework;

namespace TestSamples
{
    public class RlaybackTests
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
        }

        [Test]
        public void PlayTest_NormalSpeed()
        {
            var framesRecieved = 0;
            var reader = KinectSensor.GetDefault().DepthFrameSource.OpenReader();
            reader.FrameArrived += delegate { framesRecieved++; };

            KinectTesting.Play();

            Assert.That(framesRecieved, Is.GreaterThan(0));
        }

        [Test]
        public void PlayTest_FastSpeed()
        {
            var framesRecieved = 0;
            var reader = KinectSensor.GetDefault().DepthFrameSource.OpenReader();
            reader.FrameArrived += delegate { framesRecieved++; };

            KinectTesting.Play(PlaybackTiming.Fast);

            Assert.That(framesRecieved, Is.GreaterThan(0));
        }

        [Test]
        public void PlayTest_FastFromTo()
        {
            var framesRecieved = 0;
            var reader = KinectSensor.GetDefault().DepthFrameSource.OpenReader();
            reader.FrameArrived += delegate { framesRecieved++; };

            KinectTesting.Play(PlaybackTiming.Fast, new TimeSpan(0, 0, 0, 0, 1), new TimeSpan(0, 0, 0, 0, 500));

            Assert.That(framesRecieved, Is.GreaterThan(0));
        }

        // Note: Playback Throws Exception if PlaySingleEvent is used for Body
        //[Test]
        //public void TestSingelFrameBody()
        //{
        //    var framesRecieved = 0;
        //    var reader = KinectSensor.GetDefault().BodyFrameSource.OpenReader();
        //    reader.FrameArrived += delegate { framesRecieved++; };

        //    KinectTesting.PlaySingleEvent(PlaybackStreams.Body, new TimeSpan(0, 0, 1));

        //    Assert.That(framesRecieved, Is.EqualTo(1));
        //}


        // Note: No Audio Stream in this File
        //[Test]
        //public void TestSingelFrameAudio()
        //{
        //    var framesRecieved = 0;
        //    var reader = KinectSensor.GetDefault().AudioSource.OpenReader();
        //    reader.FrameArrived += delegate { framesRecieved++; };

        //    KinectTesting.PlaySingleEvent(KinectStreams.Audio, new TimeSpan(0, 0, 0, 0, 100));

        //    Assert.That(framesRecieved, Is.EqualTo(1));
        //}

        [Test]
        public void SingleFrameTest_Ir()
        {
            var framesRecieved = 0;
            var reader = KinectSensor.GetDefault().InfraredFrameSource.OpenReader();
            reader.FrameArrived += delegate { framesRecieved++; };

            KinectTesting.PlaySingleEvent(KinectStreams.Ir, new TimeSpan(0, 0, 0, 0, 100));
            KinectTesting.Wait(TimeSpan.FromSeconds(1));
            Assert.That(framesRecieved, Is.EqualTo(1));
        }

        [Test]
        public void SingleFrameTest_Color()
        {
            var framesRecieved = 0;
            var reader = KinectSensor.GetDefault().ColorFrameSource.OpenReader();
            reader.FrameArrived += delegate { framesRecieved++; };

            KinectTesting.PlaySingleEvent(KinectStreams.Color, new TimeSpan(0, 0, 0, 0, 100));
            KinectTesting.Wait(new TimeSpan(0, 0, 1));
            Assert.That(framesRecieved, Is.EqualTo(1));
        }

        [Test]
        public void SingleFrameTest_Depth()
        {
            var framesRecieved = 0;
            var reader = KinectSensor.GetDefault().DepthFrameSource.OpenReader();
            reader.FrameArrived += delegate { framesRecieved++; };

            KinectTesting.PlaySingleEvent(KinectStreams.Depth, new TimeSpan(0, 0, 0, 0, 100));
            KinectTesting.Wait(new TimeSpan(0, 0, 1));
            Assert.That(framesRecieved, Is.EqualTo(1));
        }

        [Test]
        public void PlayNumberOfEventsTest_Depth()
        {
            var framesRecieved = 0;
            var reader = KinectSensor.GetDefault().DepthFrameSource.OpenReader();
            reader.FrameArrived += delegate { framesRecieved++; };
            var watch = new Stopwatch();
            watch.Start();

            KinectTesting.PlayNumberOfEvents(KinectStreams.Depth, new TimeSpan(0, 0, 0, 0, 100), 10);

            watch.Stop();
            Assert.That(framesRecieved, Is.EqualTo(10));
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