using System;
using System.Collections.Generic;
using System.Threading;
using Framework.Wraps;
using Microsoft.Kinect;
using Microsoft.Kinect.Tools;

// DepthFrame
// IRFrame
// AudioFrame?
// Body Frame
// BodyIndexFrame
// MultiFrame

// Je: ExtentionMethods zum Befüllen der Frames
//     Datenhalterklasse
//     Convert-Methoden?
//     evtl Statistische Methoden / Summary der Daten
//    

namespace Framework
{
    public static class KinectWrapping
    {
        private static BodyFrameWrap BodyWrap { get; set; }
        private static List<BodyFrameWrap> BodyWraps { get; set; }

        public static BodyFrameWrap GetSingleBodyFrameWrapWithTrackedBody(TimeSpan? position, KStudioPlayback playback)
        {
            if (position == null)
            {
                position = new TimeSpan(0, 0, 0);
            }

            var reader = KinectSensor.GetDefault().BodyFrameSource.OpenReader();
            reader.FrameArrived += delegate(object sender, BodyFrameArrivedEventArgs e)
            {
                var frameRef = e.FrameReference;
                using (var frame = frameRef.AcquireFrame())
                {
                    if (frame != null)
                    {
                        if (frame.TrackedSkeleton() != null)
                        {
                            BodyWrap = frame.GetWrap();
                        }
                    }
                }
            };

            KinectTesting.Play(PlaybackTiming.Fast, position);

            var n = 0;
            while (BodyWrap == null)
            {
                if (n == 100) throw new TimeoutException("No Bodywrap was set in given Time.");
                Thread.Sleep(50);
            }

            return BodyWrap;
        }

        public static BodyFrameWrap GetWrap(this BodyFrame frame)
        {
            var wrap = new BodyFrameWrap();
            if (frame == null) return null;
            wrap.BodyCount = frame.BodyCount;
            wrap.BodyFrameSource = frame.BodyFrameSource;
            wrap.FloorClipPlane = frame.FloorClipPlane;
            wrap.RelativeTime = frame.RelativeTime;
            var bodies = new Body[wrap.BodyCount];

            frame.GetAndRefreshBodyData(bodies);
            wrap.Bodies = bodies;

            return wrap;
        }

        public static BodyWrap GetWrap(this Body body)
        {
            var wrap = new BodyWrap();
            if (body == null) return null;
            wrap.ClippedEdges = body.ClippedEdges;
            wrap.HandLeftConfidence = body.HandLeftConfidence;
            wrap.HandRightConfidence = body.HandRightConfidence;
            wrap.HandLeftState = body.HandLeftState;
            wrap.HandRightState = body.HandRightState;
            wrap.IsRestricted = body.IsRestricted;
            wrap.JointOrientations = body.JointOrientations;
            wrap.Joints = body.Joints;
            wrap.IsTracked = body.IsTracked;
            wrap.Lean = body.Lean;
            wrap.LeanTrackingState = body.LeanTrackingState;
            wrap.TrackingId = body.TrackingId;

            return wrap;
        }

        public static BodyFrameWrap[] GetArrayOfBodyFrameWraps(TimeSpan? position, KStudioPlayback playback,
            int arrayLength)
        {
            BodyWraps = new List<BodyFrameWrap>();
            if (position == null)
            {
                position = new TimeSpan(0, 0, 0);
            }

            var reader = KinectSensor.GetDefault().BodyFrameSource.OpenReader();
            reader.FrameArrived += delegate(object sender, BodyFrameArrivedEventArgs e)
            {
                var frameRef = e.FrameReference;
                using (var frame = frameRef.AcquireFrame())
                {
                    if (frame != null)
                    {
                        BodyWraps.Add(frame.GetWrap());
                    }
                }
            };

            KinectTesting.Play(PlaybackTiming.Normal, position);

            var n = 0;
            while (BodyWraps.Count < arrayLength)
            {
                if (n == 100) throw new TimeoutException("No Bodywrap was set in given Time.");
                Thread.Sleep(50);
            }

            return BodyWraps.GetRange(0,arrayLength).ToArray();
        }
    }
}