using System;
using System.Collections.ObjectModel;
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

        public static void PlaybackEverythingAndWait()
        {
            if (Playback != null)
            {
                Playback.EndBehavior = KStudioPlaybackEndBehavior.Stop;
                Playback.Start();
                Thread.Sleep(Playback.Duration);
                InvokePlaybackFinishedEvent();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timing">Play with 30Hz or faster?</param>
        /// <param name="start">Play from here</param>
        /// <param name="end">Play till here</param>
        public static void Play(PlaybackTiming timing = PlaybackTiming.Normal, TimeSpan? start = null, TimeSpan? end = null)
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

            if (start != null)
            {
                Playback.InPointByRelativeTime = start.Value;
            }

            if (end != null && end < Playback.Duration)
            {
                Playback.OutPointByRelativeTime = end.Value;
            }

            Playback.EndBehavior = KStudioPlaybackEndBehavior.Stop;
            Playback.Start();
            Thread.Sleep(Playback.Duration);
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

    public enum EventStreams
    {
        All = 0,
        Body = 1,
        Depth = 2,
        Ir = 4,
        Color = 8,
        Audio = 16
    }
}