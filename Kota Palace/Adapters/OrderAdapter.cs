using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Chip;
using Google.Android.Material.TextView;
using Kota_Palace.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;

namespace Kota_Palace.Adapters
{
    internal class OrderAdapter : RecyclerView.Adapter
    {
        public event EventHandler<OrderAdapterClickEventArgs> ItemClick;
        public event EventHandler<OrderAdapterClickEventArgs> ItemLongClick;
        private readonly List<Order> Orders = new List<Order>();
        //private HubConnection hubConnection;
        public OrderAdapter(List<Order> data)
        {
            Orders = data;
            
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.order_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new OrderAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override  void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            //var item = Orders[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as OrderAdapterViewHolder;
            //holder.TextView.Text = items[position];
            decimal price = 0;
            holder.Items.RemoveAllViews();
            var inflator = LayoutInflater.From(holder.ItemView.Context);
            foreach (var item in Orders[position].OrderItems)
            {
                var chip = inflator.Inflate(Resource.Layout.chip_item, null, false) as Chip;

                chip.Clickable = false;
                chip.Text = $"{item.ItemName.ToUpper()}(*{item.Quantity})" ;
                price += (item.Price * item.Quantity);
                holder.Items.AddView(chip);
            }

            //fill in your items
            holder.Quantity.Text = $"{Orders[position].OrderItems.Count}".ToUpper();
            holder.Price.Text = $"R{price}".ToUpper();
            holder.Status.Text = $"{Orders[position].Status}".ToUpper();
            holder.ItemName.Text = $"{Orders[position].Id}".ToUpper();

            

        }

        public override int ItemCount => Orders.Count;

        void OnClick(OrderAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(OrderAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class OrderAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public MaterialTextView Quantity { get; set; }
        public MaterialTextView Price { get; set; }
        public MaterialTextView Status { get; set; }
        public MaterialTextView ItemName { get; set; } 
        public ChipGroup Items { get; set; }

        public OrderAdapterViewHolder(View itemView, Action<OrderAdapterClickEventArgs> clickListener,
                            Action<OrderAdapterClickEventArgs> longClickListener) : base(itemView)
        {


            Quantity = itemView.FindViewById<MaterialTextView>(Resource.Id.order_item_quantity);
            Status = itemView.FindViewById<MaterialTextView>(Resource.Id.order_item_status);
            Items = itemView.FindViewById<ChipGroup>(Resource.Id.order_item_products);
            Price = itemView.FindViewById<MaterialTextView>(Resource.Id.order_item_total_price);
            ItemName = itemView.FindViewById<MaterialTextView>(Resource.Id.order_item_name);
            //TextView = v;
            itemView.Click += (sender, e) => clickListener(new OrderAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new OrderAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }
    }

    public class OrderAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}