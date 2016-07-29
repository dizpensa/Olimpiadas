using Android.Gms.Maps.Model;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olimpiada
{   [Serializable]
    class Figure
    {
        public MarkerOptions marker { get; set; }
        public string name { get; set; }
        public bool got { get; set; }
        public string kind { get; set; }
        public LatLng latLgn { get; set; }
        public List<string> categories { get; set; }
        public BitmapDescriptor image { get; set; }
        public int imageId { get; set; }

        public Figure(string name, string kind, LatLng latLgn,List<string> categories)
        {
            this.name = name;
            got = false;
            this.latLgn = latLgn;
            this.kind = kind;
            this.categories = categories;
            if(kind == "ouro")
            {
                image = BitmapDescriptorFactory.FromResource(Resource.Drawable.PacoteOuro);
                imageId = Resource.Drawable.PacoteOuro;
            }
            else if(kind == "prata")
            {
                image = BitmapDescriptorFactory.FromResource( Resource.Drawable.PacotePrata);
                imageId = Resource.Drawable.PacotePrata;
            }
            else
            {
                image = BitmapDescriptorFactory.FromResource( Resource.Drawable.PacoteBronze);
                imageId = Resource.Drawable.PacoteBronze;
            }
        }

        public void Get()
        {
            got = true;
            image = BitmapDescriptorFactory.FromResource(Resource.Drawable.PacotePego);
            imageId = Resource.Drawable.PacotePego;
        }

        public MarkerOptions CreateMarker()
        {
            marker = new MarkerOptions()
                .SetPosition(latLgn)
                .SetTitle(name)
                .SetSnippet(kind)
                .SetIcon(image);
            return marker;
        }
    }
}