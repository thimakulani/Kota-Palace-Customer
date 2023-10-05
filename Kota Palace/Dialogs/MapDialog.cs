using Android.Content;
using Android.Gms.Common.Api.Internal;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Google.Android.Material.Button;
using Google.Android.Material.TextView;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kota_Palace.Dialogs
{
    public class MapDialog : DialogFragment, IOnMapReadyCallback
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        private Context context;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.map_frag, container, false);

            context = view.Context;
            ConnectViews(view);
            return view;
        }
        private GoogleMap googleMap;
        public async void OnMapReady(GoogleMap googleMap)
        {
            this.googleMap = googleMap;
            var location = await Xamarin.Essentials.Geolocation.GetLocationAsync();

            if(location != null)
            {
                var latlan = new LatLng(location.Latitude, location.Longitude);
                googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(latlan, 17));
            }


            googleMap.CameraIdle += GoogleMap_CameraIdle;
        }

        private async void GoogleMap_CameraIdle(object sender, EventArgs e)
        {
            var LatLan = googleMap.CameraPosition.Target;
            if (LatLan != null)
            {
                Geocoder geocoder = new Geocoder(context);
                IList<Address> addressList = await geocoder.GetFromLocationAsync(LatLan.Latitude, LatLan.Longitude, 5);

                Address address = addressList?.FirstOrDefault();

                TxtAddress.Text = $"{address?.GetAddressLine(0)}";
            }
        }

        private MaterialButton BtnConfirm;
        private MaterialTextView TxtAddress;
        private void ConnectViews(View view)
        {
            BtnConfirm = view.FindViewById<MaterialButton>(Resource.Id.BtnConfirm);
            TxtAddress = view.FindViewById<MaterialTextView>(Resource.Id.TxtAddress);
            
            var mapFragment = ChildFragmentManager.FindFragmentById(Resource.Id.fragMap).JavaCast<SupportMapFragment>();
            mapFragment.GetMapAsync(this);


            BtnConfirm.Click += BtnConfirm_Click;
            // Toast.MakeText(context,$"{lat} + {lon}", ToastLength.Long).Show();
        }
        public event EventHandler<AddressEventHandler> AddressChanged;
        public class AddressEventHandler : EventArgs
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; } 
        }
        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            var LatLan = googleMap.CameraPosition.Target;
            AddressChanged.Invoke(this, new AddressEventHandler() { Latitude = LatLan.Latitude, Longitude = LatLan.Longitude });
            Dismiss();
        }
        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        }
    }
}