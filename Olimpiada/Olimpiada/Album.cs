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
            figures = JsonConvert.DeserializeObject<List<Figure>>(Intent.GetStringExtra("figures"));
            List<string> list = new List<string>();
            for(int i = 0; i < figures.Count; i++)
            {
                if (figures.ElementAt(i).got)
                {
                    list.Add(figures.ElementAt(i).name + " :  Obtida");
                }
                else
                {
                    list.Add(figures.ElementAt(i).name + " :  Não Obtida");
                }
            }
            for(int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(list.ElementAt(i));
            }
            
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1,list);

            figuresListView.Adapter = adapter;

        }
    }
}