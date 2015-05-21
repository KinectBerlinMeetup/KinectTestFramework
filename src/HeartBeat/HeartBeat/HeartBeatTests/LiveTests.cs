using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Framework;
using HeartBeat;
using Microsoft.Kinect;
using NUnit.Framework;

namespace HeartBeatTests
{

    public class LiveTests
    {
        [Test]
        [Ignore("For easy execution of a HeartBeat system test.")]
        public async void TestDelayDetection()
        {
            //var kinect = KinectSensor.GetDefault();
            //kinect.Open();

            var frameDelayCount = 0;
            var heartBeat = new HeartBeatCore(new TimeSpan(0, 0, 0, 10), KinectStreams.Depth, KinectSensor.GetDefault());
            heartBeat.FrameDelayEventHandler += delegate
            {
                frameDelayCount++;
                Debug.WriteLine(DateTime.Now + ": delay detected");
            };
            heartBeat.Start();

            //Thread.Sleep(new TimeSpan(0, 0, 0, 30));
            await Task.Run(async () =>
            {
                await Task.Delay(new TimeSpan(1, 0, 0, 30));
            });

            //heartBeat.Stop();
            Debug.WriteLine("Frame delays: " + frameDelayCount);
        }
    }
}
