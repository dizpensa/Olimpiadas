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
using Newtonsoft.Json;

namespace Olimpiada
{
    [Activity(Label = "Olimpiada", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IOnMapReadyCallback, ILocationListener,IInfoWindowAdapter, IOnInfoWindowClickListener
    {
        private GoogleMap map;

        private LocationManager locationManager;
        Location currentLocation; 

        string locationProvider;

        List<Figure> figures;

        Button albumButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            ActionBar.Hide();

            figures = setUpFigures(); 

            locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy =  Accuracy.Coarse,
                PowerRequirement = Power.Medium
            };

            albumButton = FindViewById<Button>(Resource.Id.albumButton);

            albumButton.Click += AlbumButton_Click;

            locationProvider = locationManager.GetBestProvider(criteriaForLocationService, true);

            setUpMap();

        }

        private void AlbumButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(Album));
            intent.PutExtra("figures", JsonConvert.SerializeObject(figures));
            this.StartActivity(intent);
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

        private List<Figure> setUpFigures()
        {
            List<Figure> figures = new List<Figure>();
            figures.Add(new Figure("Cristo Redentor","ouro", new LatLng(-22.951907, -43.210497)));
            figures.Add(new Figure("Jardim Zoologico", "bronze", new LatLng(-22.9044037, -43.2311314)));
            figures.Add(new Figure("Maracanã","prata",new LatLng(-22.9121039, -43.2323445)));
            figures.Add(new Figure("Parque dos Patins", "prata", new LatLng(-22.9722663, -43.2186377)));
            figures.Add(new Figure("Quinta da Boa Vista", "bronze", new LatLng(-22.905522, -43.2259774)));
            figures.Add(new Figure("Jockey Club", "bronze", new LatLng(-22.9745449, -43.2235009)));
            figures.Add(new Figure("Jardim Botanico", "ouro", new LatLng(-22.9673667, -43.2272268)));
            figures.Add(new Figure("Planetário da Gávea", "prata", new LatLng(-22.978242, -43.2324218)));
            figures.Add(new Figure("Parque Lage", "ouro", new LatLng(-22.9603099, -43.2143196)));
            figures.Add(new Figure("Pedra do Arpoador", "ouro", new LatLng(-22.9901455, -43.1934786)));
            return figures;
        }

        public void setUpmarkers()
        {
            map.Clear();
            for (int i = 0; i < figures.Count; i++)
            {
                map.AddMarker(figures.ElementAt<Figure>(i).CreateMarker());
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(new LatLng(-22.9477901, -43.2293674), 12.35f);
            map.MoveCamera(camera);
            map.UiSettings.ZoomControlsEnabled = true;
            map.MyLocationEnabled = true;
            setUpmarkers();

            map.SetInfoWindowAdapter(this);
            map.SetOnInfoWindowClickListener(this);

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
            View view = LayoutInflater.Inflate(Resource.Layout.figureInfo,null,false);

            Figure figure = new Figure("", "", new LatLng(0, 0));

            for (int i =0; i < figures.Count; i++)
            {
                if (figures.ElementAt<Figure>(i).name.Equals(marker.Title))
                {
                    figure = figures.ElementAt(i);
                    break;
                }
            }

            view.FindViewById<ImageView>(Resource.Id.figureImage).SetImageResource(figure.imageId);
            view.FindViewById<TextView>(Resource.Id.figureName).Text = figure.name;
            view.FindViewById<TextView>(Resource.Id.figureKind).Text = figure.kind;

            double distance = Distance(figure.latLgn.Latitude, figure.latLgn.Longitude);
            if (distance < 1)
            {
                distance *= 1000;
                view.FindViewById<TextView>(Resource.Id.figureDistance).Text = Math.Round(distance, 2).ToString() + "m de distancia";
            }
            else
            {
                view.FindViewById<TextView>(Resource.Id.figureDistance).Text = Math.Round(distance, 2).ToString() + "Km de distancia";
            }
            return view;
        }


        public void OnInfoWindowClick(Marker marker)
        {
            Figure figure = new Figure("", "", new LatLng(0, 0));

            for (int i = 0; i < figures.Count; i++)
            {
                if (figures.ElementAt<Figure>(i).name.Equals(marker.Title))
                {
                    figure = figures.ElementAt(i);
                    break;
                }
            }

            if (IsClose(figure.latLgn, 0.1))
            {
                figure.Get();
                setUpmarkers();
            }
            else
            {
                Console.WriteLine("TOO FAR TO GET IT...");
            }


        }
    }
}