using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Olimpiada
{
    [Activity(Label = "Album")]
    public class Album : Activity
    {
        List<Figure> figures;
        TextView element;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.album);
            ActionBar.Hide();
            element = FindViewById<TextView>(Resource.Id.figuraTextView);
            figures = JsonConvert.DeserializeObject<List<Figure>>(Intent.GetStringExtra("figures"));
            element.Text = figures.ElementAt<Figure>(0).name +"got: "+figures.ElementAt<Figure>(0).got;
        }
    }
}