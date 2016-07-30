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

namespace Olimpiada
{
    class AlbumListViewAdapter : BaseAdapter<Figure>
    {

        private List<Figure> figures;
        private Context context;

        public AlbumListViewAdapter(Context context, List<Figure> figures)
        {
            this.figures = figures;
            this.context = context;
        }

        public override Figure this[int position]
        {
            get
            {
                return figures[position];
            }
        }

        public override int Count
        {
            get
            {
                return figures.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            if(row == null)
            {
                row = LayoutInflater.From(context).Inflate(Resource.Layout.AlbumRow, null, false);
            }

            ImageView figureImage = row.FindViewById<ImageView>(Resource.Id.figureImageInAlbum1);
            TextView figureName = row.FindViewById<TextView>(Resource.Id.figureNameInAlbum1);
            TextView figureKind = row.FindViewById<TextView>(Resource.Id.figureKindInAlbum1);

            figureImage.SetImageResource(figures[position].imageId);
            figureName.Text = figures[position].name;
            figureKind.Text = figures[position].kind;

            return row;
        }
    }
}