using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            ExterminateClient();
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

        public static void DeleteInAndOutPoint()
        {
            Playback.InPointByRelativeTime = new TimeSpan(0);
            Playback.OutPointByRelativeTime = Playback.Duration;
        }

        public static void PlayTillNextMarker()
        {
            CheckPlayback();
            CheckKinectIsOpen();
            StartOrResumePlayback();
            while (Playback.State != KStudioPlaybackState.Paused)
            {
                if (Playback.State == KStudioPlaybackState.Stopped) break;
                Thread.Sleep(50);
            }
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
            CheckPlayback();
            CheckKinectIsOpen();
            Playback.SetPausePointsByRelativeTime(new List<TimeSpan> {markerPosition});
            StartOrResumePlayback();
            while (Playback.State != KStudioPlaybackState.Paused)
            {
                if (Playback.State == KStudioPlaybackState.Stopped) break;
                Thread.Sleep(50);
            }
        }


        public static void PlaySingleEvent(KinectStreams stream, TimeSpan position)
        {
            CheckPlayback();
            CheckKinectIsOpen();
            CheckTimespanIsInRecord(position);
            var tempPausePoints = Playback.PausePointsByRelativeTime.ToList();
            DeletePauseMarkers();
            
            Playback.StartPaused();

            List<KStudioEventStream> finding = null;

            switch (stream)
            {
                // TODO: change where to first
                case KinectStreams.All:
                    Playback.StepOnce();
                    break;
                case KinectStreams.Body:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Body Index")).ToList();
                    // Note: Playback.StepIOnce throws ArgumentExceptions for unknown reasons when using Body or BodyIndex streams.
                    break;
                case KinectStreams.Depth:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Depth")).ToList();
                    break;
                case KinectStreams.Ir:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Nui IR")).ToList();
                    break;
                case KinectStreams.Color:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Uncompressed Color")).ToList();
                    break;
                case KinectStreams.Audio:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Audio")).ToList();
                    break;
            }

            if (finding != null && finding.Any())
            {
                if (stream == KinectStreams.Depth)
                {
                    // Note: Somehow for Depth-StreamEvents you have to fire 2 frames to get one (if you fire 10 you get 9)
                    Playback.StepOnce(finding[0]);
                    while (Playback.State != KStudioPlaybackState.Paused)
                    {
                        Thread.Sleep(200);
                    }
                }
                Playback.StepOnce(finding[0]);
                while (Playback.State != KStudioPlaybackState.Paused)
                {
                    Thread.Sleep(200);
                }
            }
            else
            {
                throw new InvalidOperationException("No Eventstream for " + stream + " in current Playbackfile found.");
            }

            Playback.Stop();
            SetPauseMarkers(tempPausePoints);
        }

        public static void PlayNumberOfEvents(KinectStreams stream, TimeSpan position, int numberOfEvents)
        {
            CheckPlayback();
            CheckKinectIsOpen();
            CheckTimespanIsInRecord(position);

            Playback.StartPaused();
            var tempPausePoints = Playback.PausePointsByRelativeTime.ToList();
            DeletePauseMarkers();

            if (numberOfEvents <= 0) throw new ArgumentException("Number of Events should be greater than 0.");


            List<KStudioEventStream> finding = null;

            switch (stream)
            {
                // TODO: change where to first
                case KinectStreams.All:
                    Playback.StepOnce();
                    break;
                case KinectStreams.Body:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Body Index")).ToList();
                    // Note: Playback.StepIOnce throws ArgumentExceptions for unknown reasons when using Body or BodyIndex streams.
                    break;
                case KinectStreams.Depth:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Depth")).ToList();
                    break;
                case KinectStreams.Ir:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("IR")).ToList();
                    break;
                case KinectStreams.Color:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Uncompressed Color")).ToList();
                    break;
                case KinectStreams.Audio:
                    finding = CurrentEventStreams.Where(k => k.DataTypeName.Contains("Audio")).ToList();
                    break;
            }

            if (finding != null && finding.Any())
            {
                if (stream == KinectStreams.Depth)
                {
                    // Note: Somehow for Depth-StreamEvents you have to fire 2 frames to get one (if you fire 10 you get 9)
                    Playback.StepOnce(finding[0]);
                    while (Playback.State != KStudioPlaybackState.Paused)
                    {
                        if (Playback.State == KStudioPlaybackState.Stopped)
                        {
                            Playback.Pause();
                            break;
                        }
                        Thread.Sleep(50);
                    }
                }

                for (var i = 0; i < numberOfEvents; i++)
                {
                    Playback.StepOnce(finding[0]);
                    while (Playback.State != KStudioPlaybackState.Paused)
                    {
                        if (Playback.State == KStudioPlaybackState.Stopped)
                        {
                            break;
                        }
                        Thread.Sleep(50);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("No Eventstream for " + stream + " in current Playbackfile found.");
            }
            Playback.Stop();
            SetPauseMarkers(tempPausePoints);
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
            CheckPlayback();

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
            DeleteInAndOutPoint();
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

        #region checks

        private static void CheckTimespanIsInRecord(TimeSpan position)
        {
            if (position < Playback.Duration)
            {
                Playback.InPointByRelativeTime = position;
            }
            else
            {
                throw new ArgumentException(
                    "Given position is not within File-Record length. Recordlength is currently " + Playback.Duration +
                    ".");
            }
        }

        private static void CheckPlayback()
        {
            if (Playback == null)
                throw new InvalidOperationException(
                    "Playback needs to be initialised before. Use SetupPlayback() for example.");
        }

        private static void CheckKinectIsOpen()
        {
            if (!KinectSensor.GetDefault().IsOpen)
                Console.WriteLine("WARNING! Default KinectSensor is not Open, Readers will not recieve Frames!");
        }

        #endregion

        public static async Task Wait(TimeSpan timeSpan)
        {
            await Task.Run(async () =>
            {
                await Task.Delay(timeSpan);
            });
        }

    }

    public enum PlaybackTiming
    {
        Fast,
        Normal
    }

    // TODO (KO): Make Flagable
    // TODO (KO): Make Singular
    public enum KinectStreams
    {
        All = 0,
        Body = 1,
        Depth = 2,
        Ir = 4,
        Color = 8,
        Audio = 16
    }
}