using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Firebase.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.Chip;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Kota_Palace.Models;
using Plugin.CloudFirestore;
using Xamarin.Essentials;
using static Android.Gms.Common.Apis.Api;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Kota_Palace.Dialogs
{
    public class SelectedItemDialog : DialogFragment
    {

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.check_out_dialog, container, false);
            ConnectView(view);

            return view;
        }
        private MaterialButton BtnAdd;
        private MaterialButton BtnRemove;
        private MaterialButton BtnCheckout;
        private ChipGroup chipAddOns;
        private ChipGroup chipSources;
        private TextView Txt_counter;
        private FloatingActionButton FabClose;
        private TextInputEditText InputName;
        private TextInputEditText InputPrice;
        private TextInputEditText InputNote;
        int counter = 1;
        private MenuModel menuModel;
        private int spaza_id;
        private List<string> CartItemsExtras = new List<string>();
        public SelectedItemDialog(MenuModel menuModel, int spazaID)
        {
            this.menuModel = menuModel;
            spaza_id = spazaID;
        }
        private Context context;
        private void ConnectView(View view)
        {
            context = view.Context;
            BtnAdd = view.FindViewById<MaterialButton>(Resource.Id.check_out_item_add);
            BtnCheckout = view.FindViewById<MaterialButton>(Resource.Id.BtnCheckOut);
            BtnRemove = view.FindViewById<MaterialButton>(Resource.Id.check_out_item_remove);
            FabClose = view.FindViewById<FloatingActionButton>(Resource.Id.check_out_fab_close);
            InputName = view.FindViewById<TextInputEditText>(Resource.Id.check_out_item_name);
            InputNote = view.FindViewById<TextInputEditText>(Resource.Id.check_out_item_additional_info);
            InputPrice = view.FindViewById<TextInputEditText>(Resource.Id.check_out_item_price);
            Txt_counter = view.FindViewById<TextView>(Resource.Id.check_out_item_counter);
            chipAddOns = view.FindViewById<ChipGroup>(Resource.Id.check_out_item_add_ons);
            chipSources = view.FindViewById<ChipGroup>(Resource.Id.check_out_item_sources);

            InputName.Text = menuModel.Name;

            InputPrice.Text = $"{menuModel.Price}";
            InputName.Enabled = false;
            InputPrice.Enabled = false;
            Txt_counter.Text = "1";

            //chip_items = new List<string>();

            if (menuModel.Extras != null)
            {

                var inflator = LayoutInflater.From(view.Context);
                foreach (var item in menuModel.Extras)
                {
                    var chip = inflator.Inflate(Resource.Layout.chip_item, null, false) as Chip;
                    chip.Text = $"{item.Title}";
                    chip.Selected = true;
                    chipAddOns.AddView(chip);
                    CartItemsExtras.Add(item.Title);
                }
            }
            BtnCheckout.Click += BtnCheckout_Click;
            BtnRemove.Click += BtnRemove_Click;
            BtnAdd.Click += BtnAdd_Click;
            FabClose.Click += FabClose_Click;
        }

        private void FabClose_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            counter++;
            Txt_counter.Text = counter.ToString();
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (counter > 1)
            {
                counter--;
                Txt_counter.Text = counter.ToString();
            }
        }

        private JavaList<string> AddOnItems = new JavaList<string>();
        private JavaList<string> SourcesItems = new JavaList<string>();
        private async void BtnCheckout_Click(object sender, EventArgs e)
        {
            string c_id = Preferences.Get("Id", null);

            CartModel cartModel = new CartModel()
            {
                Price = ((decimal)(menuModel.Price * counter)),
                Quantity = counter,
                Extras = string.Join("#", CartItemsExtras.ToArray()),
                ItemName = menuModel.Name,
                Note = InputNote.Text,
                BusinessId = spaza_id,
                Customer_Id = c_id,
            };

            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync($"{API.ApiUrl}/carts/item", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    string results = await response.Content.ReadAsStringAsync();
                    AndHUD.Shared.ShowSuccess(context, "Item has been added to cart", MaskType.Black, TimeSpan.FromSeconds(2));
                }
                else
                {
                    string results = await response.Content.ReadAsStringAsync();
                    AndHUD.Shared.ShowError(context, $"{results}", MaskType.Black, TimeSpan.FromSeconds(2));
                }
            }
            catch (Exception ex)
            {
                AndHUD.Shared.ShowError(context, $"{ex.Message}", MaskType.Black, TimeSpan.FromSeconds(2));
            }
            AddOnItems.Clear();
            FabClose.PerformClick();
        }

        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        }
    }
}