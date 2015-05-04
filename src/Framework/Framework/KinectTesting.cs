using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Microsoft.Kinect;
using Microsoft.Kinect.Tools;

namespace Framework
{
    public static class KinectTesting
    {
        public static KStudioClient Client;
        public static KStudioPlayback Playback;
        public static ReadOnlyObservableCollection<KStudioEventStream> CurrentEventStreams;
        public static EventHandler PlaybackFinished;

        #region SetUp

        public static void SetupPlayback(string filepath)
        {
            SetupClientAndConnect();
            if (Client != null && Client.IsServiceConnected)
            {
                Playback = Client.CreatePlayback(filepath);
                CurrentEventStreams = Playback.Source.EventStreams;
            }
            SetupKinect();
        }

        public static void SetupClientAndConnect()
        {
            SetupClient();
            ConnectClientToService();
        }

        public static bool ConnectClientToService()
        {
            if (Client != null)
            {
                Client.ConnectToService();
                return Client.IsServiceConnected;
            }
            return false;
        }

        public static void SetupClient()
        {
            if (Client == null)
            {
                Client = KStudio.CreateClient();
            }
        }

        public static void SetupKinect(KinectSensor kinect = null)
        {
            if (kinect == null) kinect = KinectSensor.GetDefault();
            kinect.Open();
        }

        #endregion

        #region Extermination

        public static void ExterminateKinect(KinectSensor kinect = null)
        {
            if (kinect == null) kinect = KinectSensor.GetDefault();
            kinect.Close();
        }

        public static void ExterminatePlayback()
        {
            ExterminateKinect();
            if (Playback != null)
            {
                CurrentEventStreams = null;
                Playback.Dispose();
                Playback = null;
            }
        }

        public static void ExterminateClient()
        {
            if (Client != null)
            {
                DisconnectClient();
                Client.Dispose();
                Client = null;
            }
        }

        public static void DisconnectClient()
        {
            if (Client != null && Client.IsServiceConnected)
            {
                Client.DisconnectFromService();
            }
        }

        #endregion

        #region Playback

        public static void SetPauseMarkers(List<TimeSpan> markers)
        {
            DeletePauseMarkers();
            Playback.SetPausePointsByRelativeTime(markers);
        }

        public static void DeletePauseMarkers()
        {
            var pausePoints = Playback.PausePointsByRelativeTime.ToList();
            foreach (var point in pausePoints)
            {
                Playback.RemovePausePointByRelativeTime(point);
            }
        }

        public static void PlayTillNextMarker()
        {
            if (Playback == null) return;
            var duration = Playback.PausePointsByRelativeTime.First(f => f > Playback.CurrentRelativeTime) -
                           Playback.CurrentRelativeTime;
            StartOrResumePlayback();
            Thread.Sleep(duration);
        }

        public static void StartOrResumePlayback()
        {
            if (Playback.State == KStudioPlaybackState.Paused)
            {
                Playback.Resume();
            }
            else
            {
                Playback.Start();
            }
        }

        public static void PlayTillMarker(TimeSpan markerPosition)
        {
            if (Playback == null) return;
            var duration = markerPosition - Playback.CurrentRelativeTime;
            Playback.SetPausePointsByRelativeTime(new List<TimeSpan> {markerPosition});
            StartOrResumePlayback();
            Thread.Sleep(duration);
        }


        public static void PlayAllWithoutPauses()
        {
            if (Playback != null)
            {
                var tempPausePoints = Playback.PausePointsByRelativeTime.ToList();
                DeletePauseMarkers();
                Playback.EndBehavior = KStudioPlaybackEndBehavior.Stop;
                Playback.Start();
                Thread.Sleep(Playback.Duration);
                SetPauseMarkers(tempPausePoints);
                InvokePlaybackFinishedEvent();
            }
        }

        public static void PlaySingleEvent(PlaybackStreams stream, TimeSpan position)
        {
            if (Playback == null) return;
            CheckKinectIsOpen();
            if (position < Playback.Duration)
            {
                Playback.InPointByRelativeTime = position;
                Playback.StartPaused();
            }

            List<KStudioEventStream> finding = null;

            switch (stream)
            {
                // TODO: change where to first
                case PlaybackStreams.All:
                    Playback.StepOnce();
                    break;
                case PlaybackStreams.Body:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Body Index")).ToList();
                    // Note: Playback.StepIOnce throws ArgumentExceptions for unknown reasons when using Body or BodyIndex streams.
                    break;
                case PlaybackStreams.Depth:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Depth")).ToList();
                    break;
                case PlaybackStreams.Ir:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("IR")).ToList();
                    break;
                case PlaybackStreams.Color:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Uncompressed Color")).ToList();
                    break;
                case PlaybackStreams.Audio:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Audio")).ToList();
                    break;
            }

            var index = CurrentEventStreams.IndexOf(finding[0]);
            var index2 = Client.EventStreams.IndexOf(finding[0]);


            if (finding != null && finding.Any())
            {
                if (stream == PlaybackStreams.Depth)
                {
                    // Note: Somehow for Depth-StreamEvents you have to fire 2 frames to get one (if you fire 10 you get 9)
                    Playback.StepOnce(finding[0]);
                    Thread.Sleep(500);
                }
                Playback.StepOnce(finding[0]);
            }
            else
            {
                throw new InvalidOperationException("No Eventstream for " + stream + " in current Playbackfile found.");
            }

            Thread.Sleep(500); // if sleep value is to small, no frames arrive at eventhandlers
            Playback.Stop();
        }

        private static void CheckKinectIsOpen()
        {
            if (!KinectSensor.GetDefault().IsOpen)
                throw new InvalidOperationException("You won't get any frames because sensor is not open");
        }

        /// <summary>
        ///     Note: Stop and Start needs longer thant pause and resume. Therefore for sequential playback of different parts,
        ///     rather write another method.
        ///     Note: ingores Pausepoints
        /// </summary>
        /// <param name="timing">Play with 30Hz or faster?</param>
        /// <param name="start">Play from here</param>
        /// <param name="end">Play till here</param>
        public static void Play(PlaybackTiming timing = PlaybackTiming.Normal, TimeSpan? start = null,
            TimeSpan? end = null)
        {
            if (Playback == null) return;

            switch (timing)
            {
                case PlaybackTiming.Fast:
                    Playback.Mode = KStudioPlaybackMode.TimingDisabled;
                    break;
                case PlaybackTiming.Normal:
                    Playback.Mode = KStudioPlaybackMode.TimingEnabled;
                    break;
            }

            if (start != null && start < Playback.Duration)
            {
                Playback.InPointByRelativeTime = start.Value;
            }

            if (end != null && end < Playback.Duration)
            {
                Playback.OutPointByRelativeTime = end.Value;
            }

            var tempPausePoints = Playback.PausePointsByRelativeTime.ToList();

            DeletePauseMarkers();
            Playback.EndBehavior = KStudioPlaybackEndBehavior.Stop;
            Playback.Start();

            while (Playback.State != KStudioPlaybackState.Stopped)
            {
                Thread.Sleep(50);
            }
            SetPauseMarkers(tempPausePoints);
            InvokePlaybackFinishedEvent();
        }

        private static void InvokePlaybackFinishedEvent()
        {
            if (PlaybackFinished != null)
            {
                PlaybackFinished.Invoke(Playback, new EventArgs());
            }
        }

        #endregion
    }

    public enum PlaybackTiming
    {
        Fast,
        Normal
    }

    public enum PlaybackStreams
    {
        All = 0,
        Body = 1,
        Depth = 2,
        Ir = 4,
        Color = 8,
        Audio = 16
    }
}