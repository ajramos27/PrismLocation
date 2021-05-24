using CoreLocation;
using LocationPrism.Models;
using LocationPrism.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace LocationPrism.iOS.Services
{
    class LocationService : ILocationService
    {
        private CLLocationManager locationManager;

        public CLLocationManager LocMgr
        {
            get { return locationManager; }
        }

        //SERVICE FUNCTIONS

        public void Start(int interval)
        {

            locationManager = new CLLocationManager();
            locationManager.PausesLocationUpdatesAutomatically = false;

            locationManager.RequestAlwaysAuthorization();
            locationManager.AllowsBackgroundLocationUpdates = true;

            if (CLLocationManager.LocationServicesEnabled)
            {

                //set the desired accuracy, in meters
                locationManager.DesiredAccuracy = CLLocation.AccurracyBestForNavigation;
                locationManager.DistanceFilter = CLLocationDistance.FilterNone;


                var locationDelegate = new MyLocationDelegate();
                locationManager.Delegate = locationDelegate;
                locationManager.AllowDeferredLocationUpdatesUntil(CLLocationDistance.MaxDistance, interval);

                locationManager.StartUpdatingLocation();
                locationManager.StartMonitoringSignificantLocationChanges();
                //locationDelegate.StartTimer(interval);
            }
        }

        public void Stop()
        {
            locationManager.StopUpdatingLocation();
        }

        public void ChangeInterval(int interval)
        {
            if(locationManager!= null)
            {
                ((MyLocationDelegate)locationManager.Delegate).ChangeInterval(interval);
            }
        }

        public async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission)
            where T : BasePermission
        {
            var status = await permission.CheckStatusAsync();
            if (status != PermissionStatus.Granted)
            {
                status = await permission.RequestAsync();
            }

            return status;
        }        
    }

    public class MyLocationDelegate : CLLocationManagerDelegate
    {
        private CLLocation lastLoc;
        private int interval = Timeout.Infinite;
        private Timer timer;

        public override void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
        {

            var lastLocation = (locations[locations.Length - 1]);
            Console.WriteLine(lastLocation.Coordinate.Latitude + "," + lastLocation.Coordinate.Longitude);
            lastLoc = lastLocation;
            var location = new Position(lastLoc.Coordinate.Latitude, lastLoc.Coordinate.Longitude);
            MessagingCenter.Send(location, "LocationUpdate");
        }


        public void StartTimer(int interval)
        {
            this.interval = interval;
            var startTimeSpan = TimeSpan.FromSeconds(interval);
            var periodTimeSpan = TimeSpan.FromSeconds(interval);

            timer = new Timer(async (e) =>
            {
                await Task.Run(() =>
                {
                    Console.WriteLine("Current is: " + lastLoc.Coordinate.Latitude + "," + lastLoc.Coordinate.Longitude);
                    var location = new Position(lastLoc.Coordinate.Latitude, lastLoc.Coordinate.Longitude);
                    MessagingCenter.Send(location, "LocationUpdate");
                });

            }, null, startTimeSpan, periodTimeSpan);
        }

        public void ChangeInterval(int interval)
        {
            this.interval = interval;
            timer.Change(TimeSpan.FromSeconds(interval), TimeSpan.FromSeconds(interval));
        }

    }

}