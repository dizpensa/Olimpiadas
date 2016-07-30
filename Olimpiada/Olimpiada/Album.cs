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
        ListView figuresListView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.album);
            ActionBar.Hide();
            figuresListView = FindViewById<ListView>(Resource.Id.listaFigurinhas);
            try
            {
                figures = JsonConvert.DeserializeObject<List<Figure>>(Intent.GetStringExtra("figures"));

                AlbumListViewAdapter adapter = new AlbumListViewAdapter(this, figures);

                figuresListView.Adapter = adapter;
            }
            catch
            {
                Toast.MakeText(ApplicationContext, "Puta que pariu", ToastLength.Short)
                       .Show();
            }
        }
    }
}