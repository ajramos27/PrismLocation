using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LocationPrism.Services;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(LocationPrism.Droid.Services.LocationService))]
namespace LocationPrism.Droid.Services
{
    [Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeLocation) ]
    public class LocationService : Service, ILocationService
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if(intent.Action == "START_SERVICE")
            {
                System.Diagnostics.Debug.WriteLine("Starting");
                RegisterNotification();

                var startTimeSpan = TimeSpan.Zero;
                var periodTimeSpan = TimeSpan.FromSeconds(10);

                var timer = new System.Threading.Timer(async (e) =>
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    var position = await Geolocation.GetLocationAsync(request);
                    Console.WriteLine(position.Longitude + "," + position.Latitude);

                }, null, startTimeSpan, periodTimeSpan);

            } else if (intent.Action == "STOP_SERVICE")
            {
                System.Diagnostics.Debug.WriteLine("Stopping");
                StopForeground(true);
                StopSelfResult(startId);

            }

            return StartCommandResult.NotSticky;
        }

        public void Start()
        {
            Intent startService = new Intent(MainActivity.ActivityCurrent, typeof(LocationService));
            startService.SetAction("START_SERVICE");
            MainActivity.ActivityCurrent.StartService(startService);
        }

        public void Stop()
        {
            Intent stopIntent = new Intent(MainActivity.ActivityCurrent, this.Class);
            stopIntent.SetAction("STOP_SERVICE");
            MainActivity.ActivityCurrent.StartService(stopIntent);
        }

        void RegisterNotification()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            NotificationChannel channel = new NotificationChannel("ServiceChannel", "Location Service", NotificationImportance.Max);
            NotificationManager manager = (NotificationManager)MainActivity.ActivityCurrent.GetSystemService(Context.NotificationService);

            manager.CreateNotificationChannel(channel);

            //var intent = new Intent(this, typeof(MainActivity));
            //intent.AddFlags(ActivityFlags.ClearTop);
            //var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent);
            //.SetContentIntent(pendingIntent)

            Notification notification = new Notification.Builder(this, "ServiceChannel")
                .SetContentTitle("Trip in progress")
                .SetContentText("HaulerHub is tracking your location")
                .SetSmallIcon(Resource.Drawable.abc_ic_star_black_16dp)
                .SetOngoing(true)
                .Build();

            StartForeground(100, notification);
        }

    }
}