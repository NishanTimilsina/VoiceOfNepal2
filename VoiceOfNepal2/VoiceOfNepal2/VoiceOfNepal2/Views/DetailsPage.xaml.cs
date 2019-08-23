using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceOfNepal2.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VoiceOfNepal2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage(YouTubeItem item)
        {
            InitializeComponent();
            var url = "https://www.youtube.com/watch?v=" + item.videoId;
            loadVideo_wv.Source = url;

        }
    }
}