using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Kota_Palace.Adapters;
using Kota_Palace.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Essentials;
using static Android.Icu.Text.Transliterator;

namespace Kota_Palace.Frag_Tabs
{
    public class TabOrder : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.tab_order, container, false);
            ConnectViews(view);
            return view;
        }
        private readonly List<Order> Orders = new List<Order>();
        private Context context;
        OrderAdapter adapter;
        private async void ConnectViews(View view)
        {
            context = view.Context;
            var row_order = view.FindViewById<RecyclerView>(Resource.Id.row_order);
            adapter = new OrderAdapter(Orders);
            row_order.SetLayoutManager(new LinearLayoutManager(view.Context));
            row_order.SetAdapter(adapter);
            try
            {
                var id = Preferences.Get("Id", "");
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{API.ApiUrl}/orders/customer/{id}");
                if(response.IsSuccessStatusCode)
                {
                    var results = await response.Content.ReadAsStringAsync();
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Order>> (results);
                    foreach (var item in data)
                    {
                        Orders.Add(item);
                        adapter.NotifyDataSetChanged();
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                LoadSignalR();
            }
        }

        private async void LoadSignalR()
        {
            var id = Preferences.Get("Id", null);
            //id = Preferences.Get("Id", null);
            var hubConnection = new HubConnectionBuilder()
                .WithUrl("https://kotapalaceadmin.azurewebsites.net/OrderHub")
                .Build();
            hubConnection.On<string>(id, (response) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    
                    var order = Newtonsoft.Json.JsonConvert.DeserializeObject<Order>(response);
                    if(order.Status == "Completed")
                    {
                        Orders.RemoveAll(x => x.Id == order.Id);
                        adapter.NotifyDataSetChanged();
                    }
                    else
                    {
                        foreach (var item in Orders)
                        {
                            if(item.Id == order.Id)
                            {
                                item.Status = order.Status;
                                adapter.NotifyDataSetChanged();
                                break;

                            }
                        }
                    }
                    
                });


            });
            await hubConnection.StartAsync();
            if(hubConnection.State == HubConnectionState.Connected)
            {
                Toast.MakeText(context, "CONNECTED", ToastLength.Long).Show();
            }
        }
    }
}