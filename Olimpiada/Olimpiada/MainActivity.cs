using System;
using System.Collections.Generic;
using System.Linq;

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
using Android.Provider;

using Environment = Android.OS.Environment;
using Path = System.IO.Path;
using Uri = Android.Net.Uri;
using Android.Graphics;
using System.IO;

namespace Olimpiada
{
    [Activity(Label = "Olimpiada", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IOnMapReadyCallback, ILocationListener, IInfoWindowAdapter, IOnInfoWindowClickListener
    {
        private GoogleMap map;

        private LocationManager locationManager;
        Location currentLocation;

        string locationProvider;

        List<Figure> figures;

        Button albumButton;

        Bitmap currentFigureImage;

        int CAMERA_CAPTURE_IMAGE_REQUEST_CODE = 100;
        string IMAGE_DIRECTORY_NAME = "Photos";
        private Uri fileUri;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            ActionBar.Hide();

            /*ISharedPreferences pref = Application.Context.GetSharedPreferences("AppInfo", FileCreationMode.Private);
            string tempFigures = pref.GetString("Figures", string.Empty);

            if(tempFigures == string.Empty)
            {
                figures = setUpFigures();
            }
            else
            {
                figures = JsonConvert.DeserializeObject<List<Figure>>(tempFigures);
            }*/
            figures = setUpFigures();

            locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Coarse,
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
           // figures.Add(new Figure("Casa", "ouro", new LatLng(-22.904329, -43.3007853), new List<string> { "" }));
            figures.Add(new Figure("Cristo Redentor", "ouro", new LatLng(-22.951907, -43.210497), new List<string> { "tradicional", "paisagem" }));
            figures.Add(new Figure("Jardim Zoologico", "bronze", new LatLng(-22.9044037, -43.2311314), new List<string> { "parque" }));
            figures.Add(new Figure("Maracanã", "prata", new LatLng(-22.9121039, -43.2323445), new List<string> { "tradicional", "esporte" }));
            figures.Add(new Figure("Parque dos Patins", "prata", new LatLng(-22.9722663, -43.2186377), new List<string> { "parque" }));
            figures.Add(new Figure("Quinta da Boa Vista", "bronze", new LatLng(-22.905522, -43.2259774), new List<string> { "parque" }));
            figures.Add(new Figure("Jockey Club", "bronze", new LatLng(-22.9745449, -43.2235009), new List<string> { "esporte" }));
            figures.Add(new Figure("Jardim Botanico", "ouro", new LatLng(-22.9673667, -43.2272268), new List<string> { "tradicional", "parque" }));
            figures.Add(new Figure("Planetário da Gávea", "prata", new LatLng(-22.978242, -43.2324218), new List<string> { "ciencia" }));
            figures.Add(new Figure("Parque Lage", "ouro", new LatLng(-22.9603099, -43.2143196), new List<string> { "parque" }));
            figures.Add(new Figure("Pedra do Arpoador", "ouro", new LatLng(-22.9901455, -43.1934786), new List<string> { "paisagem", "praia" }));
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

        private double Distance(double figLat, double figLng)
        {
            figLat = (Math.PI / 180.0) * figLat;
            figLng = (Math.PI / 180.0) * figLng;
            double lat = (Math.PI / 180.0) * currentLocation.Latitude;
            double lgn = (Math.PI / 180.0) * currentLocation.Longitude;
            double R = 6372.795477598;
            double distance = R * Math.Acos((Math.Sin(lat) * Math.Sin(figLat) + Math.Cos(lat) * Math.Cos(figLat) * Math.Cos(lgn - figLng)));
            return distance;
        }

        private bool IsClose(LatLng latLgn, double radius)
        {
            if (Distance(latLgn.Latitude, latLgn.Longitude) <= radius)
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

            Figure figure = new Figure("", "", new LatLng(0, 0), new List<string>());

            for (int i = 0; i < figures.Count; i++)
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
            if (!IsClose(figure.latLgn, 0.1))
            {
                view.FindViewById<Button>(Resource.Id.getFigure).Text = "Informações";
                view.SetBackgroundColor(Android.Graphics.Color.LightGray);
            }
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
            Figure figure = new Figure("", "", new LatLng(0, 0), new List<string>());

            for (int i = 0; i < figures.Count; i++)
            {
                if (figures.ElementAt<Figure>(i).name.Equals(marker.Title))
                {
                    /*string temp = JsonConvert.SerializeObject(figures);
                    Console.WriteLine(temp);
                    ISharedPreferences pref = Application.Context.GetSharedPreferences("AppInfo",FileCreationMode.Private);
                    ISharedPreferencesEditor edit = pref.Edit();
                    edit.PutString("Figures",temp.Trim());
                    edit.Apply();*/
                    figure = figures.ElementAt(i);
                    break;
                }
            }

            if (IsClose(figure.latLgn, 0.1))
            {
                figure.Get();
                setUpmarkers();
                takePhoto();
                if (currentFigureImage != null)
                {
                    figure.image = BitmapDescriptorFactory.FromBitmap(currentFigureImage);
                    currentFigureImage = null;
                }
            }
            else
            {
                Console.WriteLine("TOO FAR TO GET IT...");
            }


        }

        void takePhoto()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            fileUri = GetOutputMidiaFile(this.ApplicationContext, IMAGE_DIRECTORY_NAME, string.Empty);
            Intent.PutExtra(MediaStore.ExtraOutput, fileUri);

            StartActivityForResult(intent, CAMERA_CAPTURE_IMAGE_REQUEST_CODE);
        }

        Uri GetOutputMidiaFile(Context context, string subdir, string name)
        {
            subdir = subdir ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                name = "IMG_" + timestamp + ".jpg";
            }
            //OBTENEMOS EL PATH DONDE SE ENCUENTRA EL DIRECTORIO DE PICTURES DENTRO DE NUESTRO DISPOSITIVO
            string mediaType = Environment.DirectoryPictures;

            //OBTENEMOS EL PATH ABSOLUTO EN COMBINACION CON EL SUBDIRECTORIO DONDE GUARDAREMOS LAS IMAGENES
            using (Java.IO.File mediaStorageDir = new Java.IO.File(Environment.GetExternalStoragePublicDirectory(mediaType), subdir))
            {
                //VERIFICAMOS QUE EXISTA EL SUBDIRECTORIO
                if (!mediaStorageDir.Exists())
                {
                    //SI NO EXISTE CREAMOS EL DIRECTORIO
                    if (!mediaStorageDir.Mkdirs())
                        throw new IOException("NO se pudo crear el directorio, asegurece que tenga permisos la APP para crear archivos");

                }

                //REGRESAMOS LA URI DE LA NUEVA IMAGEN QUE SE USARA PARA GUARDAR LA FOTO
                return Uri.FromFile(new Java.IO.File(GetUniquePath(mediaStorageDir.Path, name)));

            }
        }

        string GetUniquePath(string path, string name)
        {
            string ext = Path.GetExtension(name);
            if (ext == String.Empty)
                ext = ".jpg";

            name = Path.GetFileNameWithoutExtension(name);

            string nname = name + ext;
            int i = 1;
            while (File.Exists(Path.Combine(path, nname)))
                nname = name + "_" + (i++) + ext;

            return Path.Combine(path, nname);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // VERIFICAMOS EL CODIGO DE CAPTURA DE LA IMAGEN
            if (requestCode == CAMERA_CAPTURE_IMAGE_REQUEST_CODE)
            {
                //SI LA FOTO SE CAPTURO PROCEDEMOS A MOSTRA LA IMAGEN DE ESTA
                if (resultCode == Result.Ok)
                {
                    // DESPLEGAMOS LA IMAGEN QUE CAPTURAMOS DE NUESTRA FOTO
                    previewCapturedImage();
                }
                else if (resultCode == Result.Canceled)
                {
                    // CUANDO LA CAPTURA ES CANCELADA

                    Toast.MakeText(this.ApplicationContext, "Captura de imagem cancelada", ToastLength.Short)
                        .Show();
                }
                else
                {
                    // CUANDO ALGO FALLO EN LA CAPTURA DE LA IMAGEN
                    Toast.MakeText(this.ApplicationContext, "Falha na captura de imagem", ToastLength.Short)
                        .Show();
                }

            }

        }

        void previewCapturedImage()
        {
            //MUESTRA LA IMAGEN QUE CAPTURAMOS
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InSampleSize = 8;
            //CREAMOS NEUSTRO BITMAP
            Bitmap bitmap = BitmapFactory.DecodeFile(fileUri.Path, options);
            //LE ASIGNAMOS EL BITMAP A NUESTRO CONTROL IMAGE
            //imgPreview.SetImageBitmap(bitmap);
            currentFigureImage = bitmap;
        }

    }
}