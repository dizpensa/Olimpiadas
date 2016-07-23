using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;


namespace Olimpiada
{
    [Activity(Label = "Olimpiada", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IOnMapReadyCallback,ILocationListener
    {
        private GoogleMap map;

        private LocationManager locationManager;
        Location currentLocation;

        TextView latitudeLongitudeTextView;
        TextView addressTextView;
        TextView distanceTextView;
        string locationProvider;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            latitudeLongitudeTextView = FindViewById<TextView>(Resource.Id.LatitudeLongetude);
            addressTextView = FindViewById<TextView>(Resource.Id.Address);
            distanceTextView = FindViewById<TextView>(Resource.Id.Distance);

            locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Medium
            };

            locationProvider = locationManager.GetBestProvider(criteriaForLocationService, true);

            /*IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);

            if(acceptableLocationProviders.Any())
            {
                locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                locationProvider = string.Empty;
            }*/

            Console.WriteLine("*************" + locationProvider + "*************");

            setUpMap();

        }

        protected override void OnResume()
        {
            base.OnResume();
            locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
        }

        private void setUpMap()
        {
            if(map == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(new LatLng(-22.9109878, -43.7285266),9.5f);
            map.MoveCamera(camera);
            map.UiSettings.ZoomControlsEnabled = true;

            BitmapDescriptor btm =BitmapDescriptorFactory.FromResource(Resource.Drawable.Pacote);

            MarkerOptions options = new MarkerOptions()
                .SetPosition(new LatLng(-22.951907, -43.210497))
                .SetTitle("Cristo Redentor")
                .SetSnippet("Recebendo o Rio de braços abertos")
                .SetIcon(btm);

            map.AddMarker(options);

        }

        public void OnLocationChanged(Location location)
        {
            try
            {
                currentLocation = location;
                if(currentLocation == null)
                {
                    latitudeLongitudeTextView.Text = "Location not found";
                }
                else
                {
                    latitudeLongitudeTextView.Text = String.Format("{0},{1}", currentLocation.Latitude, currentLocation.Longitude);
                }

                Android.Locations.Geocoder geocoder = new Geocoder(this);

                IList<Address> addressList = geocoder.GetFromLocation(currentLocation.Latitude, currentLocation.Longitude, 5);
                Address address = addressList.FirstOrDefault();

                double lat1 = currentLocation.Latitude;
                double theta = currentLocation.Longitude - (-0.13);
                double distance = Math.Sin(Math.PI / 100.0 * (lat1))
                                           * Math.Sin(Math.PI / 180.0 * (51.50)) *
                                             Math.Cos(Math.PI / 180.0 * (lat1)) *
                                             Math.Cos(Math.PI / 180.0 * (51.50)) *
                                             Math.Cos(Math.PI / 180.0 * (theta));
                distanceTextView.Text = "Distance" + distance.ToString() + "miles";

                if(address != null)
                {
                    StringBuilder deviceAddress = new StringBuilder();
                    for(int i = 0; i < address.MaxAddressLineIndex; i++)
                    {
                        deviceAddress.Append(address.GetAddressLine(i)).AppendLine(",");
                    }
                    addressTextView.Text = deviceAddress.ToString();
                }
                else
                {
                    addressTextView.Text = "address not found";
                }
            }
            catch
            {
                addressTextView.Text = "address not found";
            }
        }

        public void OnProviderDisabled(string provider)
        {
           
        }

        public void OnProviderEnabled(string provider)
        {
            
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            
        }
    }
}

