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
    public class MainActivity : Activity, IOnMapReadyCallback, ILocationListener
    {
        private GoogleMap map;

        private LocationManager locationManager;
        Location currentLocation;

        string locationProvider;

        bool first = true;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

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
            if (map == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(new LatLng(-22.9477901, -43.2293674), 12.35f);
            map.MoveCamera(camera);
            map.UiSettings.ZoomControlsEnabled = true;
            map.MyLocationEnabled = true;

            BitmapDescriptor pacoteOuro = BitmapDescriptorFactory.FromResource(Resource.Drawable.PacoteOuro);
            BitmapDescriptor pacotePrata = BitmapDescriptorFactory.FromResource(Resource.Drawable.PacotePrata);
            BitmapDescriptor pacoteBronze = BitmapDescriptorFactory.FromResource(Resource.Drawable.PacoteBronze);


            List<MarkerOptions> markers = new List<MarkerOptions>();

            MarkerOptions marker = new MarkerOptions()
                .SetPosition(new LatLng(-22.951907, -43.210497))
                .SetTitle("Cristo Redentor")
                .SetSnippet("Recebendo o Rio de braços abertos")
                .SetIcon(pacoteOuro);
            markers.Add(marker);

            marker = new MarkerOptions()
                .SetPosition(new LatLng(-22.9044037, -43.2311314))
                .SetTitle("Jardim Zoologico")
                .SetIcon(pacotePrata);
            markers.Add(marker);

            map.MarkerClick += Map_MarkerClick;

            for (int i = 0; i < markers.Count; i++)
           {
                map.AddMarker(markers.ElementAt<MarkerOptions>(i));
           }

        }

        private void Map_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            IsClose(e.Marker.Position.Latitude, e.Marker.Position.Longitude);
        }

        public void OnLocationChanged(Location location)
        {
            try
            {
                currentLocation = location;
                if (currentLocation == null)
                {
                    Console.WriteLine("LOCATION NOT FOUND");
                }
                else if(first)
                {
                    first = false;
                }
                /*
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
                //distanceTextView.Text = "Distance" + distance.ToString() + "miles";

                if (address != null)
                {
                    StringBuilder deviceAddress = new StringBuilder();
                    for (int i = 0; i < address.MaxAddressLineIndex; i++)
                    {
                        deviceAddress.Append(address.GetAddressLine(i)).AppendLine(",");
                    }
                   // addressTextView.Text = deviceAddress.ToString();
                }
                else
                {
                   // addressTextView.Text = "address not found";
                }*/
            }
            catch
            {
                //addressTextView.Text = "address not found";
            }
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }

        private bool IsClose(double figLat,double figLng)
        {
            figLat = (Math.PI / 180.0) * figLat;
            figLng = (Math.PI / 180.0) * figLng;
            double lat = (Math.PI / 180.0) * currentLocation.Latitude;
            double lgn = (Math.PI / 180.0) * currentLocation.Longitude;
            double R = 6372.795477598;
            double distance = R * Math.Acos((Math.Sin(lat) * Math.Sin(figLat) + Math.Cos(lat) * Math.Cos(figLat) * Math.Cos(lgn - figLng)));
            Console.WriteLine("Distancia: " + distance + "km");
            return true;
        }

    }
}