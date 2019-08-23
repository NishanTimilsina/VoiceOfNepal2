using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Ads;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VoiceOfNepal2.Droid
{
    [Activity(Label = "VoiceOfNepal2", Icon = "@mipmap/logo", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Android.Gms.Ads.MobileAds.Initialize(ApplicationContext, "ca-app-pub-3666786384636527~1577017426");

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            base.SetTheme(Resource.Style.splashscreen);

            //MobileAds.Initialize(ApplicationContext, "ca-app-pub-3666786384636527~1577017426");

            //apcenter configuration
            AppCenter.Start("91971a83-a5d4-4c6e-8ec2-2ca2ae85e4d2",
                   typeof(Analytics), typeof(Crashes));
            Task.Run(async () => { await LogCrashReport(); });
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        private async Task LogCrashReport()
        {
            bool didAppCrash = await Crashes.HasCrashedInLastSessionAsync();
            if (didAppCrash)
            {
                var crashReport = await Crashes.GetLastSessionCrashReportAsync();
                var errorText = JsonConvert.SerializeObject(crashReport);
                var ex = new Exception() { Source = errorText };
                Crashes.TrackError(ex);
            }
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", e.ExceptionObject as Exception);
            Crashes.TrackError(newExc);

        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", e.Exception);
            e.SetObserved();
            Crashes.TrackError(newExc);

        }

        internal static void LogUnhandledException(Exception ex)
        {
            Crashes.TrackError(ex);
        }
    }
}