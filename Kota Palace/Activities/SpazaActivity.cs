using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using FFImageLoading;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextView;
using Kota_Palace.Adapters;
using Kota_Palace.Dialogs;
using Kota_Palace.Models;
using Refractored.Controls;
using System;
using System.Collections.Generic;
using System.Net.Http;
using static Android.Content.ClipData;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Kota_Palace.Activities
{
    [Activity(Label = "SelectedSpaza")]
    public class SpazaActivity : AppCompatActivity
    {
        private RecyclerView Recycler;
        private readonly List<MenuModel> items = new List<MenuModel>();
        private int spazaID;
        private CircleImageView imgIcon;
        private AppCompatImageView imgBack;
        private FloatingActionButton FabCall;
        private FloatingActionButton FabCart;
        private string phone = null;
        private MaterialTextView txt_title;
        private MaterialTextView txt_status;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            spazaID = Intent.GetIntExtra("SpazaKeyId", -1);
            // Create your application here

            SetContentView(Resource.Layout.layout_spaza_home);
            ConnectViews();
        }
        public override void SetEnterSharedElementCallback(SharedElementCallback callback)
        {
            base.SetEnterSharedElementCallback(callback);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (Android.Resource.Id.Home == item.ItemId)
            {
                Finish();
            }

            return base.OnOptionsItemSelected(item);
        }
        private async void ConnectViews()
        {
            Recycler = FindViewById<RecyclerView>(Resource.Id.RecyclerSpazaMenu);
            // toolbar = FindViewById<Toolbar>(Resource.Id.top_toolbar);
            imgIcon = FindViewById<CircleImageView>(Resource.Id.img_icon);
            imgBack = FindViewById<AppCompatImageView>(Resource.Id.img_back);
            FabCart = FindViewById<FloatingActionButton>(Resource.Id.FabCart);
            FabCall = FindViewById<FloatingActionButton>(Resource.Id.fab_call);
            txt_title = FindViewById<MaterialTextView>(Resource.Id.txt_title);
            txt_status = FindViewById<MaterialTextView>(Resource.Id.txt_status);
            // var empty_view = FindViewById<EmptyView>(Resource.Id.empty_view);
            //empty_view.ShowLoading(Resource.String.app_name);


            //FabCall.Enabled = false;
            FabCart.Click += FabCart_Click;
            imgBack.Click += (s, e) =>
            {
                Finish();
            };
            FabCall.Click += (s, e) =>
            {
                Xamarin.Essentials.PhoneDialer.Open(phone);
            };
            SpazaMenuAdapter adapter = new SpazaMenuAdapter(items);
            var gl = new GridLayoutManager(this, 2)
            {
                Orientation = RecyclerView.Vertical
            };
            gl.SetSpanSizeLookup(new TSpanSizeLookup(adapter));
            Recycler.SetLayoutManager(gl);
            Recycler.SetAdapter(adapter);
            adapter.ItemAddToCartClick += Adapter_ItemAddToCartClick;


            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{API.ApiUrl}/businesses/buss_menu/{spazaID}");
                if (response.IsSuccessStatusCode)
                {
                    var results = await response.Content.ReadAsStringAsync();
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<BusinessMenuViewModel>(results);
                    //toolbar.Title = data.Business.BusinessName;
                    ImageService.Instance.LoadUrl(data.Business.ImgUrl)
                        .DownSampleInDip(512, 512)
                        .FadeAnimation(true)
                        .Into(imgIcon);
                    txt_status.Text = data.Business.Online;
                    txt_title.Text = data.Business.Name;
                    phone = data.Business.PhoneNumber;

                    foreach (var item in data.Menu)
                    {
                        items.Add(item);
                        adapter.NotifyDataSetChanged();
                    }
                    //items.Add(new MenuModel() { BusinessId = 1, Extras = null, Name = "xxx", Price = 200, Status = true, Url = "xxxxxx" });
                    adapter.NotifyDataSetChanged();
                }
                else
                {
                    var results = await response.Content.ReadAsStringAsync();
                    AndHUD.Shared.ShowError(this, results, MaskType.Clear, TimeSpan.FromSeconds(3));
                }
            }
            catch (Exception)
            {

            }



            //CrossCloudFirestore
            //    .Current
            //    .Instance
            //    .Collection("Admins")
            //    .Document(spazaID)
            //    .AddSnapshotListener((snapshot, errors) =>
            //    {
            //        if (!snapshot.Exists)
            //        {
            //            var data = snapshot.ToObject<SpazaModel>();
            //            toolbar.Title = data.BusinessName;
            //            ImageService.Instance.LoadUrl(data.ImgUrl)
            //                .IntoAsync(imgIcon);
            //        }
            //    });


            //CrossCloudFirestore
            //    .Current
            //    .Instance
            //    .Collection("Menu")
            //    .Document(spazaID)
            //    .Collection("Items")
            //    .AddSnapshotListener((snapshot, errors) =>
            //    {
            //        if (!snapshot.IsEmpty)
            //        {
            //            foreach (var dc in snapshot.DocumentChanges)
            //            {
            //                switch (dc.Type)
            //                {
            //                    case DocumentChangeType.Added:
            //                        items.Add(dc.Document.ToObject<MenuModel>());
            //                        adapter.NotifyDataSetChanged();
            //                        break;
            //                    case DocumentChangeType.Modified:

            //                        break;
            //                    case DocumentChangeType.Removed:
            //                        break;
            //                    default:
            //                        break;
            //                }
            //            }

            //        }
            //    });



        }

        private void Adapter_ItemAddToCartClick(object sender, SpazaMenuAdapterClickEventArgs e)
        {
            //CheckOutDialog(e.Position);
            SelectedItemDialog dlg = new SelectedItemDialog(items[e.Position], spazaID);
            dlg.Show(SupportFragmentManager.BeginTransaction(), null);
        }

        private void FabCart_Click(object sender, EventArgs e)
        {
            CartDialog dlg = new CartDialog();
            dlg.Show(SupportFragmentManager.BeginTransaction(), null);
        }

        private void FabCall_Click(object sender, EventArgs e)
        {
            Xamarin.Essentials.PhoneDialer.Open(phone);
        }




    }
}
public class BusinessMenuViewModel
{
    public SpazaModel Business { get; set; }
    public List<MenuModel> Menu { get; set; }
}

public class TSpanSizeLookup : GridLayoutManager.SpanSizeLookup
{
    private readonly SpazaMenuAdapter adapter;

    public TSpanSizeLookup(SpazaMenuAdapter EAdp)
    {
        this.adapter = EAdp;
    }

    public override int GetSpanSize(int position)
    {
        return (adapter.GetItemViewType(position)) switch
        {
            1 => 1,
            0 => 2,
            _ => -1,
        };
    }
}

public class SpaceItemDeco : RecyclerView.ItemDecoration
{
    readonly int space;
    public SpaceItemDeco(int space)
    {
        this.space = space;
    }

    public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
    {
        base.GetItemOffsets(outRect, view, parent, state);
        outRect.Top = outRect.Bottom = outRect.Left = outRect.Right = space;
    }
}