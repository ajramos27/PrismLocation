using LocationPrism.Models;
using LocationPrism.Repositories;
using LocationPrism.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private IPositionRepository _positionRepository;
        private IApiService _apiService;

        public ObservableCollection<Position> _locations { get; set; }
        public ObservableCollection<Position> Locations
        {
            get { return _locations; }
            set
            {
                _locations = value;
                RaisePropertyChanged("Locations");
            }
        }

        public DelegateCommand StartService { get; private set; }
        public DelegateCommand StopService { get; private set; }

        public MainPageViewModel(INavigationService navigationService, ILocationService locationService, IApiService apiService, IPositionRepository positionRepository )
            : base(navigationService)
        {
            Title = "Main Page";

            _locationService = locationService;
            _apiService = apiService;
            _positionRepository = positionRepository;

            StartService = new DelegateCommand(Start);
            StopService = new DelegateCommand(Stop);

            Locations = new ObservableCollection<Position>();

        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            await CheckAndRequestLocationPermission();
            List<Position> list = await _positionRepository.GetAll();

            //await _positionRepository.Clear();
            list.ForEach(l => Locations.Add(l));

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

        private void Start()
        {
            var timer = 10;           
            _locationService.Start(timer);
            MessagingCenter.Subscribe<Position>(this, "LocationUpdate", async (location) =>
            {
                Console.WriteLine("Hello from shared: " + location.Latitude + ", "+ location.Longitude);
                await _apiService.UpdateLocation(location);
                int newTimer = 20;
                if (newTimer != timer)
                {
                    _locationService.ChangeInterval(newTimer);
                    timer = newTimer;
                }
            });
        }
        
        private void Stop()
        {
            _locationService.Stop();
        }
    }
}
