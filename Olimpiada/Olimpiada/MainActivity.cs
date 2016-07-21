using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Maps;

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
        }
    }
}

