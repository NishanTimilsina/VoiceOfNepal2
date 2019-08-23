using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using VoiceOfNepal2.Models;
using Microsoft.AppCenter.Crashes;

namespace VoiceOfNepal2.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            MenuPages.Add((int)MenuItemType.Browse, (NavigationPage)Detail);
        }

        public async Task NavigateFromMenu(int id)
        {
            try
            {
                if (!MenuPages.ContainsKey(id))
                {
                    switch (id)
                    {
                        case (int)MenuItemType.Browse:
                            MenuPages.Add(id, new NavigationPage(new HomePage()));
                            break;
                        case (int)MenuItemType.About:
                            MenuPages.Add(id, new NavigationPage(new AboutPage()) { BarBackgroundColor = Color.FromHex("#7b0002"), BarTextColor = Color.White }); ;
                            break;
                    }
                }

                var newPage = MenuPages[id];

                if (newPage != null && Detail != newPage)
                {
                    Detail = newPage;

                    if (Device.RuntimePlatform == Device.Android)
                        await Task.Delay(100);

                    IsPresented = false;
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
            
        }
    }
}