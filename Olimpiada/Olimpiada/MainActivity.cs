using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

namespace Olimpiada
{
    [Activity(Label = "Olimpiada", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IOnMapReadyCallback
    {
        private GoogleMap map;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
           
            SetContentView(Resource.Layout.Main);

            setUpMap();

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
            map.MapType = GoogleMap.MapTypeHybrid;

            LatLng latlgn = new LatLng(-22.951907, -43.210497);

            BitmapDescriptor btm =BitmapDescriptorFactory.FromResource(Resource.Drawable.cristo);

            MarkerOptions options = new MarkerOptions()
                .SetPosition(latlgn)
                .SetTitle("Cristo Redentor")
                .SetSnippet("Recebendo o Rio de braços abertos")
                .SetIcon(btm);

            map.AddMarker(options);

        }
    }
}

