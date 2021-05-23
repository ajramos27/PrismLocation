using CoreLocation;
using Foundation;
using LocationPrism.Services;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace LocationPrism.iOS.Services
{
    class LocationService : ILocationService
    {
        public async void Start()
        {

            var status = await CheckAndRequestPermissionAsync(new Permissions.LocationAlways());
            if (status != PermissionStatus.Granted)
            {
                // Notify user permission was denied
                return;
            }
            else
            {
                StartListening();
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

        public void Stop()
        {
            throw new NotImplementedException();
        }

        async void StartListening()
        { 

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(1);

            var timer = new System.Threading.Timer(async (e) =>
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var position = await Geolocation.GetLocationAsync(request);
                Console.WriteLine(position.Longitude + "," + position.Latitude);
                MessagingCenter.Send<Object, Location>(this, "LocationUpdate", position);

            }, null, startTimeSpan, periodTimeSpan);
        }
    }
}