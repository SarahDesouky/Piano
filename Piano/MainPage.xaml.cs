using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.Editing;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Piano
{

    public sealed partial class MainPage : Page
    {

        public StorageFile _recordStorageFile { get; set; }
        private MediaCapture _mediaCaptureManager;
        private string _audioFileName = "VoiceRecording.mp3";

        int record = 0;
        MediaComposition mediacomposition;
        DateTime begin = new DateTime();
        DateTime now = new DateTime();
        TimeSpan diff = new TimeSpan();
        DateTime end = new DateTime();
        TimeSpan totalRecordingTime = new TimeSpan();
        BackgroundAudioTrack track;
        public string fileName = "PianoRecording.mp3";

        DispatcherTimer timer = new DispatcherTimer();
        



        public MainPage()
        {
            this.InitializeComponent();
            diff = new TimeSpan();
            begin = new DateTime();
            now = new DateTime();
            InitializeAudioRecording();
            mediacomposition = new MediaComposition();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += DispatcherTimerEventHandler;
            
        }

        private void DispatcherTimerEventHandler(object sender, object e)
        {
            int seconds = Convert.ToInt32(Seconds.Text);
            int minutes = Convert.ToInt32(Minute.Text);
           
            if(seconds%2!=0)
            {
                circle.Fill = new SolidColorBrush(Colors.Black);
            }
            else
            {
                circle.Fill = new SolidColorBrush(Colors.Red);
            }
          
            seconds += 1;
            if(seconds==60)
            {
                Seconds.Text = "00";
                minutes++;
                if(minutes<10)
                {
                    Minute.Text = "0" + minutes.ToString();
                }
                else
                {
                    Minute.Text = minutes.ToString();
                }
            }
            else
            {
                if (seconds < 10)
                    Seconds.Text = "0" + seconds.ToString();
                else
                    Seconds.Text = seconds.ToString();
            }
        }


        //Record Voice By Microphone

        private async void InitializeAudioRecording()
        {
            _mediaCaptureManager = new MediaCapture();
            var settings = new MediaCaptureInitializationSettings();
            settings.StreamingCaptureMode = StreamingCaptureMode.Audio;
            settings.MediaCategory = MediaCategory.Other;
            settings.AudioProcessing = AudioProcessing.Default;
            await _mediaCaptureManager.InitializeAsync(settings);
        }  

        private async void CaptureAudio(object sender, RoutedEventArgs e)
        {

            //Wave.Visibility = Visibility.Visible;
            //voice.Visibility = Visibility.Collapsed;

            Stop.Visibility = Visibility.Visible;
            try
            {
                String fileName = _audioFileName;

                _recordStorageFile = await KnownFolders.MusicLibrary.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                MediaEncodingProfile recordProfile = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Auto);
                await _mediaCaptureManager.StartRecordToStorageFileAsync(recordProfile, this._recordStorageFile);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to capture audio");
            }
        }

        private async void StopCapture(object sender, RoutedEventArgs e)
        {
            //voice.Visibility = Visibility.Visible;
            piano.Visibility = Visibility.Visible;
            //Wave.Visibility = Visibility.Collapsed;
            Stop.Visibility = Visibility.Collapsed;
            try
            {

                await (_mediaCaptureManager.StopRecordAsync());
            }
            catch (Exception)
            { 
                await (new MessageDialog("You Need To Start A Recording First!").ShowAsync());
            }
        }

        //Record Piano 

        public void StartRec(object sender, RoutedEventArgs e)
        {
            //Wave.Visibility = Visibility.Visible;
            //voice.Visibility = Visibility.Collapsed;
            record = 1;
            circle.Fill = new SolidColorBrush(Colors.Black);
            begin = DateTime.Now;
            piano.Visibility = Visibility.Collapsed;
            Stop.Visibility = Visibility.Visible;
            timerP.Visibility = Visibility.Visible;

            timer.Start();
        }

        public async void StopRec(object sender, RoutedEventArgs e)
        {
            //Wave.Visibility = Visibility.Collapsed;
            //voice.Visibility = Visibility.Visible;
            piano.Visibility = Visibility.Visible;
            Stop.Visibility = Visibility.Collapsed;
            timer.Stop();
            timerP.Visibility = Visibility.Collapsed;
            Seconds.Text = "00";
            Minute.Text = "00";


            string name = ((AppBarButton)sender).Name;
            if (name.Equals("voice"))
            {
                StopCapture(sender, e);
            }
            else
            {

                try
                {
                    end = DateTime.Now;
                    totalRecordingTime = end - begin;
                    mediacomposition.Clips.Add(await MediaClip.CreateFromImageFileAsync
                           (await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Logo.scale-100.png")), totalRecordingTime));

                    var storageFile = await KnownFolders.MusicLibrary.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                    await mediacomposition.RenderToFileAsync(storageFile);

                    diff = new TimeSpan();
                    begin = new DateTime();
                    now = new DateTime();
                    record = 0;
                    mediacomposition = new MediaComposition();
                    //this.Frame.Navigate(typeof(RecordingsPage));
                }
                catch (Exception)
                {
                    await (new MessageDialog("You Need To Start A Recording First!").ShowAsync());

                }
            }
        }

        private async void la_Click(object sender, RoutedEventArgs e)
        {
            //A.Position = new System.TimeSpan(0);
            //A.Play();
            string name = ((Button)sender).Name;
            Uri uri;

            switch (name)
            {
                case "la0u": uri = new Uri("ms-appx:///Assets/FinalNotes/la0.wav"); la0.Position = new System.TimeSpan(0); la0.Play(); break;
                case "la1u": uri = new Uri("ms-appx:///Assets/FinalNotes/la1.wav"); la1.Position = new System.TimeSpan(0); la1.Play(); break;
                case "la2u": uri = new Uri("ms-appx:///Assets/FinalNotes/la2.wav"); la2.Position = new System.TimeSpan(0); la2.Play(); break;
                case "la3u": uri = new Uri("ms-appx:///Assets/FinalNotes/la3.wav"); la3.Position = new System.TimeSpan(0); la3.Play(); break;
                default: uri = new Uri("ms-appx:///Assets/FinalNotes/la0.wav"); la0.Position = new System.TimeSpan(0); la0.Play(); break;

            }

            if (record == 1)
            {
                now = DateTime.Now;
                diff = (now - begin);
                var audio = await StorageFile.GetFileFromApplicationUriAsync(uri);
                track = await BackgroundAudioTrack.CreateFromFileAsync(audio);
                track.Delay = diff;
                mediacomposition.BackgroundAudioTracks.Add(track);
            }
            
            
        }

        private async void si_Click(object sender, RoutedEventArgs e)
        {
            //B.Position = new System.TimeSpan(0);
            //B.Play();

            string name = ((Button)sender).Name;
            Uri uri;

                switch (name)
                {
                    case "si0u": uri = new Uri("ms-appx:///Assets/FinalNotes/si0.wav"); si0.Position = new System.TimeSpan(0); si0.Play(); break;
                    case "si1u": uri = new Uri("ms-appx:///Assets/FinalNotes/si1.wav"); si1.Position = new System.TimeSpan(0); si1.Play(); break;
                    case "si2u": uri = new Uri("ms-appx:///Assets/FinalNotes/si2.wav"); si2.Position = new System.TimeSpan(0); si2.Play(); break;
                    case "si3u": uri = new Uri("ms-appx:///Assets/FinalNotes/si3.wav"); si3.Position = new System.TimeSpan(0); si3.Play(); break;
                    default: uri = new Uri("ms-appx:///Assets/FinalNotes/si0.wav"); si0.Position = new System.TimeSpan(0); si0.Play(); break;

                }


                if (record == 1)
            {
                now = DateTime.Now;
                diff = (now - begin);
                var audio = await StorageFile.GetFileFromApplicationUriAsync(uri);
                track = await BackgroundAudioTrack.CreateFromFileAsync(audio);
                track.Delay = diff;
                mediacomposition.BackgroundAudioTracks.Add(track);
            }
           
        }

        private async void las_Click(object sender, RoutedEventArgs e)
        {
            //AS.Position = new System.TimeSpan(0);
            //AS.Play();
            string name = ((Button)sender).Name;
            Uri uri;

                switch (name)
                {
                    case "las0u": uri = new Uri("ms-appx:///Assets/FinalNotes/las0.wav"); las0.Position = new System.TimeSpan(0); las0.Play(); break;
                    case "las1u": uri = new Uri("ms-appx:///Assets/FinalNotes/las1.wav"); las1.Position = new System.TimeSpan(0); las1.Play(); break;
                    case "las2u": uri = new Uri("ms-appx:///Assets/FinalNotes/las2.wav"); las2.Position = new System.TimeSpan(0); las2.Play(); break;
                    case "las3u": uri = new Uri("ms-appx:///Assets/FinalNotes/las3.wav"); las3.Position = new System.TimeSpan(0); las3.Play(); break;
                    default: uri = new Uri("ms-appx:///Assets/FinalNotes/las0.wav"); las0.Position = new System.TimeSpan(0); las0.Play(); break;

                }

                if (record == 1)
            {
                now = DateTime.Now;
                diff = (now - begin);
                var audio = await StorageFile.GetFileFromApplicationUriAsync(uri);
                track = await BackgroundAudioTrack.CreateFromFileAsync(audio);
                track.Delay = diff;
                mediacomposition.BackgroundAudioTracks.Add(track);
            }
            
        }

        private async void du_Click(object sender, RoutedEventArgs e)
        {
            //C.Position = new System.TimeSpan(0);
            //C.Play();
            string name = ((Button)sender).Name;
            Uri uri;


                switch (name)
                {
                    case "du1u": uri = new Uri("ms-appx:///Assets/FinalNotes/du1.wav"); du1.Position = new System.TimeSpan(0); du1.Play(); break;
                    case "du2u": uri = new Uri("ms-appx:///Assets/FinalNotes/du2.wav"); du2.Position = new System.TimeSpan(0); du2.Play(); break;
                    case "du3u": uri = new Uri("ms-appx:///Assets/FinalNotes/du3.wav"); du3.Position = new System.TimeSpan(0);  du3.Play(); break;
                    default: uri = new Uri("ms-appx:///Assets/FinalNotes/du4.wav"); du4.Position = new System.TimeSpan(0); du4.Play(); break;

                }

                if (record == 1)
            {
                now = DateTime.Now;
                diff = (now - begin);
                var audio = await StorageFile.GetFileFromApplicationUriAsync(uri);
                track = await BackgroundAudioTrack.CreateFromFileAsync(audio);
                track.Delay = diff;
                mediacomposition.BackgroundAudioTracks.Add(track);
            }
           
        }

        private async void ray_Click(object sender, RoutedEventArgs e)
        {
            //D.Position = new System.TimeSpan(0);
            //D.Play();

            string name = ((Button)sender).Name;
            Uri uri;

            switch (name)
            {
                case "ray1u": uri = new Uri("ms-appx:///Assets/FinalNotes/ray1.wav"); ray1.Position = new System.TimeSpan(0); ray1.Play(); break;
                case "ray2u": uri = new Uri("ms-appx:///Assets/FinalNotes/ray2.wav"); ray2.Position = new System.TimeSpan(0); ray2.Play(); break;
                case "ray3u": uri = new Uri("ms-appx:///Assets/FinalNotes/ray3.wav"); ray3.Position = new System.TimeSpan(0); ray3.Play(); break;
                case "ray4u": uri = new Uri("ms-appx:///Assets/FinalNotes/ray4.wav"); ray4.Position = new System.TimeSpan(0); ray4.Play(); break;

                default: uri = new Uri("ms-appx:///Assets/FinalNotes/ray1.wav"); ray1.Position = new System.TimeSpan(0); ray1.Play(); break;

            }

            if (record == 1)
            {
                now = DateTime.Now;
                diff = (now - begin);
                var audio = await StorageFile.GetFileFromApplicationUriAsync(uri);
                track = await BackgroundAudioTrack.CreateFromFileAsync(audio);
                track.Delay = diff;
                mediacomposition.BackgroundAudioTracks.Add(track);
               
            }
           
        }

        private async void dus_Click(object sender, RoutedEventArgs e)
        {
            //CS.Position = new System.TimeSpan(0);
            //CS.Play();

            string name = ((Button)sender).Name;
            Uri uri;

            switch (name)
            {
                case "dus1u": uri = new Uri("ms-appx:///Assets/FinalNotes/dus1.wav"); dus1.Position = new System.TimeSpan(0); dus1.Play(); break;
                case "dus2u": uri = new Uri("ms-appx:///Assets/FinalNotes/dsu2.wav"); dus2.Position = new System.TimeSpan(0); dus2.Play(); break;
                case "dus3u": uri = new Uri("ms-appx:///Assets/FinalNotes/dus3.wav"); dus3.Position = new System.TimeSpan(0); dus3.Play(); break;
                case "dus4u": uri = new Uri("ms-appx:///Assets/FinalNotes/dus4.wav"); dus4.Position = new System.TimeSpan(0); dus4.Play(); break;
                default: uri = new Uri("ms-appx:///Assets/FinalNotes/dus1.wav"); dus1.Position = new System.TimeSpan(0); dus1.Play(); break;

            }

            if (record == 1)
            {
                now = DateTime.Now;
                diff = (now - begin);
                var audio = await StorageFile.GetFileFromApplicationUriAsync(uri);
                track = await BackgroundAudioTrack.CreateFromFileAsync(audio);
                track.Delay = diff;
                mediacomposition.BackgroundAudioTracks.Add(track);
            }
            
        }

        private async void rays_Click(object sender, RoutedEventArgs e)
        {
            //DS.Position = new System.TimeSpan(0);
            //DS.Play();

            string name = ((Button)sender).Name;
            Uri uri;

            switch (name)
            {
                case "rays1u": uri = new Uri("ms-appx:///Assets/FinalNotes/rays1.wav"); rays1.Position = new System.TimeSpan(0); rays1.Play(); break;
                case "rays2u": uri = new Uri("ms-appx:///Assets/FinalNotes/rays2.wav"); rays2.Position = new System.TimeSpan(0); rays2.Play(); break;
                case "rays3u": uri = new Uri("ms-appx:///Assets/FinalNotes/rays3.wav"); rays3.Position = new System.TimeSpan(0); rays3.Play(); break;
                case "rays4u": uri = new Uri("ms-appx:///Assets/FinalNotes/rays4.wav"); rays4.Position = new System.TimeSpan(0); rays4.Play(); break;
                default: uri = new Uri("ms-appx:///Assets/FinalNotes/rays1.wav"); rays1.Position = new System.TimeSpan(0); rays1.Play(); break;

            }


            if (record == 1)
            {
                now = DateTime.Now;
                diff = (now - begin);
                var audio = await StorageFile.GetFileFromApplicationUriAsync(uri);
                track = await BackgroundAudioTrack.CreateFromFileAsync(audio);
                track.Delay = diff;
                mediacomposition.BackgroundAudioTracks.Add(track);
            }
          
        }

        private async void fa_Click(object sender, RoutedEventArgs e)
        {
            //F.Position = new System.TimeSpan(0);
            //F.Play();

            string name = ((Button)sender).Name;
            Uri uri;

            switch (name)
            {
                case "fa1u": uri = new Uri("ms-appx:///Assets/FinalNotes/fa1.wav"); fa1.Position = new System.TimeSpan(0); fa1.Play(); break;
                case "fa2u": uri = new Uri("ms-appx:///Assets/FinalNotes/fa2.wav"); fa2.Position = new System.TimeSpan(0); fa2.Play(); break;
                case "fa3u": uri = new Uri("ms-appx:///Assets/FinalNotes/fa3.wav"); fa3.Position = new System.TimeSpan(0); fa3.Play(); break;
                default: uri = new Uri("ms-appx:///Assets/FinalNotes/fa1.wav"); fa1.Position = new System.TimeSpan(0); fa1.Play(); break;

            }


            if (record == 1)
            {
                now = DateTime.Now;
                diff = (now - begin);
                var audio = await StorageFile.GetFileFromApplicationUriAsync(uri);
                track = await BackgroundAudioTrack.CreateFromFileAsync(audio);
                track.Delay = diff;
                mediacomposition.BackgroundAudioTracks.Add(track);
            }
           
        }

        private async void me_Click(object sender, RoutedEventArgs e)
        {
            //E.Position = new System.TimeSpan(0);
            //E.Play();

            string name = ((Button)sender).Name;
            Uri uri;

            switch (name)
            {
                case "me1u": uri = new Uri("ms-appx:///Assets/FinalNotes/mi1.wav"); mi1.Position = new System.TimeSpan(0); mi1.Play(); break;
                case "me2u": uri = new Uri("ms-appx:///Assets/FinalNotes/mi2.wav"); mi2.Position = new System.TimeSpan(0); mi2.Play(); break;
                case "me3u": uri = new Uri("ms-appx:///Assets/FinalNotes/mi3.wav"); mi3.Position = new System.TimeSpan(0); mi3.Play(); break;
                default: uri = new Uri("ms-appx:///Assets/FinalNotes/mi1.wav"); mi1.Position = new System.TimeSpan(0); mi1.Play(); break;

            }


            if (record == 1)
            {
                now = DateTime.Now;
                diff = (now - begin);
                var audio = await StorageFile.GetFileFromApplicationUriAsync(uri);
                track = await BackgroundAudioTrack.CreateFromFileAsync(audio);
                track.Delay = diff;
                mediacomposition.BackgroundAudioTracks.Add(track);
            }
           
        }

        private async void fas_Click(object sender, RoutedEventArgs e)
        {
            //FS.Position = new System.TimeSpan(0);
            //FS.Play();

            string name = ((Button)sender).Name;
            Uri uri;

            switch (name)
            {
                case "fas1u": uri = new Uri("ms-appx:///Assets/FinalNotes/fas1.wav"); fas1.Position = new System.TimeSpan(0); ; fas1.Play(); break;
                case "fas2u": uri = new Uri("ms-appx:///Assets/FinalNotes/fas2.wav"); fas2.Position = new System.TimeSpan(0); fas2.Play(); break;
                case "fas3u": uri = new Uri("ms-appx:///Assets/FinalNotes/fas3.wav"); fas3.Position = new System.TimeSpan(0); fas3.Play(); break;
                default: uri = new Uri("ms-appx:///Assets/FinalNotes/fas1.wav"); fas1.Position = new System.TimeSpan(0); fas1.Play(); break;

            }

            if (record == 1)
            {
                now = DateTime.Now;
                diff = (now - begin);
                var audio = await StorageFile.GetFileFromApplicationUriAsync(uri);
                track = await BackgroundAudioTrack.CreateFromFileAsync(audio);
                track.Delay = diff;
                mediacomposition.BackgroundAudioTracks.Add(track);
            }
            
        }

        private async void so_Click(object sender, RoutedEventArgs e)
        {
            //G.Position = new System.TimeSpan(0);
            //G.Play();

            string name = ((Button)sender).Name;
            Uri uri;

            switch (name)
            {

                case "so1u": uri = new Uri("ms-appx:///Assets/FinalNotes/so1.wav"); so1.Position = new System.TimeSpan(0); so1.Play(); break;
                case "so2u": uri = new Uri("ms-appx:///Assets/FinalNotes/so2.wav"); so2.Position = new System.TimeSpan(0); so2.Play(); break;
                case "so3u": uri = new Uri("ms-appx:///Assets/FinalNotes/so3.wav"); so3.Position = new System.TimeSpan(0); so3.Play(); break;
                default: uri = new Uri("ms-appx:///Assets/FinalNotes/so1.wav"); so1.Position = new System.TimeSpan(0); so1.Play(); break;

            }


            if (record == 1)
            {
                now = DateTime.Now;
                diff = (now - begin);
                var audio = await StorageFile.GetFileFromApplicationUriAsync(uri);
                track = await BackgroundAudioTrack.CreateFromFileAsync(audio);
                track.Delay = diff;
                mediacomposition.BackgroundAudioTracks.Add(track);
            }
            
        }

        private async void sols_Click(object sender, RoutedEventArgs e)
        {
            //GS.Position = new System.TimeSpan(0);
            //GS.Play();

            string name = ((Button)sender).Name;
            Uri uri;

            switch (name)
            {

                case "sos1u": uri = new Uri("ms-appx:///Assets/FinalNotes/sos1.wav"); sos1.Position = new System.TimeSpan(0); sos1.Play(); break;
                case "sos2u": uri = new Uri("ms-appx:///Assets/FinalNotes/sos2.wav"); sos2.Position = new System.TimeSpan(0); sos2.Play(); break;
                case "sos3u": uri = new Uri("ms-appx:///Assets/FinalNotes/sos3.wav"); sos3.Position = new System.TimeSpan(0); sos3.Play(); break;
                default: uri = new Uri("ms-appx:///Assets/FinalNotes/sos1.wav"); sos1.Position = new System.TimeSpan(0); sos1.Play(); break;

            }


            if (record == 1)
            {
                now = DateTime.Now;
                diff = (now - begin);
                var audio = await StorageFile.GetFileFromApplicationUriAsync(uri);
                track = await BackgroundAudioTrack.CreateFromFileAsync(audio);
                track.Delay = diff;
                mediacomposition.BackgroundAudioTracks.Add(track);
            }
            
        }


        private void RSS_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RSSPage));
        }

        private void Recordings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RecordingsPage));
        }

    }

}
