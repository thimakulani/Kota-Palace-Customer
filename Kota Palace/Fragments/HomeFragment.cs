using Android.Content;
using Android.OS;
using Android.Text.Format;
using Android.Views;
using AndroidHUD;
using AndroidX.AppCompat.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using IO.SuperCharge.ShimmerLayoutLib;
using Kota_Palace.Adapters;
using Kota_Palace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Android.Content.ClipData;
using static Android.Icu.Text.Transliterator;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Kota_Palace.Fragments
{
    public partial class HomeFragment : Fragment
    {
        private List<SpazaModel> Items = new List<SpazaModel>();
        private RecyclerView recycler;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        Context context;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.home_fragment, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }
        private ShimmerLayout shimmerLayout;
        private void ConnectViews(View view)
        {
            recycler = view.FindViewById<RecyclerView>(Resource.Id.RecyclerShops);
            shimmerLayout = view.FindViewById<ShimmerLayout>(Resource.Id.shimmer_layout);
            shimmerLayout.StartShimmerAnimation();
            var sv = view.FindViewById<SearchView>(Resource.Id.InputSearchSpaza);
            sv.QueryTextChange += (s, e) =>
            {
                Items = (from data in Items
                         where
                    data.Name == e.NewText
                         select data).ToList();
                adapter.NotifyDataSetChanged();
            };
            GetBusiness();

        }
        SpazaAdapter adapter;
        private async void GetBusiness()
        {
            //var currentLocation = await Geolocation.GetLocationAsync();
            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{API.ApiUrl}/businesses");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SpazaModel>>(result);
                    Items = data;
                    adapter = new SpazaAdapter(Items);
                    recycler.SetLayoutManager(new LinearLayoutManager(context));
                    recycler.SetAdapter(adapter);
                    adapter.ItemClick += Adapter_ItemClick;
                    adapter.NotifyDataSetChanged();
                    //adapter.NotifyDataSetChanged();
                    /*foreach (var item in data)
                    {
                        var latlang = item.Coordinates.Split('/');
                        double latitude = double.Parse(latlang[0].Trim());
                        double longitude = double.Parse(latlang[1].Trim());
                        var sp = new Xamarin.Essentials.Location(latitude, longitude);
                        var location = Xamarin.Essentials.Location.CalculateDistance(currentLocation, latitude, longitude, DistanceUnits.Kilometers);
                        if(location > 5)
                        {
                            Items.Add(item);
                            adapter.NotifyDataSetChanged();
                        }
                    }*/
                }
                else
                {
                    var result = await response.Content.ReadAsStringAsync();
                    AndHUD.Shared.ShowError(context, $"Something went wrong \n {result}", MaskType.Clear, TimeSpan.FromSeconds(3));
                }

            }
            catch(HttpRequestException EX)
            {
                AndHUD.Shared.ShowError(context, $"Something went wrong \n {EX.Message}", MaskType.Clear, TimeSpan.FromSeconds(3));
            }
            catch (Exception EX)
            {
                AndHUD.Shared.ShowError(context, $"Something went wrong \n {EX.Message}", MaskType.Clear, TimeSpan.FromSeconds(3));


            }
            finally
            {
                shimmerLayout.StopShimmerAnimation();
                shimmerLayout.Visibility = ViewStates.Gone;
            }

        }
        async void ChangedData(SpazaAdapter adapter)
        {

            await Task.Delay(1000).ContinueWith(t =>
            {
                adapter.NotifyDataSetChanged();
                ChangedData(adapter);//This is for repeate every 5s.
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        public event EventHandler<SpazaSelectedClickHandler> SpazaClicked;
        public class SpazaSelectedClickHandler : EventArgs
        {
            public int KeyId { get; set; }
            public View Itemview { get; set; }
        }
        private void Adapter_ItemClick(object sender, ShopsAdapterClickEventArgs e)
        {

            SpazaClicked.Invoke(this, new SpazaSelectedClickHandler { KeyId = Items[e.Position].Id, Itemview = e.View });
        }
    }
}