using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceOfNepal2.Models;
using VoiceOfNepal2.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VoiceOfNepal2.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            BindingContext = new HomePageViewModel();


        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as YouTubeItem;
            Navigation.PushAsync(new DetailsPage(item));
           

        }

    }
}