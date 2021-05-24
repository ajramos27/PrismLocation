using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using LocationPrism.Models;
using LocationPrism.Services;
using System;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(LocationPrism.Droid.Services.LocationService))]
namespace LocationPrism.Droid.Services
{
    [Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeLocation) ]
    public class LocationService : Service, ILocationService
    {

        private int interval = Timeout.Infinite;
        private Timer timer;

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
                StartTimer(intent.GetIntExtra("timer", 0));

            } else if (intent.Action == "STOP_SERVICE")
            {
                System.Diagnostics.Debug.WriteLine("Stopping");
                StopTimer();
                StopForeground(true);
                StopSelfResult(startId);

            } else if (intent.Action == "UPDATE_SERVICE")
            {
                System.Diagnostics.Debug.WriteLine("Update");
                UpdateTimer(intent.GetIntExtra("timer", 0));
            }

            return StartCommandResult.NotSticky;
        }

        public void Start(int interval)
        {
            Intent startService = new Intent(MainActivity.ActivityCurrent, typeof(LocationService));
            startService.PutExtra("timer", interval);
            startService.SetAction("START_SERVICE");
            MainActivity.ActivityCurrent.StartService(startService);
        }

        public void Stop()
        {
            Intent stopIntent = new Intent(MainActivity.ActivityCurrent, this.Class);
            stopIntent.SetAction("STOP_SERVICE");
            MainActivity.ActivityCurrent.StartService(stopIntent);
        }

        public void ChangeInterval(int interval)
        {
            Intent updateIntent = new Intent(MainActivity.ActivityCurrent, this.Class);
            updateIntent.PutExtra("timer", interval);
            updateIntent.SetAction("UPDATE_SERVICE");
            MainActivity.ActivityCurrent.StartService(updateIntent);
        }

        public void StartTimer(int updateTime)
        {         
            var startTime = TimeSpan.Zero;
            var periodTime = TimeSpan.FromSeconds(updateTime);

            this.interval = updateTime;

            timer = new Timer(async (e) =>
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Default);
                var position = await Geolocation.GetLocationAsync(request);
                Console.WriteLine("Current is: " + position.Longitude + "," + position.Latitude);
                var location = new Position(position.Latitude, position.Longitude);
                MessagingCenter.Send(location, "LocationUpdate");
                

            }, null, startTime, periodTime);
        }
      
        public void StopTimer()
        {
            interval = Timeout.Infinite;
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void UpdateTimer(int interval)
        {
            this.interval = interval;
            timer.Change(TimeSpan.FromSeconds(interval), TimeSpan.FromSeconds(interval));
        }

        private void RegisterNotification()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            NotificationChannel channel = new NotificationChannel("ServiceChannel", "Location Service", NotificationImportance.Max);
            NotificationManager manager = (NotificationManager)MainActivity.ActivityCurrent.GetSystemService(Context.NotificationService);

            manager.CreateNotificationChannel(channel);

            Notification notification = new Notification.Builder(this, "ServiceChannel")
                .SetContentTitle("Trip in progress")
                .SetContentText("HaulerHub is tracking your location")
                .SetSmallIcon(Resource.Drawable.abc_btn_radio_to_on_mtrl_015)
                .SetOngoing(true)
                .Build();

            StartForeground(100, notification);
        }

    }
}