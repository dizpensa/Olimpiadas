using Android.Gms.Maps.Model;
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
        public int imageId { get; set; }
        public string kind { get; set; }
        public LatLng latLgn;

        public Figure(string name, string kind, LatLng latLgn)
        {
            this.name = name;
            got = false;
            this.latLgn = latLgn;
            this.kind = kind;
            if(kind == "ouro")
            {
                imageId = Resource.Drawable.PacoteOuro;
            }
            else if(kind == "prata")
            {
                imageId = Resource.Drawable.PacotePrata;
            }
            else
            {
                imageId = Resource.Drawable.PacoteBronze;
            }
        }

        public void Get()
        {
            got = true;
            imageId = Resource.Drawable.PacotePego;
        }

        public MarkerOptions CreateMarker()
        {
            marker = new MarkerOptions()
                .SetPosition(latLgn)
                .SetTitle(name)
                .SetSnippet(kind)
                .SetIcon(BitmapDescriptorFactory.FromResource(imageId));
            return marker;
        }
    }
}