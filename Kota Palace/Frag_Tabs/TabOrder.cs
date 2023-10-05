﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Kota_Palace.Adapters;
using Kota_Palace.Models;
using Xamarin.Essentials;

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
        private async void ConnectViews(View view)
        {
            var row_order = view.FindViewById<RecyclerView>(Resource.Id.row_order);
            OrderAdapter adapter = new OrderAdapter(Orders);
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
        }
    }
}