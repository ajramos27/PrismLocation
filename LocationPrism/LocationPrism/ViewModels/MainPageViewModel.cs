using LocationPrism.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LocationPrism.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private ILocationService _locationService;
        private IApiService _apiService;

        public DelegateCommand StartService { get; private set; }
        public DelegateCommand StopService { get; private set; }

        public MainPageViewModel(INavigationService navigationService, ILocationService locationService, IApiService apiService )
            : base(navigationService)
        {
            Title = "Main Page";

            _locationService = locationService;
            _apiService = apiService;

            StartService = new DelegateCommand(Start);
            StopService = new DelegateCommand(Stop);

        }

        public async override void Initialize(INavigationParameters parameters)
        {
            await CheckAndRequestLocationPermission();
        }

        public async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.LocationAlways>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.LocationAlways>();

            return status;
        }

        private void Stop()
        {
            DependencyService.Get<ILocationService>().Stop();
        }

        private async void Start()
        {
            //DependencyService.Get<ILocationService>().Start();
            //await StartListening();

            MessagingCenter.Subscribe<Object, Location>(this, "LocationUpdate", (sender, loc) =>
                {
                    _apiService.Hello();
                    Console.WriteLine("Messaging:" + loc.Latitude + "," + loc.Longitude);
                });

            _locationService.Start();
        }

        /*
        async Task StartListening()
        {
            if (CrossGeolocator.Current.IsListening)
                return;

            ///This logic will run on the background automatically on iOS, however for Android and UWP you must put logic in background services. Else if your app is killed the location updates will be killed.
            await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(1), 10, true, new Plugin.Geolocator.Abstractions.ListenerSettings
            {
                ActivityType = Plugin.Geolocator.Abstractions.ActivityType.AutomotiveNavigation,
                AllowBackgroundUpdates = true,
                DeferLocationUpdates = true,
                DeferralDistanceMeters = 1,
                DeferralTime = TimeSpan.FromSeconds(1),
                ListenForSignificantChanges = false,
                PauseLocationUpdatesAutomatically = false
            });

            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;

        }

        private void Current_PositionChanged(object sender, PositionEventArgs e)
        {
            Console.WriteLine(e.Position.Latitude + "," + e.Position.Longitude);
        }*/
    }
}
