using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidHUD;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Firebase.Auth;
using FirebaseAdmin.Messaging;
using Google.Android.Material.Button;
using Google.Android.Material.TextView;
using Kota_Palace.Adapters;
using Kota_Palace.Fragments;
using Kota_Palace.Models;
using Plugin.CloudFirestore;
using Xamarin.Essentials;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Kota_Palace.Dialogs
{
    public class CartDialog : DialogFragment
    {


        // private readonly Dictionary<string, List<CartModel>> ItemChild = new Dictionary<string, List<CartModel>>();
        private readonly List<CartModel> Items = new List<CartModel>();
        private RecyclerView Recycler;
        private Context context;

        // private CartData data = new CartData();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            SetStyle(StyleNoFrame, Resource.Style.FullScreenDialogStyle);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.cart_dialog, container, false);
            ConnectView(view);

            return view;
        }
        MaterialButton Btn_check_out;
        MaterialTextView txt_total_price;

        private void ConnectView(View view)
        {
            context = view.Context;
            Btn_check_out = view.FindViewById<MaterialButton>(Resource.Id.btn_check_out);
            txt_total_price = view.FindViewById<MaterialTextView>(Resource.Id.txt_total_price);
            Toolbar menu_toolbar = view.FindViewById<Toolbar>(Resource.Id.cartToolBar);
            Recycler = view.FindViewById<RecyclerView>(Resource.Id.RecyclerCart);
            menu_toolbar.SetNavigationIcon(Resource.Drawable.ic_mtrl_chip_close_circle);
            menu_toolbar.NavigationClick += Menu_toolbar_NavigationClick;
            txt_total_price.Text = $"TOTAL PRICE: R0";
            Btn_check_out.Click += Btn_check_out_Click;
            Btn_check_out.Enabled = false;
            GetCart();


        }
        private void RefreshTotal()
        {
            decimal price = 0;
            foreach (var item in Items)
            {
                price += item.Quantity * item.Price;
            }
            txt_total_price.Text = $"TOTAL PRICE: R{price}";
            //if(price == 0) { Btn_check_out.Enabled = false; }
        }
        CartAdapter adapter;
        private async void GetCart()
        {
            string id = Preferences.Get("Id", "");
            Recycler.SetLayoutManager(new LinearLayoutManager(context));
            adapter = new CartAdapter(Items);
            RefreshTotal();
            adapter.ItemOptionClick += (s, e) =>
            {
                PopupMenu popup = new PopupMenu(context, e.Btn);
                popup.Menu.Add(IMenu.None, 0, 0, "REMOVE");
                popup.Show();
                popup.MenuItemClick += (ss, ee) =>
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(context);
                    builder.SetTitle("CONFIRM");
                    builder.SetMessage("ARE YOU SURE YOU WANT TO REMOVE THIS ITEM FROM THE CART");
                    builder.SetPositiveButton("YES", async delegate
                    {
                        try
                        {
                            HttpClient httpClient = new HttpClient();
                            var response = await httpClient.DeleteAsync($"{API.ApiUrl}/carts/{Items[e.Position].Id}");
                            if (response.IsSuccessStatusCode)
                            {
                                var str_results = await response.Content.ReadAsStringAsync();
                                AndHUD.Shared.ShowSuccess(context, $"{str_results}", MaskType.Clear, TimeSpan.FromSeconds(3));
                                Items.RemoveAt(e.Position);
                                adapter.NotifyItemRemoved(e.Position);
                                RefreshTotal();
                                if (Items.Count == 0)
                                {
                                    Btn_check_out.Enabled = false;
                                }

                            }
                            else
                            {
                                var str_results = await response.Content.ReadAsStringAsync();
                                AndHUD.Shared.ShowError(context, $"{str_results}".ToUpper(), MaskType.Clear, TimeSpan.FromSeconds(3));

                            }
                        }
                        catch (Exception ex)
                        {
                            AndHUD.Shared.ShowError(context, $"Something went wrong \n {ex.Message}".ToUpper(), MaskType.Clear, TimeSpan.FromSeconds(3));

                        }
                    });
                    builder.SetNegativeButton("NO", delegate
                    {
                        builder.Dispose();
                    });
                    builder.Show();
                };
            };

            Recycler.SetAdapter(adapter);
            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{API.ApiUrl}/carts/cust_cart/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CartModel>>(result);
                    if (data.Count > 0)
                    {
                        Btn_check_out.Enabled = true;
                    }
                    foreach (var item in data)
                    {
                        Items.Add(item);
                        adapter.NotifyDataSetChanged();
                    }
                    RefreshTotal();

                }
                else
                {
                    var result = await response.Content.ReadAsStringAsync();
                    AndHUD.Shared.ShowError(context, $"Something went wrong \n {result}".ToUpper(), MaskType.Clear, TimeSpan.FromSeconds(3));
                    Btn_check_out.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                AndHUD.Shared.ShowError(context, $"Something went wrong \n {ex.Message}", MaskType.Clear, TimeSpan.FromSeconds(3));
            }
        }
        double Latitude = 0.0;
        double Longitude = 0.0;
        string SelectedOption;
        private void OpenDialog()
        {
            int index = 0;
            string[] options = { "DELIVER", "EAT-IN", "COLLECT" };
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetTitle("CONFIRM ORDER");
            builder.SetNegativeButton("CANCEL", delegate
            {
                builder.Dispose();
            });
            builder.SetPositiveButton("CONTINUE", delegate
            {
                if (index >= 0)
                {
                    SelectedOption = options[index];

                    if (index == 0)
                    {
                        MapDialog mapDialog = new MapDialog();
                        mapDialog.Show(ChildFragmentManager.BeginTransaction(), "");
                        mapDialog.AddressChanged += MapDialog_AddressChanged;
                    }
                    else
                    {
                        ConfirmOrder();
                    }

                }
            });
            builder.SetSingleChoiceItems(options, -1, (s, e) =>
            {
                index = e.Which;

            });
            builder.Show();
        }

        private void ConfirmOrder()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetTitle("CONFIRM");
            builder.SetMessage("CLICK CONTINUE TO CONFIRM YOUR ORDER");
            builder.SetNegativeButton("CANCEL", delegate
            {
                builder.Dispose();
            });
            builder.SetPositiveButton("CONTINUE", delegate
            {
                PlaceOrdr();
                builder.Dispose();
            });
            builder.Show();
        }

        private void MapDialog_AddressChanged(object sender, MapDialog.AddressEventHandler e)
        {
            Latitude = e.Latitude;
            Longitude = e.Longitude;
            ConfirmOrder();
        }

        //add to cart
        private void Btn_check_out_Click(object sender, System.EventArgs e)
        {

            OpenDialog();

            //AndHUD.Shared.ShowSuccess(context, "Successfully updated", MaskType.Clear, TimeSpan.FromSeconds(3));
        }
        private async void PlaceOrdr()
        {
            try
            {
                int b_id = Items[0].BusinessId;
                List<OrderItems> orderItems = new List<OrderItems>();
                foreach (var item in Items)
                {
                    var d = new OrderItems()
                    {
                        Extras = item.Extras,
                        ItemName = item.ItemName,
                        Price = item.Price,
                        Quantity = item.Quantity,


                    };
                    orderItems.Add(d);
                }
                Order order = new Order()
                {
                    BusinessId = Items[0].BusinessId,
                    Customer_Id = Preferences.Get("Id", ""),
                    OrderItems = orderItems,
                    Status = "Pending",
                    OrderDateUtc = DateTime.UtcNow,
                    DriverId = "",
                    Longitude = Latitude,
                    Latitude = Longitude,
                    Option = SelectedOption
                };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(order);
                HttpClient httpClient = new HttpClient();
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{API.ApiUrl}/orders/order", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var str_results = await response.Content.ReadAsStringAsync();
                    AndHUD.Shared.ShowSuccess(context, str_results, MaskType.Clear, TimeSpan.FromSeconds(3));
                    Items.Clear();
                    adapter.NotifyDataSetChanged();
                    RefreshTotal();
                    Btn_check_out.Enabled = false;



                    var stream = Resources.Assets.Open("account_service.json");
                    var fcm = FirebaseAdminSDK.GetFirebaseMessaging(stream);
                    FirebaseAdmin.Messaging.Message message = new FirebaseAdmin.Messaging.Message()
                    {
                        Topic = b_id.ToString(),
                        Notification = new Notification()
                        {
                            Title = "ORDERED",
                            Body = $"Has placed an order.".ToUpper(),
                            ImageUrl = "https://firebasestorage.googleapis.com/v0/b/local-kota-15d60.appspot.com/o/app_icon%2Fapp_icon_04_4544852.png?alt=media&token=7bf1721d-a30d-4c4f-ad85-62f1d71f1769",
                        },
                    };
                    await fcm.SendAsync(message);

                }
                else
                {
                    var str_results = await response.Content.ReadAsStringAsync();
                    AndHUD.Shared.ShowError(context, str_results.ToUpper(), MaskType.Clear, TimeSpan.FromSeconds(3));
                }
            }
            catch (Exception EX)
            {
                AndHUD.Shared.ShowError(context, EX.Message.ToUpper(), MaskType.Clear, TimeSpan.FromSeconds(3));
            }


        }
        private void Menu_toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e)
        {
            this.Dismiss();
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            //remove event listener from firebase
        }

    }


}
