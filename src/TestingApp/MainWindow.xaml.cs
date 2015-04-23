using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect.Tools;

namespace TestingApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            KStudioClient client = KStudio.CreateClient();
        }


        public static void RecordClip(string filePath, TimeSpan duration)
        {
            using (KStudioClient client = KStudio.CreateClient())
            {
                client.ConnectToService();

                var streamCollection = new KStudioEventStreamSelectorCollection();
                streamCollection.Add(KStudioEventStreamDataTypeIds.Ir);
                streamCollection.Add(KStudioEventStreamDataTypeIds.Depth);
                streamCollection.Add(KStudioEventStreamDataTypeIds.Body);
                streamCollection.Add(KStudioEventStreamDataTypeIds.BodyIndex);

                using (KStudioRecording recording = client.CreateRecording(filePath, streamCollection))
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

                client.DisconnectFromService();
            }
        }

        public static void PlaybackClip(string filePath, uint loopCount)
        {
            using (KStudioClient client = KStudio.CreateClient())
            {
                client.ConnectToService();

                using (KStudioPlayback playback = client.CreatePlayback(filePath))
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

                client.DisconnectFromService();
            }
        }
    }
}
