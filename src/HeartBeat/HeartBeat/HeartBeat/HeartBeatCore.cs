using System;
using System.Collections.Generic;
using System.Timers;
using Framework;
using Microsoft.Kinect;

namespace HeartBeat
{
    // Main Logic Class
    public class HeartBeatCore
    {
        public readonly TimeSpan Default_FrameDelayTolerance = new TimeSpan(0, 0, 0, 1);
        public TimeSpan FrameDelayTolerance;
        public KinectStreams SelectedStreams;
        public KinectSensor Kinect;
        public EventHandler<FrameDelayEventArgs> FrameDelayEventHandler;
        public ColorFrameReader ColorReader { get; private set; }
        public InfraredFrameReader IrReader { get; private set; }
        public DepthFrameReader DepthReader { get; private set; }
        public BodyFrameReader BodyReader { get; private set; }
        public AudioBeamFrameReader AudioReader { get; private set; }
        public StreamMonitor BodyStreamMonitor { get; private set; }
        public StreamMonitor AudioStreamMonitor { get; private set; }
        public StreamMonitor ColorStreamMonitor { get; private set; }
        public StreamMonitor DepthStreamMonitor { get; private set; }
        public StreamMonitor IrStreamMonitor { get; private set; }
        public Timer HeartBeat { get; private set; }

        public HeartBeatCore(TimeSpan? frameDelayTolerance, KinectStreams streams, KinectSensor kinect)
        {
            FrameDelayTolerance = frameDelayTolerance ?? Default_FrameDelayTolerance; // sets Tolerance if given 
            SelectedStreams = streams;
            Kinect = kinect ?? KinectSensor.GetDefault();
            HeartBeat = new Timer(FrameDelayTolerance.TotalMilliseconds);
            HeartBeat.Elapsed += HeartBeat_Elapsed;
            InitializeReaders(SelectedStreams);
        }

        public void ShutdownReaders(KinectStreams selectedStreams)
        {
            switch (selectedStreams)
            {
                case KinectStreams.All:
                    BodyReader.Dispose();
                    BodyReader = null;
                    DepthReader.Dispose();
                    DepthReader = null;
                    IrReader.Dispose();
                    IrReader = null;
                    ColorReader.Dispose();
                    ColorReader = null;
                    AudioReader.Dispose();
                    AudioReader = null;
                    break;
                case KinectStreams.Body:
                    BodyReader.Dispose();
                    BodyReader = null;
                    break;
                case KinectStreams.Depth:
                    DepthReader.Dispose();
                    DepthReader = null;
                    break;
                case KinectStreams.Ir:
                    IrReader.Dispose();
                    IrReader = null;
                    break;
                case KinectStreams.Color:
                    ColorReader.Dispose();
                    ColorReader = null;
                    break;
                case KinectStreams.Audio:
                    AudioReader.Dispose();
                    AudioReader = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("selectedStreams", selectedStreams, null);
            }
        }

        public void InitializeReaders(KinectStreams selectedStreams)
        {
            switch (selectedStreams)
            {
                case KinectStreams.All:
                    InitializeBodyReader();
                    InitializeDepthReader();
                    InitializeInfraredReader();
                    InitializeColorReader();
                    InitializeAudioReader();
                    break;
                case KinectStreams.Body:
                    InitializeBodyReader();
                    break;
                case KinectStreams.Depth:
                    InitializeDepthReader();
                    break;
                case KinectStreams.Ir:
                    InitializeInfraredReader();
                    break;
                case KinectStreams.Color:
                    InitializeColorReader();
                    break;
                case KinectStreams.Audio:
                    InitializeAudioReader();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("selectedStreams", selectedStreams, null);
            }
        }

        public void Start()
        {
            if (!Kinect.IsOpen)
            {
                Kinect.Open();
            }
            HeartBeat.Start();
        }

        private void HeartBeat_Elapsed(object sender, ElapsedEventArgs e)
        {
            var now = DateTimeOffset.UtcNow;
            CheckMonitors(SelectedStreams, now);
        }

        // TODO (KO): Not sure if format is okay
        private void CheckMonitors(KinectStreams selectedStreams, DateTimeOffset now)
        {
            var monitors = GetMonitors(selectedStreams);
            foreach (var monitor in monitors)
            {
                if ((now - monitor.LastFrameTimestemp) > FrameDelayTolerance)
                {
                    if (FrameDelayEventHandler != null)
                    {
                        FrameDelayEventHandler.Invoke(monitor, new FrameDelayEventArgs(monitor.LastFrameTimestemp - now, monitor.StreamType));
                    }
                }
            }
        }

        private List<StreamMonitor> GetMonitors(KinectStreams streams)
        {
            switch (streams)
            {
                case KinectStreams.All:
                    return new List<StreamMonitor>
                    {
                        BodyStreamMonitor,
                        DepthStreamMonitor,
                        IrStreamMonitor,
                        ColorStreamMonitor,
                        AudioStreamMonitor
                    };
                case KinectStreams.Body:
                    return new List<StreamMonitor> {BodyStreamMonitor};
                case KinectStreams.Depth:
                    return new List<StreamMonitor> {DepthStreamMonitor};
                case KinectStreams.Ir:
                    return new List<StreamMonitor> {IrStreamMonitor};
                case KinectStreams.Color:
                    return new List<StreamMonitor> {ColorStreamMonitor};
                case KinectStreams.Audio:
                    return new List<StreamMonitor> {AudioStreamMonitor};
                default:
                    throw new ArgumentOutOfRangeException("streams", streams, null);
            }
        }

        private void InitializeBodyReader()
        {
            BodyReader = Kinect.BodyFrameSource.OpenReader();
            BodyReader.FrameArrived += BodyReader_FrameArrived;
            BodyStreamMonitor = new StreamMonitor(KinectStreams.Body);
        }

        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            var frameReference = e.FrameReference;
            using (var frame = frameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    BodyStreamMonitor.Update(frame.RelativeTime);
                }
            }
        }

        private void InitializeDepthReader()
        {
            DepthReader = Kinect.DepthFrameSource.OpenReader();
            DepthReader.FrameArrived += DepthReader_FrameArrived;
            DepthStreamMonitor = new StreamMonitor(KinectStreams.Depth);
        }

        private void DepthReader_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            var frameReference = e.FrameReference;
            using (var frame = frameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    DepthStreamMonitor.Update(frame.RelativeTime);
                }
            }
        }

        private void InitializeColorReader()
        {
            ColorReader = Kinect.ColorFrameSource.OpenReader();
            ColorReader.FrameArrived += ColorReader_FrameArrived;
            ColorStreamMonitor = new StreamMonitor(KinectStreams.Color);
        }

        private void ColorReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            var frameReference = e.FrameReference;
            using (var frame = frameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    ColorStreamMonitor.Update(frame.RelativeTime);
                }
            }
        }

        private void InitializeInfraredReader()
        {
            IrReader = Kinect.InfraredFrameSource.OpenReader();
            IrReader.FrameArrived += IrReader_FrameArrived;
            IrStreamMonitor = new StreamMonitor(KinectStreams.Ir);
        }

        private void IrReader_FrameArrived(object sender, InfraredFrameArrivedEventArgs e)
        {
            var frameReference = e.FrameReference;
            using (var frame = frameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    IrStreamMonitor.Update(frame.RelativeTime);
                }
            }
        }

        private void InitializeAudioReader()
        {
            AudioReader = Kinect.AudioSource.OpenReader();
            AudioReader.FrameArrived += AudioReader_FrameArrived;
            AudioStreamMonitor = new StreamMonitor(KinectStreams.Audio);
        }

        private void AudioReader_FrameArrived(object sender, AudioBeamFrameArrivedEventArgs e)
        {
            var frameReference = e.FrameReference;
            using (var frame = frameReference.AcquireBeamFrames())
            {
                if (frame != null)
                {
                    AudioStreamMonitor.Update(frame[0].RelativeTimeStart);
                }
            }
        }

        public void Stop()
        {
            if (Kinect.IsOpen)
            {
                Kinect.Close();
            }
            HeartBeat.Stop();
            ShutdownReaders(SelectedStreams);
        }
    }

    public class FrameDelayEventArgs : EventArgs
    {
        public TimeSpan FrameDelay;
        public KinectStreams Type;

        public FrameDelayEventArgs(TimeSpan frameDelay, KinectStreams type)
        {
            FrameDelay = frameDelay;
            Type = type;
        }
    }
}