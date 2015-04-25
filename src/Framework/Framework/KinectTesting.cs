using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
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

        #endregion

        #region Extermination

        public static void ExterminatePlayback()
        {
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
            List<TimeSpan> pausePoints = Playback.PausePointsByRelativeTime.ToList();
            foreach (TimeSpan point in pausePoints)
            {
                Playback.RemovePausePointByRelativeTime(point);
            }
        }

        public static void PlayTillNextMarker()
        {
            if (Playback == null) return;
            TimeSpan duration = Playback.PausePointsByRelativeTime.First(f => f > Playback.CurrentRelativeTime) -
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
            TimeSpan duration = markerPosition - Playback.CurrentRelativeTime;
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
            if (position < Playback.Duration)
            {
                Playback.InPointByRelativeTime = position;
                Playback.StartPaused();
            }

            List<KStudioEventStream> finding = null;

            switch (stream)
            {
                case PlaybackStreams.All:
                    Playback.StepOnce();
                    break;
                case PlaybackStreams.Body:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Body")).ToList();
                    break;
                case PlaybackStreams.Depth:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Depth")).ToList();
                    break;
                case PlaybackStreams.Ir:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Depth")).ToList();
                    break;
                case PlaybackStreams.Color:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Depth")).ToList();
                    break;
                case PlaybackStreams.Audio:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Depth")).ToList();
                    break;
            }

            if (finding != null && finding.Any())
            {
                Playback.StepOnce(finding[0]);
            }
            Thread.Sleep(15);
            Playback.Stop();
        }

        /// <summary>
        ///     Note: Stop and Start needs longer thant pause and resume. Therefore for sequential playback of different parts,
        ///     rather write another method.
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
            else
            {
                start = Playback.StartRelativeTime;
            }

            if (end != null && end < Playback.Duration)
            {
                Playback.OutPointByRelativeTime = end.Value;
            }
            else
            {
                end = Playback.Duration;
            }

            TimeSpan duration = end.Value - start.Value;

            var tempPausePoints = Playback.PausePointsByRelativeTime.ToList();
            DeletePauseMarkers();
            Playback.EndBehavior = KStudioPlaybackEndBehavior.Stop;
            Playback.Start();
            Thread.Sleep(duration);
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