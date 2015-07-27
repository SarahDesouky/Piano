using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Web.Http;

namespace Piano
{
    class MainViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        List<rssChannelItem> _rssPosts;
    //    List<thumbnail> _images;
    //    public List<thumbnail> images
    //    {

    //        get { return _images }
    //         set
    //        {
    //            _images = value;
    //            if (PropertyChanged != null)
    //            {
    //                PropertyChanged(this, new PropertyChangedEventArgs("images"));
    //            }
    //        }
    //}
        public List<rssChannelItem> rssPosts
        {
            get { return _rssPosts; }
            set
            {
                _rssPosts = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("rssPosts"));
                }
            }
        }
        public MainViewModel()
        {
            GetRSSFeed();
            //getPianoRss();
        }

        string entries;
        //string posts;
        rss posts;
        XmlSerializer xml;
        async void GetRSSFeed()
        {
           

            HttpClient Client = new HttpClient();
            entries = await Client.GetStringAsync(new Uri("http://www.bbc.co.uk/feeds/rss/music/latest_releases.xml"));
            xml = new XmlSerializer(typeof(rss));
            posts = (rss)xml.Deserialize(new StringReader(entries));
            rssPosts = new List<rssChannelItem>(posts.channel.item);

        }

        async void getPianoRss()
        {

            HttpClient Client = new HttpClient();
            entries = await Client.GetStringAsync(new Uri("http://www.pianoworks.com/rss/showallblogs.aspx"));
            xml = new XmlSerializer(typeof(rss));
            posts = (rss)xml.Deserialize(new StringReader(entries));
            rssPosts = new List<rssChannelItem>(posts.channel.item);

        }


    }
}
