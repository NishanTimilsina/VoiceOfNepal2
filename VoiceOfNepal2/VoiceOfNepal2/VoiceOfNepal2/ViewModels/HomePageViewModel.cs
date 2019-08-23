using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FreshMvvm;
using Newtonsoft.Json;
using VoiceOfNepal2.Models;
using Microsoft.AppCenter.Crashes;

namespace VoiceOfNepal2.ViewModels
{
    class HomePageViewModel : BaseViewModel
    {
        // Get your API Key @ https://console.developers.google.com/apis/api/youtube/
        const string ApiKey = "AIzaSyB2Hu4D97zOB8f410cwT2rCc6JnmwoLCAo";
        const string ChannelId = "UCUjXcNjOpNXhamNRcuEikUA";
        const string PlaylistId = "PLNkuHnYa5Xt5F8JjLaqB89HYkT5sb4vIw";
        private string adUnitId;

        public string AdUnitId
        {
            get { return adUnitId; }
            set { adUnitId = value;
                OnPropertyChanged("AdUnitId");
            }
        }

        // Documentation @ https://developers.google.com/apis-explorer/#p/youtube/v3/youtube.videos.list 
        string channelUrl = $"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=50&channelId={ChannelId}&key={ApiKey}&playlistId={PlaylistId}";
        string detailsUrl = "https://www.googleapis.com/youtube/v3/videos?part=snippet,statistics&key=" + ApiKey + "&id={0}";

        public HomePageViewModel()
        {
            Task.Factory.StartNew(GetPlayListVideos);
#if DEBUG
            AdUnitId = "ca-app-pub-3940256099942544/6300978111";  //test
#else
            AdUnitId = "ca-app-pub-3666786384636527/3415399626";  //prod
#endif
        }
        private ObservableCollection<YouTubeItem> _Items;
        public  ObservableCollection<YouTubeItem> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }


        async Task GetChannelData()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    Items = new ObservableCollection<YouTubeItem>();

                    var videoIds = new List<string>();
                    var json = await httpClient.GetStringAsync(channelUrl);

                    // Deserialize our data, this is in a simple List format
                    var response = JsonConvert.DeserializeObject<YouTubeApiListRoot>(json);

                    // Add all the video id's we've found to our list.
                    videoIds.AddRange(response.items.Select(item => item.id));

                    // Get the details for all our items
                     Items = await GetVideoDetailsAsync(videoIds);
                    
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        async Task GetPlayListVideos()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    Items = new ObservableCollection<YouTubeItem>();
                    var youtubeItems = new ObservableCollection<YouTubeItem>();
                    var videoIds = new List<string>();
                    var json = await httpClient.GetStringAsync(channelUrl);

                    // Deserialize our data, this is in a simple List format
                    var response = JsonConvert.DeserializeObject<YouTubeApiDetailsRoot>(json);

                    foreach (var item in response.items)
                    {
                        var youTubeItem = new YouTubeItem()
                        {
                            Title = item.snippet.title,
                            Description = item.snippet.description,
                            ChannelTitle = item.snippet.channelTitle,
                            PublishedAt = item.snippet.publishedAt,
                            DefaultThumbnailUrl = item.snippet?.thumbnails?.@default?.url,
                            MediumThumbnailUrl = item.snippet?.thumbnails?.medium?.url,
                            HighThumbnailUrl = item.snippet?.thumbnails?.high?.url,
                            StandardThumbnailUrl = item.snippet?.thumbnails?.standard?.url,
                            MaxResThumbnailUrl = item.snippet?.thumbnails?.maxres?.url,
                            ViewCount = item.statistics?.viewCount,
                            LikeCount = item.statistics?.likeCount,
                            DislikeCount = item.statistics?.dislikeCount,
                            FavoriteCount = item.statistics?.favoriteCount,
                            CommentCount = item.statistics?.commentCount,
                            Tags = item.snippet?.tags,
                            videoId = item.snippet?.resourceId?.videoId

                        };


                        youtubeItems.Add(youTubeItem);
                    }
                    var res = youtubeItems.ToList().OrderBy(n => n.PublishedAt);
                    Items = new ObservableCollection<YouTubeItem>(res);
                }


            
            }
            catch (Exception ex)
            {
                var ms = ex;
            }
        }

        async Task<ObservableCollection<YouTubeItem>> GetVideoDetailsAsync(List<string> videoIds)
        {
            try
            {
                var videoIdString = string.Join(",", videoIds);
                var youtubeItems = new ObservableCollection<YouTubeItem>();

                using (var httpClient = new HttpClient())
                {
                    string url = string.Format(detailsUrl, videoIdString);
                    var json = await httpClient.GetStringAsync(url);
                    var response = JsonConvert.DeserializeObject<YouTubeApiDetailsRoot>(json);

                    foreach (var item in response.items)
                    {
                        var youTubeItem = new YouTubeItem()
                        {
                            Title = item.snippet.title,
                            Description = item.snippet.description,
                            ChannelTitle = item.snippet.channelTitle,
                            PublishedAt = item.snippet.publishedAt,
                            VideoId = item.id,
                            DefaultThumbnailUrl = item.snippet?.thumbnails?.@default?.url,
                            MediumThumbnailUrl = item.snippet?.thumbnails?.medium?.url,
                            HighThumbnailUrl = item.snippet?.thumbnails?.high?.url,
                            StandardThumbnailUrl = item.snippet?.thumbnails?.standard?.url,
                            MaxResThumbnailUrl = item.snippet?.thumbnails?.maxres?.url,
                            ViewCount = item.statistics?.viewCount,
                            LikeCount = item.statistics?.likeCount,
                            DislikeCount = item.statistics?.dislikeCount,
                            FavoriteCount = item.statistics?.favoriteCount,
                            CommentCount = item.statistics?.commentCount,
                            Tags = item.snippet?.tags
                        };

                        
                            youtubeItems.Add(youTubeItem);
                    }
                }
                var res = youtubeItems.ToList().OrderBy(n => n.PublishedAt);
                return new ObservableCollection<YouTubeItem>(res);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return new ObservableCollection<YouTubeItem>();
            }
        }
    }
}
