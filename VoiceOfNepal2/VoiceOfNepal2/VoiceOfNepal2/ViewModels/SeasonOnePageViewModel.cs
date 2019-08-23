using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VoiceOfNepal2.Models;

namespace VoiceOfNepal2.ViewModels
{
    class SeasonOnePageViewModel : BaseViewModel
    {
        const string ApiKey = "AIzaSyB2Hu4D97zOB8f410cwT2rCc6JnmwoLCAo";
        const string ChannelId = "UCUjXcNjOpNXhamNRcuEikUA";
        const string PlaylistId = "PLNkuHnYa5Xt4banLPuO-KLe9rTO0fJn5t";
        string channelUrl = $"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=50&channelId={ChannelId}&key={ApiKey}&playlistId={PlaylistId}";

        private ObservableCollection<YouTubeItem> _Items;
        public ObservableCollection<YouTubeItem> Items
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

        public SeasonOnePageViewModel()
        {
            Task.Factory.StartNew(GetPlayListVideos);
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
                Crashes.TrackError(ex);

            }
        }

    }

}
