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
using static Android.Gms.Maps.GoogleMap;

namespace Olimpiada
{
    [Activity(Label = "Olimpiada", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IOnMapReadyCallback, ILocationListener,IInfoWindowAdapter
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
            ActionBar.Hide();

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
                .SetSnippet("Dourada")
                .SetIcon(pacoteOuro);
            markers.Add(marker);

            marker = new MarkerOptions()
                .SetPosition(new LatLng(-22.9044037, -43.2311314))
                .SetTitle("Jardim Zoologico")
                .SetIcon(pacoteBronze)
                .SetSnippet("Bronze");
            markers.Add(marker);

            for (int i = 0; i < markers.Count; i++)
            {
                map.AddMarker(markers.ElementAt<MarkerOptions>(i));
            }
            map.SetInfoWindowAdapter(this);

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
            }
            catch
            {
                Console.WriteLine("ADDRESS NOT FOUND");
            }
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }

        private double Distance(double figLat,double figLng)
        {
            figLat = (Math.PI / 180.0) * figLat;
            figLng = (Math.PI / 180.0) * figLng;
            double lat = (Math.PI / 180.0) * currentLocation.Latitude;
            double lgn = (Math.PI / 180.0) * currentLocation.Longitude;
            double R = 6372.795477598;
            double distance = R * Math.Acos((Math.Sin(lat) * Math.Sin(figLat) + Math.Cos(lat) * Math.Cos(figLat) * Math.Cos(lgn - figLng)));
            Console.WriteLine("Distancia: " + distance + "km");
            return distance;
        }

        private bool IsClose(LatLng latLgn,double radius)
        {
            if(Distance(latLgn.Latitude,latLgn.Longitude) <= radius)
            {
                return true;
            }
            return false;
        }

        public View GetInfoContents(Marker marker)
        {
            return null;
        }

        public View GetInfoWindow(Marker marker)
        {
            View view = LayoutInflater.Inflate(Resource.Layout.figureInfo, null, false);
            view.Focusable = false;
            view.Activated = false;
            if (marker.Snippet.Equals("Dourada"))
            {
                view.FindViewById<ImageView>(Resource.Id.figureImage).SetImageResource(Resource.Drawable.PacoteOuro);
            }
            else if (marker.Snippet.Equals("Prata"))
            {
                view.FindViewById<ImageView>(Resource.Id.figureImage).SetImageResource(Resource.Drawable.PacotePrata);
            }
            else
            {
                view.FindViewById<ImageView>(Resource.Id.figureImage).SetImageResource(Resource.Drawable.PacoteBronze);
            }
            view.FindViewById<TextView>(Resource.Id.figureName).Text = marker.Title;
            double distance = Distance(marker.Position.Latitude, marker.Position.Longitude);
            if(distance < 1)
            {
                distance *= 1000;
                view.FindViewById<TextView>(Resource.Id.figureDistance).Text = Math.Round(distance,2).ToString() + "m de distancia";
            }
            else
            {
                view.FindViewById<TextView>(Resource.Id.figureDistance).Text = Math.Round(distance, 2).ToString() + "Km de distancia";
            }
            view.FindViewById<TextView>(Resource.Id.figureKind).Text = marker.Snippet;
            return view;
        }
    }
}