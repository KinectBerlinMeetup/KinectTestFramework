using System;
using System.Threading;
using Framework;
using HeartBeat;
using Microsoft.Kinect;
using NUnit.Framework;

namespace HeartBeatTests
{
    public class HeartBeatCore_UnitTests
    {

        [Test]
        public void ConstructorTest_Null()
        {
            var heartBeat = new HeartBeatCore(null, KinectStreams.All, null);
            Assert.That(heartBeat.FrameDelayTolerance, Is.EqualTo(heartBeat.Default_FrameDelayTolerance));
            Assert.That(heartBeat.Kinect, Is.EqualTo(KinectSensor.GetDefault()));
        }

        [Test]
        public void ConstructorTest_Filled()
        {
            var heartBeat = new HeartBeatCore(new TimeSpan(0, 0, 1), KinectStreams.All, KinectSensor.GetDefault());
            Assert.That(heartBeat.FrameDelayTolerance, Is.EqualTo(new TimeSpan(0, 0, 1)));
            Assert.That(heartBeat.Kinect, Is.EqualTo(KinectSensor.GetDefault()));
        }

        [Test]
        public void IsKinectOpenTest_NotOpen()
        {
            var heartBeat = new HeartBeatCore(null, KinectStreams.All, null);
            heartBeat.Start();
            Thread.Sleep(100);
            Assert.That(KinectSensor.GetDefault().IsOpen);
        }

        [Test]
        public void IsKinectOpenTest_Open()
        {
            KinectSensor.GetDefault().Open();
            var heartBeat = new HeartBeatCore(null, KinectStreams.All, null);
            heartBeat.Start();
            Thread.Sleep(100);
            Assert.That(KinectSensor.GetDefault().IsOpen);
        }

        [Test]
        public void StreamInitializationTest_AudioStream()
        {
            var heartBeat = new HeartBeatCore(null, KinectStreams.Audio, KinectSensor.GetDefault());
            heartBeat.InitializeReaders(heartBeat.SelectedStreams);
            Assert.That(heartBeat.AudioReader, Is.Not.Null);

            Assert.That(heartBeat.DepthReader, Is.Null);
            Assert.That(heartBeat.IrReader, Is.Null);
            Assert.That(heartBeat.ColorReader, Is.Null);
            Assert.That(heartBeat.BodyReader, Is.Null);
        }

        [Test]
        public void StreamInitializationTest_DepthStream()
        {
            var heartBeat = new HeartBeatCore(null, KinectStreams.Depth, KinectSensor.GetDefault());
            heartBeat.InitializeReaders(heartBeat.SelectedStreams);
            Assert.That(heartBeat.DepthReader, Is.Not.Null);

            Assert.That(heartBeat.AudioReader, Is.Null);
            Assert.That(heartBeat.IrReader, Is.Null);
            Assert.That(heartBeat.ColorReader, Is.Null);
            Assert.That(heartBeat.BodyReader, Is.Null);
        }

        [Test]
        public void StreamInitializationTest_IrStream()
        {
            var heartBeat = new HeartBeatCore(null, KinectStreams.Ir, KinectSensor.GetDefault());
            heartBeat.InitializeReaders(heartBeat.SelectedStreams);
            Assert.That(heartBeat.IrReader, Is.Not.Null);

            Assert.That(heartBeat.AudioReader, Is.Null);
            Assert.That(heartBeat.DepthReader, Is.Null);
            Assert.That(heartBeat.ColorReader, Is.Null);
            Assert.That(heartBeat.BodyReader, Is.Null);
        }

        [Test]
        public void StreamInitializationTest_ColorStream()
        {
            var heartBeat = new HeartBeatCore(null, KinectStreams.Color, KinectSensor.GetDefault());
            heartBeat.InitializeReaders(heartBeat.SelectedStreams);
            Assert.That(heartBeat.ColorReader, Is.Not.Null);

            Assert.That(heartBeat.AudioReader, Is.Null);
            Assert.That(heartBeat.DepthReader, Is.Null);
            Assert.That(heartBeat.IrReader, Is.Null);
            Assert.That(heartBeat.BodyReader, Is.Null);
        }

        [Test]
        public void StreamInitializationTest_BodyStream()
        {
            var heartBeat = new HeartBeatCore(null, KinectStreams.Body, KinectSensor.GetDefault());
            heartBeat.InitializeReaders(heartBeat.SelectedStreams);
            Assert.That(heartBeat.BodyReader, Is.Not.Null);

            Assert.That(heartBeat.AudioReader, Is.Null);
            Assert.That(heartBeat.DepthReader, Is.Null);
            Assert.That(heartBeat.IrReader, Is.Null);
            Assert.That(heartBeat.ColorReader, Is.Null);
        }

        [Test]
        public void StreamInitializationTest_AllStreams()
        {
            var heartBeat = new HeartBeatCore(null, KinectStreams.All, KinectSensor.GetDefault());
            heartBeat.InitializeReaders(heartBeat.SelectedStreams);
            Assert.That(heartBeat.AudioReader, Is.Not.Null);
            Assert.That(heartBeat.DepthReader, Is.Not.Null);
            Assert.That(heartBeat.IrReader, Is.Not.Null);
            Assert.That(heartBeat.ColorReader, Is.Not.Null);
            Assert.That(heartBeat.BodyReader, Is.Not.Null);
        }

        [Test]
        public void StreamShutdownTest_AllStreams()
        {
            var heartBeat = new HeartBeatCore(null, KinectStreams.All, KinectSensor.GetDefault());
            heartBeat.InitializeReaders(heartBeat.SelectedStreams);
            heartBeat.ShutdownReaders(heartBeat.SelectedStreams);

            Assert.That(heartBeat.AudioReader, Is.Null);
            Assert.That(heartBeat.DepthReader, Is.Null);
            Assert.That(heartBeat.IrReader, Is.Null);
            Assert.That(heartBeat.ColorReader, Is.Null);
            Assert.That(heartBeat.BodyReader, Is.Null);
        }


    }
}