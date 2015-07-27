using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Piano
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecordingsPage : Page
    {
        List<Recording> recordings;
        public RecordingsPage()
        {
            this.InitializeComponent();
            recordings = new List<Recording>();
            getFiles();
        }

        List<string> paths = new List<string>();
        
        

        private async void getFiles()
        {
            try
            {
                StorageFolder library = Windows.Storage.KnownFolders.MusicLibrary;
                IReadOnlyList<StorageFile> files = await library.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByDate);
                for (int i = 0; i < files.Count; i++)
                {
                    // do something with the name of each file
                    string path = files[i].Path;
                    string date = files[i].DateCreated.ToString();
                    Recording rec = new Recording(files[i].Name, files[i].Path, date);
                    recordings.Add(rec);
                    paths.Add(path);
                }
                view.ItemsSource = recordings;
            }
            catch (Exception e) {
                Debug.WriteLine("Getting files error1");
            }

        }

        string x;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Button b = (Button)sender;
            string name = b.Content.ToString();

            var file = await KnownFolders.MusicLibrary.GetFileAsync(name);
            try { 
              
    
            var stream = await file.OpenAsync(FileAccessMode.Read);
                media.SetSource(stream, file.FileType);
                media.Play();
            }
            catch(Exception s)
            {
                Debug.WriteLine(s.ToString());
            }
         

        }

        
    }

    public partial class Recording
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Date { get; set; }

        public Recording(string name, string path, string date)
        {
            this.Name = name;
            this.Path = path;
            this.Date = date;
        }
       
    }
}
