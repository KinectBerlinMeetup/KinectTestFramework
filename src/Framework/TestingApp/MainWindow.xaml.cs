using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Microsoft.Kinect;
using Microsoft.Kinect.Tools;

namespace TestingApp
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly KStudioClient _client;
        private KStudioPlayback _playback;
        private KStudioEventReader _eventReader;


        public MainWindow()
        {
            InitializeComponent();
            _client = KStudio.CreateClient();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _client.ConnectToService();
            _eventReader = _client.CreateEventReader(Path.Combine(Environment.CurrentDirectory, "TestFile_allStreams.xef"));
            
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _client.DisconnectFromService();
            _client.Dispose();
        }


        public void RecordClip(string filePath, TimeSpan duration)
        {
            var streamCollection = new KStudioEventStreamSelectorCollection();
            streamCollection.Add(KStudioEventStreamDataTypeIds.Ir);
            streamCollection.Add(KStudioEventStreamDataTypeIds.Depth);
            streamCollection.Add(KStudioEventStreamDataTypeIds.Body);
            streamCollection.Add(KStudioEventStreamDataTypeIds.BodyIndex);

            using (KStudioRecording recording = _client.CreateRecording(filePath, streamCollection))
            {
                recording.StartTimed(duration);
                while (recording.State == KStudioRecordingState.Recording)
                {
                    Thread.Sleep(500);
                }

                if (recording.State == KStudioRecordingState.Error)
                {
                    throw new InvalidOperationException("Error: Recording failed!");
                }
            }
        }

        public void PlaybackClip(string filePath, uint loopCount = 0)
        {
            using (
                KStudioPlayback playback =
                    _client.CreatePlayback(Path.Combine(Environment.CurrentDirectory, filePath)))
            {
                playback.LoopCount = loopCount;
                playback.Start();

                while (playback.State == KStudioPlaybackState.Playing)
                {
                    Thread.Sleep(500);
                }

                if (playback.State == KStudioPlaybackState.Error)
                {
                    throw new InvalidOperationException("Error: Playback failed!");
                }
            }
        }

        private void Record_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void Play_OnClick(object sender, RoutedEventArgs e)
        {
            PlaybackClip("TestFile_allStreams.xef");
        }

        private void StartPaused_OnClick(object sender, RoutedEventArgs e)
        {
            _playback = _client.CreatePlayback(Path.Combine(Environment.CurrentDirectory, "TestFile_allStreams.xef"));
            _playback.StartPaused();
            var streams = _playback.Source.EventStreams;
            //var streams = _client.EventStreams.Select(s => s.DataTypeId.ToString()).ToList();

        }

        private void Pause_OnClick(object sender, RoutedEventArgs e)
        {
            if (_playback != null && _playback.State != KStudioPlaybackState.Paused)
            {
                _playback.Pause();
            }
        }

        private void Steponce_OnClick(object sender, RoutedEventArgs e)
        {
            if (_playback != null && _playback.State == KStudioPlaybackState.Paused)
            {
                _playback.StepOnce(_client.EventStreams[0]); // Stepping through depth events
            }
        }

        private void GetNextEvent_OnClick(object sender, RoutedEventArgs e)
        {
            for (int n = 0; n < 13; n++)
            {
                KStudioEvent _event = _eventReader.GetNextEvent();
                Console.WriteLine("EventTypeID " + _event.EventStreamDataTypeId);
                Console.WriteLine("EventSemanticID " + _event.EventStreamSemanticId);
                Console.WriteLine("EventIndex " + _event.EventIndex);
            }
        }
    }
}