using System;
using System.Collections.Generic;
using System.Linq;
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

        public static List<Body[]> GetAllBodies(KStudioPlayback playback)
        {
            var bodies = new List<Body[]>();

            var reader = KinectSensor.GetDefault().BodyFrameSource.OpenReader();
            reader.FrameArrived += delegate(object sender, BodyFrameArrivedEventArgs e)
            {
                var frameRef = e.FrameReference;
                using (var frame = frameRef.AcquireFrame())
                {
                    if (frame != null)
                    {
                        bodies.Add(frame.GetSkeletons());
                    }
                }
            };

            KinectTesting.Play();
            return bodies;
        }

        public static List<Body> GetBodyTrackedBodiesForSingleFrame(TimeSpan? position, KStudioPlayback playback)
        {
            if (position == null)
            {
                position = new TimeSpan(0, 0, 0);
            }

            var bodies = new List<Body>();

            var reader = KinectSensor.GetDefault().BodyFrameSource.OpenReader();
            reader.FrameArrived += delegate(object sender, BodyFrameArrivedEventArgs e)
            {
                var frameRef = e.FrameReference;
                using (var frame = frameRef.AcquireFrame())
                {
                    if (frame != null)
                    {
                        if (bodies.Count == 0)
                        {
                            bodies = frame.GetSkeletons().Where(s => s.IsTracked).ToList();
                        }
                    }
                }
            };

            KinectTesting.Play(PlaybackTiming.Normal, position);

            var n = 0;
            while (bodies.Count == 0)
            {
                if (n == 100) throw new TimeoutException("No Bodies were set in given Time.");
                Thread.Sleep(50);
            }

            return bodies;
        }

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

        public static AudioFrameWrap GetWrap(this AudioBeamFrame frame)
        {
            var wrap = new AudioFrameWrap();
            if (frame == null) return null;
            wrap.Duration = frame.Duration;
            wrap.AudioBeam = frame.AudioBeam;
            wrap.RelativeTimeStart = frame.RelativeTimeStart;
            wrap.SubFrames = frame.SubFrames;

            return wrap;
        }

        public static ColorFrameWrap GetWrap(this ColorFrame frame)
        {
            var wrap = new ColorFrameWrap();
            if (frame == null) return null;
            wrap.ColorCameraSettings = frame.ColorCameraSettings;
            wrap.FrameDescription = frame.FrameDescription;
            wrap.RawColorImageFormat = frame.RawColorImageFormat;
            wrap.RelativeTime = frame.RelativeTime;

            var array =
                new byte[wrap.FrameDescription.Width*wrap.FrameDescription.Height*wrap.FrameDescription.BytesPerPixel];
            ;
            frame.CopyConvertedFrameDataToArray(array, ColorImageFormat.Rgba);
            wrap.RgbaPixelArray = array;

            return wrap;
        }

        public static DepthFrameWrap GetWrap(this DepthFrame frame)
        {
            var wrap = new DepthFrameWrap();
            if (frame == null) return null;
            wrap.FrameDescription = frame.FrameDescription;
            wrap.RelativeTime = frame.RelativeTime;

            var array =
                new ushort[wrap.FrameDescription.Width*wrap.FrameDescription.Height*wrap.FrameDescription.BytesPerPixel];
            frame.CopyFrameDataToArray(array);
            wrap.DepthPixelArray = array;
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

        public static MultiFrameWrap GetWrap(this MultiSourceFrame frame)
        {
            var wrap = new MultiFrameWrap
            {
                BodyFrameWrap = frame.BodyFrameReference.AcquireFrame().GetWrap(),
                ColorFrameWrap = frame.ColorFrameReference.AcquireFrame().GetWrap(),
                DepthFrameWrap = frame.DepthFrameReference.AcquireFrame().GetWrap()
                // TODO: Add other Wraps
            };
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

            return BodyWraps.GetRange(0, arrayLength).ToArray();
        }
    }
}