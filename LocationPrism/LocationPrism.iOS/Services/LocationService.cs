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

namespace LocationPrism.iOS.Services
{
    class LocationService : ILocationService
    {
        public void Start()
        {
            StartListening();
            //StartTracking();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        async void StartListening()
        {
            if (CrossGeolocator.Current.IsListening)
                return;

            ///This logic will run on the background automatically on iOS, however for Android and UWP you must put logic in background services. Else if your app is killed the location updates will be killed.
            await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(10), 10, true, new Plugin.Geolocator.Abstractions.ListenerSettings
            {
                ActivityType = ActivityType.AutomotiveNavigation,
                AllowBackgroundUpdates = true,
                DeferLocationUpdates = true,
                DeferralDistanceMeters = 1,
                DeferralTime = TimeSpan.FromSeconds(10),
                ListenForSignificantChanges = false,
                PauseLocationUpdatesAutomatically = false
            });

            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
        }

        private void Current_PositionChanged(object sender, PositionEventArgs e)
        {
            Console.WriteLine(e.Position.Latitude + "," + e.Position.Longitude);
        }
    }
}